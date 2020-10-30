using System.IO;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Newtonsoft.Json;

public class FileTenantSource : ITenantSource
{
    public Tenant[] ListTenants()
    {
        var tenants = File.ReadAllText("tenants.json");

        return JsonConvert.DeserializeObject<Tenant[]>(tenants);
    }
}