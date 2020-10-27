using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace Electricity.WebUI.Services
{
    public class CurrentUserService : IUserProvider, ICurrentUserService
    {
        public readonly IUserSource _userSource;
        private readonly string _host;
        public string UserId { get; }

        public CurrentUserService(IUserSource userSource, IHttpContextAccessor httpContextAccessor)
        {
            _userSource = userSource;
            _host = httpContextAccessor.HttpContext?.Request.Host.ToString();

            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public User GetUser()
        {
            var users = _userSource.ListUsers();

            return users
                    .Where(u => u.Host.ToLower() == _host.ToLower())
                    .FirstOrDefault();
        }
    }
}
