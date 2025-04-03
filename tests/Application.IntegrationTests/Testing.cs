// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using NetCa.Api;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;
using NetCa.Domain.Constants;
using NetCa.Infrastructure.Data;

namespace NetCa.Application.IntegrationTests;

/// <summary>
/// Testing
/// </summary>
[SetUpFixture]
public class Testing
{
    private static ITestDatabase _database;

    private static WebApplicationFactory<Program> _factory = null!;

    /// <summary>
    /// Gets or sets AuthMock
    /// </summary>
    public static Mock<IUserAuthorizationService> AuthMock { get; set; }

    /// <summary>
    /// Gets or sets ScopeFactory
    /// </summary>
    public static IServiceScopeFactory ScopeFactory { get; set; }

    /// <summary>
    /// Gets or sets AppSetting
    /// </summary>
    public static AppSetting AppSetting { get; set; }

    /// <summary>
    /// Gets or sets Context
    /// </summary>
    public static ApplicationDbContext Context { get; set; }

    /// <summary>
    /// RunBeforeAnyTests
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        try
        {
            _factory = new CustomWebApplicationFactory();

            ScopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
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
                    })
                .Options;

            var provider = ScopeFactory.CreateScope().ServiceProvider;
            var userAuth = provider.GetRequiredService<IUserAuthorizationService>();
            var mediator = provider.GetRequiredService<IMediator>();
            var timeProvider = TimeProvider.System;

            Context = new ApplicationDbContext(options, userAuth, mediator, timeProvider, AppSetting);

            _database = await TestDatabaseFactory.CreateAsync(AppSetting).ConfigureAwait(false);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    /// <summary>
    /// SendAsync
    /// </summary>
    /// <typeparam name="TResponse"/>
    /// <param name="request"/>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = ScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetService<IMediator>();

        return await mediator.Send(request).ConfigureAwait(false);
    }

    /// <summary>
    /// AddAsync
    /// </summary>
    /// <typeparam name="TEntity"/>
    /// <param name="entity"/>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        Context.Add(entity);

        await Context.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// FindAsync
    /// </summary>
    /// <typeparam name="TEntity"/>
    /// <param name="keyValues"/>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task<TEntity> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        return await Context.FindAsync<TEntity>(keyValues).ConfigureAwait(false);
    }

    /// <summary>
    /// CountAsync
    /// </summary>
    /// <typeparam name="TEntity"/>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task<int> CountAsync<TEntity>()
        where TEntity : class
    {
        return await Context.Set<TEntity>().CountAsync().ConfigureAwait(false);
    }

    #region MockingData

    /// <summary>
    /// MockUserId
    /// </summary>
    /// <param name="userId"/>
    public static void MockUserId(Guid userId)
    {
        using var scope = ScopeFactory.CreateScope();
        var userAuthorizationService = scope
            .ServiceProvider
            .GetService<IUserAuthorizationService>();

        var mock = userAuthorizationService.GetAuthorizedUser();
        mock.UserId = userId;

        AuthMock.Setup(x => x.GetAuthorizedUser()).Returns(mock);
    }

    /// <summary>
    /// MockEmail
    /// </summary>
    /// <param name="email"/>
    public static void MockEmail(string email = SystemConstants.Email)
    {
        var mock = MockData.GetUserEmailInfo();
        mock.Email = email;

        AuthMock
            .Setup(x => x.GetEmailByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mock);
    }

    /// <summary>
    /// MockUserCustomerCode
    /// </summary>
    /// <param name="customerCode"/>
    public static void MockUserCustomerCode(string customerCode = "*")
    {
        using var scope = ScopeFactory.CreateScope();
        var userAuthorizationService = scope
            .ServiceProvider
            .GetService<IUserAuthorizationService>();

        var mock = userAuthorizationService.GetAuthorizedUser();
        mock.CustomerCode = customerCode;

        AuthMock.Setup(x => x.GetAuthorizedUser()).Returns(mock);
    }

    /// <summary>
    /// MockAttribute
    /// </summary>
    /// <param name="customerCode"/>
    /// <param name="plantCode"/>
    /// <param name="customerSiteCode"/>
    /// <param name="abc"/>
    public static void MockAttribute(
        List<string> customerCode = null,
        List<string> plantCode = null,
        List<string> customerSiteCode = null,
        List<string> abc = null)
    {
        customerCode ??= ["*"];
        plantCode ??= ["*"];
        customerSiteCode ??= ["*"];
        abc ??= ["*"];

        var dict = new Dictionary<string, List<string>>
        {
            { UserAttributeConstants.CustomerName, customerCode },
            { UserAttributeConstants.PlantFieldName, plantCode },
            { UserAttributeConstants.CustomerSiteFieldName, customerSiteCode },
            { UserAttributeConstants.ABCFieldName, abc }
        };

        AuthMock
            .Setup(x => x.GetUserAttributesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(dict);
    }

    #endregion

    /// <summary>
    /// ResetData
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task ResetData()
    {
        try
        {
            await _database.ResetAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    /// <summary>
    /// RunAfterAnyTests
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [OneTimeTearDown]
    public static async Task RunAfterAnyTests()
    {
        await ResetData().ConfigureAwait(false);
        await _database.DisposeAsync().ConfigureAwait(false);
        await _factory.DisposeAsync().ConfigureAwait(false);
    }
}
