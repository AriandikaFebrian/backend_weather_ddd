// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetCa.Api;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;
using NetCa.Domain.Constants;
using NetCa.Infrastructure.Data;

namespace NetCa.Application.IntegrationTests;

using static Testing;

/// <summary>
/// CustomWebApplicationFactory
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private static Mock<IProducerService> ProduceMock { get; set; }

    private static Mock<IRedisService> RedisMock { get; set; }

    /// <summary>
    /// ConfigureWebHost
    /// </summary>
    /// <param name="builder"/>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        builder.ConfigureServices(
            (_, services) =>
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Test.json", true, true)
                    .AddJsonFile(
                        $"appsettings.Test.Local.json",
                        optional: true,
                        reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                AppSetting = configuration.Get<AppSetting>();

                services
                    .RemoveAll<IWebHostEnvironment>()
                    .AddSingleton(
                        Mock.Of<IWebHostEnvironment>(
                            x =>
                                x.EnvironmentName == EnvironmentConstants.NameTest
                                && x.ApplicationName == EnvironmentConstants.ApplicationName));

                services.AddLogging();

                AuthMock = new Mock<IUserAuthorizationService>();

                services
                    .RemoveAll<IUserAuthorizationService>()
                    .AddTransient(_ =>
                    {
                        AuthMock
                            .Setup(x => x.GetAuthorizedUser())
                            .Returns(MockData.GetAuthorizedUser());
                        AuthMock.Setup(x => x.GetUserNameSystem()).Returns(SystemConstants.Name);

                        return AuthMock.Object;
                    });

                ProduceMock = new Mock<IProducerService>();

                services
                    .RemoveAll<IProducerService>()
                    .AddSingleton(
                        _ =>
                            ProduceMock
                                .Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>()))
                                .ReturnsAsync(true));

                RedisMock = new Mock<IRedisService>();

                services
                    .RemoveAll<IRedisService>()
                    .AddSingleton(_ =>
                    {
                        RedisMock.Setup(
                            x =>
                                x.SaveSetAsync(
                                    It.IsAny<string>(),
                                    It.IsAny<string>(),
                                    It.IsAny<string[]>(),
                                    false));
                        RedisMock
                            .Setup(
                                x =>
                                    x.SaveSubAsync(
                                        It.IsAny<string>(),
                                        It.IsAny<string>(),
                                        It.IsAny<string>(),
                                        false))
                            .ReturnsAsync("KEYS");

                        return RedisMock.Object;
                    });

                services
                    .RemoveAll<DbContextOptions<ApplicationDbContext>>()
                    .AddEntityFrameworkNpgsql()
                    .AddDbContext<ApplicationDbContext>(
                        (provider, optionsBuilder) =>
                            optionsBuilder
                                .AddInterceptors(provider.GetServices<ISaveChangesInterceptor>())
                                .UseNpgsql(
                                    AppSetting.ConnectionStrings.DefaultConnection,
                                    x =>
                                    {
                                        x.MigrationsAssembly(
                                            typeof(ApplicationDbContext).Assembly.FullName);
                                        x.CommandTimeout(
                                            AppSetting.DatabaseSettings.CommandTimeout);
                                        x.EnableRetryOnFailure(
                                            AppSetting.DatabaseSettings.MaxRetryCount,
                                            TimeSpan.FromSeconds(
                                                AppSetting.DatabaseSettings.MaxRetryDelay),
                                            null);
                                    }),
                        ServiceLifetime.Transient);
            });
    }
}
