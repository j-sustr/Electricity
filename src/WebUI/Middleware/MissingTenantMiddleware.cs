using System.Threading.Tasks;
using Electricity.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Electricity.WebUI.Middleware
{
    class MissingTenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _missingTenantUrl;

        public MissingTenantMiddleware(RequestDelegate next, string missingUserUrl)
        {
            _next = next;
            _missingTenantUrl = missingUserUrl;
        }

        public async Task Invoke(HttpContext httpContext, ITenantProvider tenant)
        {
            if (tenant.GetTenant() == null)
            {
                httpContext.Response.Redirect(_missingTenantUrl);
                return;
            }

            await _next.Invoke(httpContext);
        }
    }
}