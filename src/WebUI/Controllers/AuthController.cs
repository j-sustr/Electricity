using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Electricity.Application.Common.Models.Dtos;
using NJsonSchema.Annotations;
using Electricity.Application.Common.Interfaces;

namespace Electricity.WebUI.Controllers
{
    public class AuthController : ApiController
    {
        private readonly Application.Common.Interfaces.IAuthenticationService _authService;
        private readonly ITenantProvider _tenantProvider;

        public AuthController(
            Application.Common.Interfaces.IAuthenticationService authService,
            ITenantProvider tenantProvider
            )
        {
            _authService = authService;
            _tenantProvider = tenantProvider;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(string username, string password)
        {
            var guid = _authService.Login(username, password);
            if (guid == Guid.Empty)
            {
                return Unauthorized("Login failed");
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

            return Ok(new UserDto
            {
                Username = username
            });
        }

        [HttpPost("logout")]
        [Authorize]
        [AllowAnonymous]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return Ok();
        }

        [return: CanBeNull]
        [HttpGet("current-user")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var tenant = _tenantProvider.GetTenant();
            if (tenant == null)
            {
                await HttpContext.SignOutAsync();
                return null;
            }

            var user = HttpContext.User;
            if (user?.Identity.IsAuthenticated != true)
            {
                return null;
            }

            return Ok(new UserDto
            {
                Username = user.FindFirstValue("username")
            });
        }
    }
}