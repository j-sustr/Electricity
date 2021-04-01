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
        private readonly ITenantProvider _tenantProvider;

        public UserController(ITenantProvider tenantProvider)
        {
            _tenantProvider = tenantProvider;
        }

        [HttpGet("current-user")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var tenant = _tenantProvider.GetTenant();
            if (tenant == null)
            {
                await HttpContext.SignOutAsync();
                return Ok(null);
            }

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
    }
}