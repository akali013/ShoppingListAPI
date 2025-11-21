using Microsoft.Extensions.Options;
using ShoppingListAPI.Helpers;
using ShoppingListAPI.Services;

namespace ShoppingListAPI.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = jwtUtils.ValidateJwtToken(token);
            if (userId != null)
            {
                // Attach the user object when a valid JWT is found
                context.Items["User"] = userService.GetUserById(userId.Value);
            }

            await _next(context);
        }
    }
}
