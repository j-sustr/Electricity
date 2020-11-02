using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Electricity.WebUI.Services
{
    public class KeyBasedTenantProvider
    {
        private readonly ITenantSource _tenantSource;
        private readonly string _key;

        public KeyBasedTenantProvider(ITenantSource tenantSource, IHttpContextAccessor accessor)
        {
            _tenantSource = tenantSource;
            _key = accessor.HttpContext.Request.Query["Tenant"].ToString();
        }

        public Tenant GetTenant()
        {
            var tenants = _tenantSource.ListTenants();

            return tenants
                    .Where(t => t.Key == _key)
                    .FirstOrDefault();
        }
    }
}