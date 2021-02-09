using System;
using CleanArchitecture.Infrastructure.Identity;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Infrastructure.DataSource;
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
                return new FakeDataSourceFactory(0, new BoundedInterval(new DateTime(2021, 1, 1), new DateTime(2021, 3, 1)));
            });
            services.AddSingleton<IDataSourceManager, DataSourceManager>();

            services.AddScoped<IGroupService, ApplicationDataSource>();
            services.AddScoped<IQuantityService, ApplicationDataSource>();
            services.AddScoped<ITableCollection, ApplicationDataSource>();
            services.AddScoped<Application.Common.Interfaces.IAuthenticationService, ApplicationDataSource>();

            services.AddTransient<IIdentityService, FakeIdentityService>();

            return services;
        }
    }
}