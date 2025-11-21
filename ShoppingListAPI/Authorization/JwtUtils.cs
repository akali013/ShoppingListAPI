using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShoppingListAPI.Data;
using ShoppingListAPI.Helpers;
using ShoppingListAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ShoppingListAPI.Authorization
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(User user);
        public Guid? ValidateJwtToken(string token);
        public RefreshToken GenerateRefreshToken(string ipAddress);
    }
    public class JwtUtils : IJwtUtils
    {
        private ShoppingListAPIContext _context;
        private readonly AppSettings _appSettings;

        public JwtUtils(ShoppingListAPIContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        public string GenerateJwtToken(User user)
        {
            // Generate access token that's valid for 15 minutes
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Store the user ID in the token claims
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public Guid? ValidateJwtToken(string token)
        {
            if (token == null) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // Get the user Id from the token claims
                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "id").Value;

                // Return the user ID from the JWT token if the validation succeeded
                return Guid.Parse(userId);
            }
            catch
            {
                // Return null if validation fails
                return null;
            }
        }

        public RefreshToken GenerateRefreshToken(string ipAddress)
        {
            var refreshToken = new RefreshToken
            {
                Token = getUniqueToken(),
                // Refresh token is valid for 7 days
                Expires = DateTime.UtcNow.AddDays(7),
                // Creation Date and IP Address are logged for security auditing
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            return refreshToken;

            string getUniqueToken()
            {
                // Token is a random sequence of values
                var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                // Ensure token is unique by checking against the MSSQL db
                var tokenIsUnique = !_context.Users.Any(user => user.RefreshTokens.Any(t => t.Token == token));

                if (!tokenIsUnique)
                {
                    return getUniqueToken();
                }

                return token;
            }
        }
    }
}
