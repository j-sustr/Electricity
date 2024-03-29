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
using Electricity.Application.DataSource.Commands.OpenDataSource;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Common.Enums;
using Electricity.Application.IntegrationTests;
using Electricity.Infrastructure.DataSource.Abstractions;
using Electricity.Application.Common.Extensions;
using KMB.DataSource;
using Electricity.Application.Common.Abstractions;

[SetUpFixture]
public class Testing
{
    private static IConfigurationRoot _configuration;
    private static IServiceScopeFactory _scopeFactory;

    public static IMultiTenantContextAccessor<Tenant> Debug_MultiTenantContextAccessor;

    private static int _fakeHttpContextTraceIdentifier = 0;
    private static HttpContext _fakeHttpContext;
    private static FakeSession _fakeSession;
    private static string _currentUserId;

    private static FakeDataSourceFactory _dataSourceFactory;

    private static IDataSourceCache _dataSourceCache;

    public static IServiceScope ServiceScope { get; set; }
    public static string UserId { get { return _currentUserId; } }
    public static HttpContext HttpContext { get { return _fakeHttpContext; } }

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json", true, true)
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

        EnsureDataSourceFactory(services);
        EnsureHttpContextAccessor(services);
        EnsureMultiTenantContextAccessor(services);

        var serviceProvider = services.BuildServiceProvider();
        _scopeFactory = serviceProvider.GetService<IServiceScopeFactory>();

        _dataSourceCache = serviceProvider.GetService<IDataSourceCache>();
    }

    public static async Task<DataSourceInfoDto> OpenFakeDataSourceAsync()
    {
        return await SendAsync(new OpenDataSourceCommand
        {
            Tenant = new TenantDto
            {
                DataSourceType = DataSourceType.DB,
                DBConnectionParams = new DBConnectionParams
                {
                    Server = "server",
                    DBName = "db-name",
                    Password = "pass",
                    Username = "user"
                },
            }
        });
    }

    public static void EnsureDataSourceFactory(ServiceCollection services)
    {
        var dsFactoryDescriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IDataSourceFactory));
        services.Remove(dsFactoryDescriptor);

        _dataSourceFactory = new FakeDataSourceFactory(0);
        services.AddSingleton<IDataSourceFactory>(provider => _dataSourceFactory);
    }

    public static void EnsureHttpContextAccessor(ServiceCollection services)
    {
        var httpContextAccessorDescriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IHttpContextAccessor));
        services.Remove(httpContextAccessorDescriptor);

        var mockAccessor = new Mock<IHttpContextAccessor>();
        mockAccessor.SetupGet(a => a.HttpContext)
            .Returns(() => _fakeHttpContext);

        services.AddSingleton(mockAccessor.Object);

        //services.AddSingleton(provider =>
        //    Mock.Of<IHttpContextAccessor>(a =>
        //        a.HttpContext == _fakeHttpContext));
    }

    public static void EnsureMultiTenantContextAccessor(ServiceCollection services)
    {
        // mocking MultiTenantContextAccessor because it uses AsyncLocal<T>

        var descriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IMultiTenantContextAccessor<Tenant>));
        services.Remove(descriptor);

        var mockAccessor = new Mock<IMultiTenantContextAccessor<Tenant>>();
        mockAccessor.SetupProperty(a => a.MultiTenantContext, null);

        services.AddSingleton(mockAccessor.Object);
    }

    public static void CreateSession()
    {
        _fakeSession = new FakeSession();
    }

    public static async Task CreateHttpContext(IServiceScope scope)
    {
        // using var scope = CreateServiceScope();
        var serviceProvider = scope.ServiceProvider;

        var mockHttpContext = Mock.Of<HttpContext>(c =>
            c.RequestServices == serviceProvider &&
            c.Session == _fakeSession
        );
        mockHttpContext.TraceIdentifier = (_fakeHttpContextTraceIdentifier++).ToString();

        await CreateMultiTenantContext(mockHttpContext);

        _fakeHttpContext = mockHttpContext;
    }

    public static async Task CreateMultiTenantContext(HttpContext context)
    {
        // substitude for MultiTenantMiddleware.Invoke(HttpContext context)

        var accessor = context.RequestServices.GetRequiredService<IMultiTenantContextAccessor<Tenant>>();
        Debug_MultiTenantContextAccessor = accessor;

        if (accessor.MultiTenantContext == null)
        {
            var resolver = context.RequestServices.GetRequiredService<ITenantResolver<Tenant>>();
            var multiTenantContext = (IMultiTenantContext<Tenant>)await resolver.ResolveAsync(context);
            accessor.MultiTenantContext = multiTenantContext;
        }
    }

    public static async Task RunAsDefaultTenantAndUser()
    {
        CreateServiceScope();
        await CreateHttpContext(ServiceScope);
        await OpenFakeDataSourceAsync();
        await CreateHttpContext(ServiceScope);

        await RunAsDefaultUserAsync();
    }

    public static async Task<string> RunAsDefaultUserAsync()
    {
        return RunAsUser("TestUser1", "1", new string[] { });
    }

    public static string RunAsUser(string userName, string password, string[] roles)
    {
        using var scope = _scopeFactory.CreateScope();

        var authService = scope.ServiceProvider.GetService<IAuthenticationService>();

        var guid = authService.Login(userName, password);

        _currentUserId = guid.ToString();

        return _currentUserId;
    }

    public static GroupInfo GetGroupTree()
    {
        var user = Array.Find(_dataSourceFactory.Users, user => user.UserId.ToString() == _currentUserId);
        if (user == null)
            throw new Exception("no current user");

        return user.GroupTree;
    }

    public static string GetRecordGroupIdByName(string name)
    {
        var groupTree = GetGroupTree();
        var recordGroups = groupTree.GetUserRecordGroupInfos();
        var g = Array.Find(recordGroups, g => g.Name == name);
        if (g == null)
        {
            return null;
        }
        return g.ID.ToString();
    }

    public static int GetRecordGroupCount()
    {
        var groupTree = GetGroupTree();
        return groupTree.GetUserRecordGroupInfos().Length;
    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        // using var scope = _scopeFactory.CreateScope();
        var scope = ServiceScope;

        var mediator = scope.ServiceProvider.GetService<ISender>();

        return await mediator.Send(request);
    }

    public static IServiceScope CreateServiceScope()
    {
        var scope = _scopeFactory.CreateScope();
        ServiceScope = scope;
        return scope;
    }

    public static void ResetState()
    {
        if (ServiceScope != null)
            ServiceScope.Dispose();
        ServiceScope = null;

        _fakeHttpContextTraceIdentifier = 0;
        _fakeHttpContext = null;
        _fakeSession = null;
        _currentUserId = null;

        _dataSourceCache.Clear();
    }
}