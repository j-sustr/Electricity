using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Electricity.Application.Common.Interfaces;
using Electricity.Infrastructure.DataSource.Fake;
using Electricity.WebUI;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Finbuckle.MultiTenant;
using Electricity.Application.Common.Models;
using Finbuckle.MultiTenant.Core;
using Electricity.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using CleanArchitecture.Infrastructure.Identity;

[SetUpFixture]
public class Testing
{
    private static IConfigurationRoot _configuration;
    private static IServiceScopeFactory _scopeFactory;

    private static string _currentUserId;

    private static FakeDataSourceFactory _fakeDataSourceFactory;

    private static FakeIdentityService _fakeIdentityService;

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

        services.Remove(currentUserServiceDescriptor);

        // Register testing version
        services.AddTransient(provider =>
            Mock.Of<ICurrentUserService>(s => s.UserId == _currentUserId));

        EnsureHttpContext(services);
        EnsureDataSource(services);
        AddFakeIdentityService(services);

        _scopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();
    }

    public static void EnsureDataSource(ServiceCollection services)
    {
        var dsFactoryDescriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IDataSourceFactory));

        services.Remove(dsFactoryDescriptor);

        var interval = new BoundedInterval(new DateTime(2021, 1, 1), new DateTime(2021, 1, 31));

        var dataSourceFactory = new FakeDataSourceFactory(0, interval);
        services.AddSingleton<IDataSourceFactory>(provider => dataSourceFactory);
    }

    public static void EnsureHttpContext(ServiceCollection services)
    {
        var httpContextAccessorDescriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IHttpContextAccessor));

        services.Remove(httpContextAccessorDescriptor);

        var ti = new Tenant { Id = "test-tenant-id" };
        var tc = new MultiTenantContext<Tenant>();
        tc.TenantInfo = ti;

        var requestServices = new ServiceCollection();
        requestServices.AddTransient<IMultiTenantContextAccessor<Tenant>>(_ =>
            new MultiTenantContextAccessor<Tenant> { MultiTenantContext = tc });
        var sp = requestServices.BuildServiceProvider();

        // var claimsIdentiy = new ClaimsIdentity(new Claim[] {
        //     new Claim(ClaimTypes.NameIdentifier, "test-user-id")
        // });
        // var claimsPrincipal = new ClaimsPrincipal();
        // claimsPrincipal.AddIdentity(new ClaimsIdentity())

        var mockHttpContext = Mock.Of<HttpContext>(c =>
            c.RequestServices == sp
            );

        services.AddScoped(provider =>
            Mock.Of<IHttpContextAccessor>(a =>
                a.HttpContext == mockHttpContext));
    }

    public static void AddFakeIdentityService(ServiceCollection services)
    {
        var idensityServiceDescriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IIdentityService));

        services.Remove(idensityServiceDescriptor);

        var identityService = new FakeIdentityService();
        services.AddSingleton<IIdentityService>(provider => identityService);
    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetService<ISender>();

        return await mediator.Send(request);
    }

    public static async Task<string> RunAsDefaultUserAsync()
    {
        return await RunAsUserAsync("test@local", "Testing1234!", new string[] { });
    }

    public static async Task<string> RunAsUserAsync(string userName, string password, string[] roles)
    {
        using var scope = _scopeFactory.CreateScope();

        var identityService = scope.ServiceProvider.GetService<IIdentityService>();

        var (result, userId) = await identityService.CreateUserAsync(userName, password);

        // if (roles.Any())
        // {
        //     var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

        //     foreach (var role in roles)
        //     {
        //         await roleManager.CreateAsync(new IdentityRole(role));
        //     }

        //     await userManager.AddToRolesAsync(user, roles);
        // }

        if (result.Succeeded)
        {
            _currentUserId = userId;

            return _currentUserId;
        }

        var errors = string.Join(Environment.NewLine, result.Errors);

        throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
    }
}