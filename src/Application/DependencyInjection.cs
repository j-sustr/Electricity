using AutoMapper;
using Electricity.Application.Common.Behaviours;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Electricity.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

            services.AddTransient<ArchiveRepositoryService>();
            services.AddScoped<IDataSourceManager, DataSourceManager>();

            services.AddScoped<ApplicationDataSource>(); // DEBUG
            services.AddScoped<IGroupRepository, ApplicationDataSource>();
            services.AddScoped<IArchiveRepository, ApplicationDataSource>();
            services.AddScoped<IAuthenticationService, ApplicationDataSource>();

            return services;
        }
    }
}