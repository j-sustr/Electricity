using System.Threading.Tasks;
using Electricity.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Electricity.WebUI.Middleware
{
    class MissingUserMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _missingUserUrl;

        public MissingUserMiddleware(RequestDelegate next, string missingUserUrl)
        {
            _next = next;
            _missingUserUrl = missingUserUrl;
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