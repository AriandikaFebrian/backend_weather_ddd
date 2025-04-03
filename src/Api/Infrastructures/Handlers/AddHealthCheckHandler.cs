// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Azure.Messaging.EventHubs.Producer;
using HealthChecks.Network;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;
using NetCa.Domain.Constants;

namespace NetCa.Api.Infrastructures.Handlers;

/// <summary>
/// AddHealthCheckHandler
/// </summary>
public static class AddHealthCheckHandler
{
    private const string _healthQuery = HealthCheckConstants.Query;

    /// <summary>
    /// AddHealthCheck
    /// </summary>
    /// <param name="services"></param>
    /// <param name="appSetting"></param>
    public static void AddHealthChecks(this IServiceCollection services, AppSetting appSetting)
    {
        services.AddHealthChecks().AddCheck<SystemCpuHealthCheck>(HealthCheckConstants.CpuUsage);
        services
            .AddHealthChecks()
            .AddCheck<SystemMemoryHealthCheck>(HealthCheckConstants.MemoryUsage);
        services.AddHealthChecks().AddCheck<UmsHealthCheck>(HealthCheckConstants.UmsName);

        services
            .AddHealthChecks()
            .AddNpgSql(
                appSetting.ConnectionStrings.DefaultConnection,
                name: HealthCheckConstants.DatabaseName,
                healthQuery: _healthQuery,
                failureStatus: HealthStatus.Degraded,
                timeout: TimeSpan.FromSeconds(HealthCheckConstants.TimeoutInSeconds)
            );

        foreach (var eventHub in appSetting.Messaging.AzureEventHubs)
        {
            services
                .AddHealthChecks()
                .AddAzureBlobStorage(
                    connectionString: eventHub.StorageAccount,
                    name: $"{HealthCheckConstants.StorageAccountEventHub}_{eventHub.Name}",
                    failureStatus: HealthStatus.Degraded,
                    timeout: TimeSpan.FromSeconds(HealthCheckConstants.TimeoutInSeconds)
                );

            foreach (var topic in eventHub.Topics)
            {
                services
                    .AddHealthChecks()
                    .AddAzureEventHub(
                        clientFactory: _ =>
                            new EventHubProducerClient(eventHub.ConnectionString, topic.Value),
                        name: $"{HealthCheckConstants.EventHub}_{eventHub.Name}_{topic.Name}",
                        HealthStatus.Degraded,
                        timeout: TimeSpan.FromSeconds(HealthCheckConstants.TimeoutInSeconds)
                    );
            }
        }

        services
            .AddHealthChecks()
            .AddRedis(
                appSetting.Redis.Server,
                HealthCheckConstants.RedisName,
                HealthStatus.Degraded,
                timeout: TimeSpan.FromSeconds(HealthCheckConstants.TimeoutInSeconds)
            );

        var gateWay = new Uri(appSetting.AuthorizationServer.Gateway);
        void GateWaySetup(TcpHealthCheckOptions x) => x.AddHost(gateWay.Host, 443);
        services
            .AddHealthChecks()
            .AddTcpHealthCheck(
                GateWaySetup,
                HealthCheckConstants.GateWayName,
                HealthStatus.Degraded,
                timeout: TimeSpan.FromSeconds(HealthCheckConstants.TimeoutInSeconds)
            );
    }

    /// <summary>
    /// UseHealthCheck
    /// </summary>
    /// <param name="app"></param>
    public static void UseHealthCheck(this IApplicationBuilder app)
    {
        app.UseHealthChecks(
            "/health",
            new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UiResponseWriter.WriteHealthCheckUiResponse
            }
        );
    }
}

/// <summary>
/// SystemCpuHealthCheck
/// </summary>
public class SystemCpuHealthCheck : IHealthCheck
{
    /// <summary>
    /// CheckHealthAsync
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        var startTime = DateTime.UtcNow;
        var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

        await Task.Delay(500, cancellationToken).ConfigureAwait(false);

        var endTime = DateTime.UtcNow;
        var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

        var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
        var totalMsPassed = (endTime - startTime).TotalMilliseconds;
        var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

        var data = new Dictionary<string, object> { { "Cpu_Usage", cpuUsageTotal } };

        var result = new HealthCheckResult(HealthStatus.Healthy, data: data);

        return await Task.FromResult(result).ConfigureAwait(false);
    }
}

