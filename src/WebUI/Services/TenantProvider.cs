using System.Linq;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Finbuckle.MultiTenant;

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor accessor)
    {
        _httpContextAccessor = accessor;
    }

    public Tenant GetTenant()
    {
        return _httpContextAccessor.HttpContext
            .GetMultiTenantContext<Tenant>()?.TenantInfo;
    }
}