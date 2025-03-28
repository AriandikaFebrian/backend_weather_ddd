// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Polly.Timeout;
using Timeout = NetCa.Application.Common.Models.Timeout;

namespace NetCa.Application.Common.Behaviors;

/// <summary>
/// Applies a timeout policy on the MediatR request.
/// Apply this attribute to the MediatR <see cref="IRequest"/> class (not on the handler).
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class TimeoutPolicyAttribute : Attribute
{
    private int _duration = 180;

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets to enabling/disabling policy.
    /// Defaults to true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the timeout duration of the execution.
    /// Defaults to 180 seconds.
    /// </summary>
    public int Duration
    {
        get => _duration;
        set
        {
            if (value < 1)
            {
                throw new ArgumentException(
                    "Duration must be higher than 1 seconds.",
                    nameof(value));
            }

            _duration = value;
        }
    }
}

/// <summary>
/// Wraps request handler execution of requests decorated with the <see cref="TimeoutPolicyAttribute"/>
/// inside a policy to handle transient timeout policy of the execution.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="TimeoutPolicyBehavior{TRequest, TResponse}"/> class.
/// </remarks>
/// <param name="_logger"></param>
/// <param name="appSetting"></param>
/// <param name="_environment"></param>
public class TimeoutPolicyBehavior<TRequest, TResponse>(
    ILogger<TimeoutPolicyBehavior<TRequest, TResponse>> _logger,
    AppSetting appSetting,
    IWebHostEnvironment _environment
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly Timeout _timeoutPolicy = appSetting.ResiliencyPolicy.Timeout;
    private readonly string _requestName = typeof(TRequest).Name;

    private AsyncTimeoutPolicy<TResponse> _timeout;

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_environment.EnvironmentName.Equals(EnvironmentConstants.NameTest))
        {
            return await next().ConfigureAwait(false);
        }

        var timeoutAttr = typeof(TRequest).GetCustomAttribute<TimeoutPolicyAttribute>();

        if (
            (timeoutAttr != null && !timeoutAttr.Enabled)
            || (timeoutAttr == null && !_timeoutPolicy.Enabled)
        )
        {
            return await next().ConfigureAwait(false);
        }

        _timeout ??= Polly
            .Policy
            .TimeoutAsync<TResponse>(
                timeoutAttr?.Duration ?? _timeoutPolicy.Duration,
                TimeoutStrategy.Pessimistic,
                (_, _, _, _) =>
                {
                    _logger.LogInformation("Timeout reached for request {name}", _requestName);
                    return Task.CompletedTask;
                });

        return await _timeout.ExecuteAsync(() => next()).ConfigureAwait(false);
    }
}
