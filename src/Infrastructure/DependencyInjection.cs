using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Services;
using Electricity.Infrastructure.DataSource;
using Electricity.Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Electricity.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITenantSource, FileTenantSource>();
            services.AddSingleton<IDataSourceManager, DataSourceManager>();

            services.AddScoped<IGroupService, ApplicationDataSource>();
            services.AddScoped<IQuantityService, ApplicationDataSource>();
            services.AddScoped<IRowCollectionReader, ApplicationDataSource>();
            services.AddScoped<Application.Common.Interfaces.IAuthenticationService, ApplicationDataSource>();

            // services.AddTransient<IIdentityService, IdentityService>();

            return services;
        }
    }
}
