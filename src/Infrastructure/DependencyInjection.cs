using Electricity.Application.Common.Abstractions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Services;
using Electricity.Infrastructure.DataSource;
using Electricity.Infrastructure.DataSource.Abstractions;
using Electricity.Infrastructure.DataSource.Fake;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Electricity.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IDataSourceFactory>((provider) =>
            {
                return new FakeDataSourceFactory(0);
            });
            // services.AddSingleton<IDataSourceFactory, DataSourceFactory>();
            services.AddSingleton<IDataSourceCache, DataSourceCache>();

            return services;
        }
    }
}