using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using System;
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

            var tenantService = scope.ServiceProvider.GetService<ITenantProvider>();
            var tenant = tenantService.GetTenant();

            tenant.DataSourceId.Should().NotBe(Guid.Empty);
            tenant.DBConnectionParams.Should().NotBeNull();
        }

        [Test]
        public async Task ShouldReturnEmptyGuidOnBadLogin()
        {
            using var scope = CreateServiceScope();
            await CreateHttpContext(scope);
            await OpenFakeDataSourceAsync();
            await CreateHttpContext(scope);

            var authService = scope.ServiceProvider.GetService<IAuthenticationService>();

            var guid = authService.Login("Invalid name", "Invalid password");

            guid.Should().Be(Guid.Empty);
        }
    }
}