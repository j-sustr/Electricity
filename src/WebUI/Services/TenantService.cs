using System.Linq;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Finbuckle.MultiTenant;
using Electricity.Application.Common.Abstractions;

public class TenantService : ITenantService, ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantService(IHttpContextAccessor accessor)
    {
        _httpContextAccessor = accessor;
    }

    public bool SetTenantIdentifier(string identifier)
    {
        var session = _httpContextAccessor.HttpContext.Session;
        if (session == null) return false;

        session.SetString("__tenant__", identifier);
        return true;
    }

    public Tenant GetTenant()
    {
        return _httpContextAccessor.HttpContext
            .GetMultiTenantContext<Tenant>()?.TenantInfo;
    }
}