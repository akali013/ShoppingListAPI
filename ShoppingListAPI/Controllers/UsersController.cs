using Microsoft.AspNetCore.Mvc;
using ShoppingListAPI.Authorization;
using ShoppingListAPI.Models;
using ShoppingListAPI.Services;

namespace ShoppingListAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model, ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = _userService.RefreshToken(refreshToken, ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public IActionResult RevokeToken()
        {
            // Accept the refresh token in the cookie
            var token = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token is required" });
            }

            _userService.RevokeToken(token, ipAddress());
            return Ok(new { message = "Token revoked" });
        }

        [HttpGet("{id}/refresh-tokens")]
        public IActionResult GetRefreshTokens(Guid id)
        {
            var user = _userService.GetUserById(id);
            return Ok(user.RefreshTokens);
        }

        [HttpPut("credentials")]
        public IActionResult UpdateCredentials(CredentialsRequest request)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            _userService.UpdateCredentials(refreshToken, request);
            return Ok(new {message = "Credentials updated!"});
        }


        private void setTokenCookie(string token)
        {
            // Append cookie with refresh token to the HTTP response
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                MaxAge = TimeSpan.FromDays(7),
                Secure = true,
                SameSite = SameSiteMode.None,

            };

            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            // Get the source IP from the current request
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"]!;
            }
            else
            {
                return HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString();
            }
        }
    }
}
