// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Polly.RateLimit;

namespace NetCa.Application.Common.Behaviors;

/// <summary>
/// Applies a rate limit policy on the MediatR request.
/// Apply this attribute to the MediatR <see cref="IRequest"/> class (not on the handler).
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class RateLimitPolicyAttribute : Attribute
{
    private int _numberOfAllowedExecutions = 100;
    private int _durationLimit = 1;
    private int _burst = 1;

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets to enabling/disabling policy.
    /// Defaults to true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the amount of times execution allowed per duration.
    /// Defaults to 100 times.
    /// </summary>
    public int NumberOfAllowedExecutions
    {
        get => _numberOfAllowedExecutions;
        set
        {
            if (value < 1)
            {
                throw new ArgumentException(
                    "Number Of Executions must be higher than 1.",
                    nameof(value));
            }

            _numberOfAllowedExecutions = value;
        }
    }

    /// <summary>
    /// Gets or sets the duration limit in seconds.
    /// Defaults to 1 seconds.
    /// </summary>
    public int DurationLimit
    {
        get => _durationLimit;
        set
        {
            if (value < 1)
            {
                throw new ArgumentException(
                    "Duration limit must be higher than 1 second.",
                    nameof(value));
            }

            _durationLimit = value;
        }
    }

    /// <summary>
    /// Gets or sets the burst.
    /// Defaults to 1.
    /// </summary>
    public int Burst
    {
        get => _burst;
        set
        {
            if (value < 1)
            {
                throw new ArgumentException("Burst value must be higher than 1.", nameof(value));
            }

            _burst = value;
        }
    }
}

/// <summary>
/// Wraps request handler execution of requests decorated with the <see cref="RateLimitPolicyAttribute"/>
/// inside a policy to handle transient rate limit the execution.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="RateLimitPolicyBehavior{TRequest, TResponse}"/> class.
/// </remarks>
/// <param name="appSetting"></param>
/// <param name="_environment"></param>
public class RateLimitPolicyBehavior<TRequest, TResponse>(
    AppSetting appSetting,
    IWebHostEnvironment _environment
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly string _env = appSetting.Environment;
    private readonly RateLimit _rateLimitPolicy = appSetting.ResiliencyPolicy.RateLimit;

    private AsyncRateLimitPolicy<TResponse> _rateLimit;

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
        if (
            _environment.EnvironmentName.Equals(EnvironmentConstants.NameTest)
            || !_env.Equals(EnvironmentConstants.IsProduction)
        )
        {
            return await next().ConfigureAwait(false);
        }

        var rateLimitAttr = typeof(TRequest).GetCustomAttribute<RateLimitPolicyAttribute>();

        if (
            (rateLimitAttr != null && !rateLimitAttr.Enabled)
            || (rateLimitAttr == null && !_rateLimitPolicy.Enabled)
        )
        {
            return await next().ConfigureAwait(false);
        }

        _rateLimit ??= Polly
            .Policy
            .RateLimitAsync<TResponse>(
                rateLimitAttr?.NumberOfAllowedExecutions
                    ?? _rateLimitPolicy.NumberOfAllowedExecutions,
                TimeSpan.FromSeconds(
                    rateLimitAttr?.DurationLimit ?? _rateLimitPolicy.DurationLimit),
                rateLimitAttr?.Burst ?? _rateLimitPolicy.Burst);

        return await _rateLimit.ExecuteAsync(() => next()).ConfigureAwait(false);
    }
}