/// <summary>
/// SystemMemoryHealthCheck
/// </summary>
public class SystemMemoryHealthCheck : IHealthCheck
{
    /// <summary>
    /// CheckHealthAsync
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        var metrics = MemoryMetricsClient.GetMetrics();
        var percentUsed = 100 * metrics.Used / metrics.Total;

        var status = HealthStatus.Healthy;
        if (percentUsed > HealthCheckConstants.PercentageUsedDegraded)
        {
            status = HealthStatus.Degraded;
        }

        var data = new Dictionary<string, object>
        {
            { "Total_MB", metrics.Total },
            { "Used_MB", metrics.Used },
            { "Free_MB", metrics.Free }
        };

        var result = new HealthCheckResult(status, data: data);

        return await Task.FromResult(result).ConfigureAwait(false);
    }
}

/// <summary>
/// UmsHealthCheck
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UmsHealthCheck"/> class.
/// </remarks>
/// <param name="_userAuthorizationService">Set userAuthorizationService to get User's Attributes</param>
public class UmsHealthCheck(IUserAuthorizationService _userAuthorizationService) : IHealthCheck
{
    /// <summary>
    /// CheckHealthAsync
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        var status = HealthStatus.Healthy;

        try
        {
            var statusCode = await _userAuthorizationService
                .CheckHealthAsync(cancellationToken)
                .ConfigureAwait(false);

            if (statusCode != HttpStatusCode.OK)
            {
                status = HealthStatus.Degraded;
            }
        }
        catch
        {
            status = HealthStatus.Degraded;
        }

        var result = new HealthCheckResult(status);

        return await Task.FromResult(result).ConfigureAwait(false);
    }
}

/// <summary>
/// MemoryMetrics
/// </summary>
public class MemoryMetrics
{
    /// <summary>
    /// Gets or sets total memory
    /// </summary>
    public double Total { get; set; }

    /// <summary>
    /// Gets or sets used memory
    /// </summary>
    public double Used { get; set; }

    /// <summary>
    /// Gets or sets free memory
    /// </summary>
    public double Free { get; set; }
}

/// <summary>
/// MemoryMetricsClient
/// </summary>
public static class MemoryMetricsClient
{
    /// <summary>
    /// GetMetrics
    /// </summary>
    /// <returns></returns>
    public static MemoryMetrics GetMetrics()
    {
        var metrics = IsUnix() ? GetUnixMetrics() : GetWindowsMetrics();

        return metrics;
    }

    private static bool IsUnix() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        || RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    private static MemoryMetrics GetWindowsMetrics()
    {
        var info = new ProcessStartInfo
        {
            FileName = "wmic",
            Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value",
            RedirectStandardOutput = true
        };

        using var process = Process.Start(info);
        var output = process.StandardOutput.ReadToEnd();

        var lines = output.Trim().Split("\n");
        var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
        var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);

        var metrics = new MemoryMetrics
        {
            Total = Math.Round(
                double.Parse(totalMemoryParts[1], CultureInfo.InvariantCulture) / 1024,
                0
            ),
            Free = Math.Round(
                double.Parse(freeMemoryParts[1], CultureInfo.InvariantCulture) / 1024,
                0
            )
        };
        metrics.Used = metrics.Total - metrics.Free;

        return metrics;
    }

    private static MemoryMetrics GetUnixMetrics()
    {
        var info = new ProcessStartInfo("free -m")
        {
            FileName = "/bin/bash",
            Arguments = "-c \"free -m\"",
            RedirectStandardOutput = true
        };

        using var process = Process.Start(info);
        var output = process.StandardOutput.ReadToEnd();

        var lines = output.Split("\n");
        var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

        var metrics = new MemoryMetrics
        {
            Total = double.Parse(memory[1], CultureInfo.InvariantCulture),
            Used = double.Parse(memory[2], CultureInfo.InvariantCulture),
            Free = double.Parse(memory[3], CultureInfo.InvariantCulture)
        };

        return metrics;
    }
}

/// <summary>
/// UiHealthReport
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UiHealthReport"/> class.
/// </remarks>
/// <param name="entries"></param>
/// <param name="totalDuration"></param>
public class UiHealthReport(Dictionary<string, UiHealthReportEntry> entries, TimeSpan totalDuration)
{
    /// <summary>
    /// Gets or sets status
    /// </summary>
    public UiHealthStatus Status { get; set; }

    /// <summary>
    /// Gets or sets totalDuration
    /// </summary>
    public TimeSpan TotalDuration { get; set; } = totalDuration;

