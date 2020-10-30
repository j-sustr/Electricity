using System.Linq;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Microsoft.AspNetCore.Http;

public class WebTenantProvider : ITenantProvider
{
    private readonly ITenantSource _tenantSource;
    private readonly string _host;

    public WebTenantProvider(ITenantSource tenantSource, IHttpContextAccessor accessor)
    {
        _tenantSource = tenantSource;
        _host = accessor.HttpContext.Request.Host.ToString();
    }

    public Tenant GetTenant()
    {
        var tenants = _tenantSource.ListTenants();

        return tenants
                .Where(t => t.Host.ToLower() == _host.ToLower())
                .FirstOrDefault();
    }
}