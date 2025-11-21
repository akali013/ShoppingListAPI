using System.Text.Json.Serialization;

namespace ShoppingListAPI.Models
{
    public class AuthenticateResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string JwtToken { get; set; }
        [JsonIgnore]    // Refresh token is returned only in a HTTP only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(User user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            Email = user.Email;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
