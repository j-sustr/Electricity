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
            await OpenFakeDataSourceAsync();

            using var scope = CreateServiceScope();

            // var store = scope.ServiceProvider.GetService<IMultiTenantStore<Tenant>>();
            // var tenantResolver = scope.ServiceProvider.GetService<ITenantResolver<Tenant>>();
            // var multitenantStrategy = scope.ServiceProvider.GetService<IMultiTenantStrategy>();
            // var httpContextAccesssor = scope.ServiceProvider.GetService<IHttpContextAccessor>();

            // var identifier = await multitenantStrategy.GetIdentifierAsync(httpContextAccesssor.HttpContext);
            // var tenant = await store.TryGetByIdentifierAsync(identifier);

            var tenantService = scope.ServiceProvider.GetService<ITenantProvider>();
            var tenant = tenantService.GetTenant();

            tenant.DataSourceId.Should().NotBe(Guid.Empty);
            tenant.DBConnectionParams.Should().NotBeNull();

            Debug.Write("");
        }
    }
}