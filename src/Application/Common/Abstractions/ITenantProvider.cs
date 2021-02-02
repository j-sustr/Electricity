using Electricity.Application.Common.Models;

namespace Electricity.Application.Common.Interfaces
{
    public interface ITenantProvider
    {
        Tenant GetTenant();
    }
}