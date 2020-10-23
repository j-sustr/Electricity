using System.Threading.Tasks;
using Electricity.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Electricity.WebUI.Middleware
{
    class MissingUserMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _missingUserUrl;

        public MissingUserMiddleware(RequestDelegate next, string missingTenantUrl)
        {
            _next = next;
            _missingUserUrl = missingTenantUrl;
        }

        public async Task Invoke(HttpContext httpContext, IUserProvider provider)
        {
            if (provider.GetUser() == null)
            {
                httpContext.Response.Redirect(_missingUserUrl);
                return;
            }

            await _next.Invoke(httpContext);
        }
    }
}