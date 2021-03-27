using Electricity.Application.Common.Models;
using Finbuckle.MultiTenant;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using FluentAssertions;
using System;
using Moq;
using Microsoft.AspNetCore.Http;
using Electricity.Application.Common.Abstractions;
using Electricity.Application.Common.Interfaces;

namespace Electricity.Application.IntegrationTests.DataSource.Command
{
    using static Testing;

    public class OpenDataSourceTest : TestBase
    {
        [Test]
        public async Task ShouldOpenDBDataSource()
        {
            using var scope = CreateServiceScope();
            await CreateHttpContext(scope);
            await OpenFakeDataSourceAsync();
            await CreateHttpContext(scope);

            var accessor3 = MultiTenantContextAccessor;

            var accessor2 = HttpContext.RequestServices.GetRequiredService<IMultiTenantContextAccessor<Tenant>>();

            var accessor1 = scope.ServiceProvider.GetRequiredService<IMultiTenantContextAccessor<Tenant>>();

            var tenantService = scope.ServiceProvider.GetService<ITenantProvider>();
            var tenant = tenantService.GetTenant();

            tenant.DataSourceId.Should().NotBe(Guid.Empty);
            tenant.DBConnectionParams.Should().NotBeNull();

            Debug.Write("");
        }
    }
}