using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShoppingListAPI.Authorization;
using ShoppingListAPI.Data;
using ShoppingListAPI.Helpers;
using ShoppingListAPI.Models;
using BC = BCrypt.Net.BCrypt;

namespace ShoppingListAPI.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        void RevokeToken(string token, string ipAddress);
        User GetUserById(Guid id);
        void UpdateCredentials(string refreshToken, CredentialsRequest model);
    }

    public class UserService : IUserService
    {
        private ShoppingListAPIContext _context;
        private IJwtUtils _jwtUtils;
        private readonly AppSettings _appSettings;

        public UserService(ShoppingListAPIContext context, IJwtUtils jwtUtils, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var user = _context.Users.FirstOrDefault<User>(user => user.Email == model.Email);

            // Check if user exists and the password matches
            if (user == null || !BC.Verify(model.Password, user.Password))
            {
                throw new AppException("Username or password is incorrect");
            }

            // Generate JWT and refresh token for successful auth
            var jwtToken = _jwtUtils.GenerateJwtToken(user);
            var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            user.RefreshTokens.Add(refreshToken);

            // Remove old refresh tokens from the user
            removeOldRefreshTokens(user);

            _context.Users.Update(user);
            _context.SaveChanges();

            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var user = getUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (refreshToken.IsRevoked)
            {
                // Remove all descendant tokens if this token is compromised
                revokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
                _context.Users.Update(user);
                _context.SaveChanges();
            }

            if (!refreshToken.IsActive)
            {
                throw new AppException("Invalid token");
            }

            // Rotate old refresh token with a new one
            var newRefreshToken = rotateRefreshToken(refreshToken, ipAddress);
            user.RefreshTokens.Add(newRefreshToken);
            removeOldRefreshTokens(user);

            _context.Users.Update(user);
            _context.SaveChanges();

            var jwtToken = _jwtUtils.GenerateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
        }

        public void RevokeToken(string token, string ipAddress)
        {
            var user = getUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                throw new AppException("Invalid token");
            }

            revokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
            _context.Users.Update(user);
            _context.SaveChangesAsync();
        }

        public User GetUserById(Guid id)
        {
            var user = _context.Users.FirstOrDefault<User>(u => u.Id == id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return user;
        }

        public void UpdateCredentials(string refreshToken, CredentialsRequest model)
        {
            User user = getUserByRefreshToken(refreshToken);
            user.Email = model.Email;
            user.Password = BC.HashPassword(model.Password);

            _context.Users.Update(user);
            _context.SaveChanges();
        }


        private User getUserByRefreshToken(string token)
        {
            var user = _context.Users.SingleOrDefault<User>(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                throw new AppException("Invalid token");
            }

            return user;
        }

        private RefreshToken rotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            revokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        private void removeOldRefreshTokens(User user)
        {
            // Remove old inactive refresh tokens from user based on TTL in appSettings.json
            user.RefreshTokens.RemoveAll(token => !token.IsActive && token.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private void revokeDescendantRefreshTokens(RefreshToken refreshToken, User user, string ipAddress, string reason)
        {
            // Recursively remove refresh tokens and their descendants
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = user.RefreshTokens.SingleOrDefault(token => token.Token == refreshToken.ReplacedByToken);
                if (childToken.IsActive)
                {
                    revokeRefreshToken(childToken, ipAddress, reason);
                }
                else
                {
                    revokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
                }
            }
        }

        private void revokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }
    }
}
