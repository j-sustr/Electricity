using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Electricity.Application.Common.Interfaces;
using Electricity.Infrastructure.DataSource;
using Electricity.WebUI;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

[SetUpFixture]
public class Testing
{
    private static IConfigurationRoot _configuration;
    private static IServiceScopeFactory _scopeFactory;

    private static Guid _currentUserId;

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables();

        _configuration = builder.Build();

        var startup = new Startup(_configuration);

        var services = new ServiceCollection();

        services.AddSingleton(Mock.Of<IWebHostEnvironment>(w =>
            w.EnvironmentName == "Development" &&
            w.ApplicationName == "Electricity.WebUI"));

        services.AddLogging();

        startup.ConfigureServices(services);

        // Replace service registration for ICurrentUserService
        // Remove existing registration
        var currentUserServiceDescriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(ICurrentUserService));
        var tableCollectionDescriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(ITableCollection));
        var httpContextAccessorDescriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IHttpContextAccessor));

        services.Remove(currentUserServiceDescriptor);
        services.Remove(tableCollectionDescriptor);

        var dataSource = new FakeApplicationDataSource(0);

        // Register testing version
        services.AddTransient(provider =>
            Mock.Of<ICurrentUserService>(s => s.UserId == _currentUserId));
        services.AddScoped<ITableCollection>(provider => dataSource);

        services.AddScoped(provider =>
            Mock.Of<IHttpContextAccessor>(a =>
                a.HttpContext == Mock.Of<HttpContext>(c =>
                    c.Request == Mock.Of<HttpRequest>(r => r.Host == new HostString("localhost", 5000)))));

        _scopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();
    }


    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetService<ISender>();

        return await mediator.Send(request);
    }
}