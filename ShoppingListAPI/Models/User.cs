using Microsoft.EntityFrameworkCore;
using ShoppingListAPI.Data;
using System.Text.Json.Serialization;
using BC = BCrypt.Net.BCrypt;

namespace ShoppingListAPI.Models
{
    public class User
    {
        private ShoppingListAPIContext _context;

        public User(ShoppingListAPIContext context)
        {
            _context = context;
        }

        // Constructor for creating new users
        public User(AuthenticateRequest model)
        {
            Id = Guid.NewGuid();
            Email = model.Email;
            Password = BC.HashPassword(model.Password);
            RefreshTokens = new List<RefreshToken>();
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
