using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCa.Api;
using NetCa.Api.Infrastructures.Extensions;
using NetCa.Api.Infrastructures.Handlers;
using NetCa.Api.Infrastructures.Middlewares;
using NetCa.Application;
using NetCa.Application.Common.Mappings;
using NetCa.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using NSwag;
using Serilog;
using Serilog.Filters;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("WeatherApi", client =>
{
    client.BaseAddress = new Uri("https://api.openweathermap.org/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .WithOrigins(
                "https://different-scorpion-ariandika-71bdbea0.koyeb.app"
            )
            .AllowAnyMethod()
            .AllowAnyHeader());
});
builder
    .Host
    .UseSerilog(
        (hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
            loggerConfiguration
                .Filter
                .ByExcluding(Matching.FromSource<ExceptionHandlerMiddleware>());
        }
    );

builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder
    .Configuration
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, false);
builder.Configuration.AddJsonFile("appsettings.Local.json", true, true);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

var appSetting = builder.Configuration.Get<AppSetting>();

builder
    .WebHost
    .UseKestrel(option =>
    {
        option.Limits.MaxRequestBodySize = appSetting.Kestrel.MaxRequestBodySize;
        option.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(
            appSetting.Kestrel.KeepAliveTimeoutInM
        );
        option.Limits.MinRequestBodyDataRate = new MinDataRate(
            appSetting.Kestrel.MinRequestBodyDataRate.BytesPerSecond,
            TimeSpan.FromSeconds(appSetting.Kestrel.MinRequestBodyDataRate.GracePeriod)
        );
        option.Limits.MinResponseDataRate = new MinDataRate(
            appSetting.Kestrel.MinResponseDataRate.BytesPerSecond,
            TimeSpan.FromSeconds(appSetting.Kestrel.MinResponseDataRate.GracePeriod)
        );
        option.AddServerHeader = false;
    });

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Environment, appSetting);
builder.Services.AddApiServices(builder.Environment, appSetting);
builder.Services.AddAutoMapper(typeof(MappingProfile));

if (!appSetting.IsEnableDetailError)
{
    Log.Debug("Activating exception middleware.");

    builder.Services.AddExceptionHandler<CustomExceptionHandler>();
}
else
{
    Log.Warning("Enabling detail error response.");
}

var app = builder.Build();

// ✅ Gunakan CORS sebelum middleware autentikasi
app.UseCors("AllowAll");

app.UseCorsOriginHandler(appSetting);

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsHandler(app.Environment, appSetting);
    app.UseMigrationsEndPoint();
    app.UseHttpsRedirection();
    app.UseDeveloperExceptionPage();

    var option = new RewriteOptions();
    option.AddRedirect("^$", "swagger");
    app.UseRewriter(option);
}
else
{
    app.UseHsts();
}

app.UseHealthCheck();

app.UseOpenApi(
    x =>
        x.PostProcess = (document, _) =>
        {
            document.Schemes = [OpenApiSchema.Https, OpenApiSchema.Http];
        }
);

app.UseSwaggerUi(settings =>
{
    settings.Path = "/swagger/v1/swagger.json";
    settings.EnableTryItOut = true;
});

if (appSetting.IsEnableAuth)
{
    app.UseAuthenticationHandler();
}
else
{
    Log.Warning("The authentication middleware is disabled.");
}

app.UseResponseCompression();
app.UseOverrideRequestHandler();
app.UseOverrideResponseHandler();

app.UseExceptionHandler(_ => { });
app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });

app.MapControllers();
app.MapDefaultControllerRoute();
app.MapEndpoints();
app.MapRazorPages();

#pragma warning restore ASP0014
Log.Information("Starting host");

app.Run();

namespace NetCa.Api
{
    /// <summary>
    ///     Program
    /// </summary>
    public class Program
    {
    }
}
