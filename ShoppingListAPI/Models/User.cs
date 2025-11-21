using Microsoft.EntityFrameworkCore;
using ShoppingListAPI.Data;
using System.Text.Json.Serialization;

namespace ShoppingListAPI.Models
{
    public class User
    {
        private static ShoppingListAPIContext _context;
        public User(ShoppingListAPIContext context)
        {
            _context = context;
        }

        //public async Task LoadRefreshTokens()
        //{
        //    var refreshTokens = await _context.RefreshTokens.ToArrayAsync();

        //    var tokens = refreshTokens.Where(token => token.UserId == this.Id).ToList();

        //    this.RefreshTokens = tokens;
        //}

        public Guid Id { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
