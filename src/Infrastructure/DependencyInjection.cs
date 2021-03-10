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
                var start = DateTime.SpecifyKind(new DateTime(2021, 1, 1), DateTimeKind.Local);
                var end = DateTime.SpecifyKind(new DateTime(2021, 3, 1), DateTimeKind.Local);
                return new FakeDataSourceFactory(0, new BoundedInterval(start.ToUniversalTime(), end.ToUniversalTime()));
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