    /// <summary>
    /// Gets entries
    /// </summary>
    public Dictionary<string, UiHealthReportEntry> Entries { get; } = entries;

    /// <summary>
    /// CreateFrom
    /// </summary>
    /// <param name="report"></param>
    /// <returns></returns>
    public static UiHealthReport CreateFrom(HealthReport report)
    {
        var uiReport = new UiHealthReport([], report.TotalDuration)
        {
            Status = (UiHealthStatus)report.Status,
        };

        foreach (var (key, value) in report.Entries)
        {
            var entry = new UiHealthReportEntry
            {
                Data = value.Data,
                Description = value.Description,
                Duration = value.Duration,
                Status = (UiHealthStatus)value.Status
            };

            if (value.Exception != null)
            {
                var message = value.Exception?.Message;

                entry.Exception = message;
                entry.Description = value.Description ?? message;
            }

            uiReport.Entries.Add(key, entry);
        }

        return uiReport;
    }

    /// <summary>
    /// CreateFrom
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="entryName"></param>
    /// <returns></returns>
    public static UiHealthReport CreateFrom(Exception exception, string entryName = "Endpoint")
    {
        var uiReport = new UiHealthReport([], TimeSpan.FromSeconds(0))
        {
            Status = UiHealthStatus.Unhealthy,
        };

        uiReport
            .Entries
            .Add(
                entryName,
                new UiHealthReportEntry
                {
                    Exception = exception.Message,
                    Description = exception.Message,
                    Duration = TimeSpan.FromSeconds(0),
                    Status = UiHealthStatus.Unhealthy
                }
            );

        return uiReport;
    }
}

/// <summary>
/// UiHealthStatus
/// </summary>
public enum UiHealthStatus
{
    /// <summary>
    /// Unhealthy
    /// </summary>
    Unhealthy = 0,

    /// <summary>
    /// Degraded
    /// </summary>
    Degraded = 1,

    /// <summary>
    /// Healthy
    /// </summary>
    Healthy = 2
}

/// <summary>
/// UiHealthReportEntry
/// </summary>
public class UiHealthReportEntry
{
    /// <summary>
    /// Gets or Sets Data
    /// </summary>
    /// <value></value>
    public IReadOnlyDictionary<string, object> Data { get; set; }

    /// <summary>
    /// Gets or Sets Description
    /// </summary>
    /// <value></value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or Sets Duration
    /// </summary>
    /// <value></value>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Gets or Sets Exception
    /// </summary>
    /// <value></value>
    public string Exception { get; set; }

    /// <summary>
    /// Gets or Sets Status
    /// </summary>
    /// <value></value>
    public UiHealthStatus Status { get; set; }
}

/// <summary>
/// UiResponseWriter
/// </summary>
public static class UiResponseWriter
{
    private static readonly byte[] EmptyResponse = [(byte)'{', (byte)'}'];
    private static readonly Lazy<JsonSerializerOptions> Options = new(CreateJsonOptions);

    /// <summary>
    /// WriteHealthCheckUIResponse
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="report"></param>
    /// <returns></returns>
    public static async Task WriteHealthCheckUiResponse(
        HttpContext httpContext,
        HealthReport report
    )
    {
        if (report == null)
        {
            await httpContext.Response.BodyWriter.WriteAsync(EmptyResponse).ConfigureAwait(false);
            return;
        }

        httpContext.Response.ContentType = HeaderConstants.Json;

        var uiReport = UiHealthReport.CreateFrom(report);

        var stream = new MemoryStream();
        await using (stream.ConfigureAwait(false))
        {
            if (uiReport.Status != UiHealthStatus.Healthy)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            await JsonSerializer
                .SerializeAsync(stream, uiReport, Options.Value)
                .ConfigureAwait(false);

            await httpContext
                .Response
                .BodyWriter
                .WriteAsync(stream.ToArray())
                .ConfigureAwait(false);
        }
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var jsonSerializeOptions = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        jsonSerializeOptions.Converters.Add(new JsonStringEnumConverter());

        jsonSerializeOptions.Converters.Add(new TimeSpanConverter());

        return jsonSerializeOptions;
    }
}

/// <summary>
/// TimeSpanConverter
/// </summary>
internal sealed class TimeSpanConverter : JsonConverter<TimeSpan>
{
    /// <summary>
    /// Read
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public override TimeSpan Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        return default;
    }

    /// <summary>
    /// Write
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
