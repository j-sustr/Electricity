using Electricity.Application.Common.Abstractions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Services;
using Electricity.Infrastructure.DataSource;
using Electricity.Infrastructure.DataSource.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Electricity.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddSingleton<IDataSourceFactory>((provider) =>
            //{
            //    var start = DateTime.SpecifyKind(new DateTime(2021, 1, 1), DateTimeKind.Local);
            //    var end = DateTime.SpecifyKind(new DateTime(2021, 3, 1), DateTimeKind.Local);
            //    return new FakeDataSourceFactory(0, new BoundedInterval(start.ToUniversalTime(), end.ToUniversalTime()));
            //});
            services.AddSingleton<IDataSourceFactory, DataSourceFactory>();
            services.AddSingleton<IDataSourceCache, DataSourceCache>();
            services.AddScoped<IDataSourceManager, DataSourceManager>();

            services.AddScoped<ApplicationDataSource>(); // DEBUG
            services.AddScoped<IGroupRepository, ApplicationDataSource>();
            services.AddScoped<IQuantityService, ApplicationDataSource>();
            services.AddScoped<IArchiveRepository, ApplicationDataSource>();
            services.AddScoped<IAuthenticationService, ApplicationDataSource>();

            return services;
        }
    }
}