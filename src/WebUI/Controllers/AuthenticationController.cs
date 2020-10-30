using System;
using Electricity.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Electricity.WebUI.Controllers
{
    public class AuthenticationController : ApiController
    {
        private readonly IAuthenticationService _service;

        public AuthenticationController(IAuthenticationService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<Guid> Authenticate(string username, string password)
        {
            Guid id = Guid.Empty;

            try
            {
                id = _service.Login(username, password);
            }
            catch (System.Exception)
            {
                return BadRequest("invalid credentials");
            }

            return Ok(id);
        }

    }
}