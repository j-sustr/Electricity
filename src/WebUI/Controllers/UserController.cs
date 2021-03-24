using Electricity.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Electricity.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ApiController
    {
        private readonly IAuthenticationService _authService;

        private readonly ILogger<UserController> _logger;

        public UserController(IAuthenticationService authService, ILogger<UserController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public void Login(string username, string password)
        {
            var guid = _authService.Login(username, password);
        }
    }
}