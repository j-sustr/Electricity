using System.IO;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Newtonsoft.Json;

public class FileTenantSource : ITenantSource
{
    private Tenant[] _tenants { get; set; } = null;

    public Tenant[] ListTenants()
    {
        if (_tenants == null)
        {
            var text = File.ReadAllText("tenants.json");

            _tenants = JsonConvert.DeserializeObject<Tenant[]>(text);
        }

        return _tenants;
    }
}