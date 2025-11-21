namespace ShoppingListAPI.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }

        // Refresh token TTL in days for inactive refresh tokens in the db
        public int RefreshTokenTTL { get; set; }
    }
}
