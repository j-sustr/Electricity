using System.Threading.Tasks;
using Electricity.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Electricity.WebUI.Middleware
{
    class MissingTenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _missingTenantLocation;

        public MissingTenantMiddleware(RequestDelegate next, string missingTenantLocation)
        {
            _next = next;
            _missingTenantLocation = missingTenantLocation;
        }

        public async Task Invoke(HttpContext httpContext, ITenantProvider tenant)
        {
            if (tenant.GetTenant() == null)
            {
                httpContext.Response.Redirect(_missingTenantLocation);
                return;
            }

            await _next.Invoke(httpContext);
        }
    }
}