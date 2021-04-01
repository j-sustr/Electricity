using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Electricity.WebUI.Controllers
{
    public class UserController : ApiController
    {
        private readonly Application.Common.Interfaces.IAuthenticationService _authService;

        private readonly ILogger<UserController> _logger;

        public UserController(
            Application.Common.Interfaces.IAuthenticationService authService,
            ILogger<UserController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet("current-user")]
        public ActionResult<UserDto> GetCurrentUser()
        {
            var user = HttpContext.User;
            if (user == null)
            {
                return Ok(null);
            }

            return Ok(new UserDto
            {
                Username = user.FindFirstValue("username")
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(string username, string password)
        {
            var guid = _authService.Login(username, password);
            if (guid == Guid.Empty)
            {
                return BadRequest();
            }

            var claims = new List<Claim>
            {
                new Claim("username", username),
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Role, "User"),
                new Claim("id", guid.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A
                // value set here overrides the ExpireTimeSpan option of
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok();
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return Ok();
        }
    }
}