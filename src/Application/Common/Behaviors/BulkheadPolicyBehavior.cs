// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Polly.Bulkhead;

namespace NetCa.Application.Common.Behaviors;

/// <summary>
/// Applies a fallback policy on the MediatR request.
/// Apply this attribute to the MediatR <see cref="IRequest"/> class (not on the handler).
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class BulkheadPolicyAttribute : Attribute
{
    private int _maxParallelization = 100;
    private int _maxQueuingActions = 20;

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets to enabling/disabling policy.
    /// Defaults to true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the max parallelization.
    /// Defaults to 100 parallel.
    /// </summary>
    public int MaxParallelization
    {
        get => _maxParallelization;
        set
        {
            if (value < 1)
            {
                throw new ArgumentException(
                    "Max parallelization count must be higher than 1.",
                    nameof(value));
            }

            _maxParallelization = value;
        }
    }

    /// <summary>
    /// Gets or sets the max queuing actions.
    /// Defaults to 20 queues.
    /// </summary>
    public int MaxQueuingActions
    {
        get => _maxQueuingActions;
        set
        {
            if (value < 1)
            {
                throw new ArgumentException(
                    "Max queuing actions count must be higher than 1.",
                    nameof(value));
            }

            _maxQueuingActions = value;
        }
    }
}

/// <summary>
/// Wraps request handler execution of requests decorated with the <see cref="BulkheadPolicyAttribute"/>
/// inside a policy to handle transient bulk head the execution.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="BulkheadPolicyBehavior{TRequest, TResponse}"/> class.
/// </remarks>
/// <param name="_logger"></param>
/// <param name="appSetting"></param>
/// <param name="_environment"></param>
public class BulkheadPolicyBehavior<TRequest, TResponse>(
    ILogger<BulkheadPolicyBehavior<TRequest, TResponse>> _logger,
    AppSetting appSetting,
    IWebHostEnvironment _environment
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly string _env = appSetting.Environment;
    private readonly Bulkhead _bulkHeadPolicy = appSetting.ResiliencyPolicy.Bulkhead;
    private readonly string _requestName = typeof(TRequest).Name;

    private AsyncBulkheadPolicy<TResponse> _bulkHead;

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

        var bulkHeadAttr = typeof(TRequest).GetCustomAttribute<BulkheadPolicyAttribute>();

        if (
            (bulkHeadAttr != null && !bulkHeadAttr.Enabled)
            || (bulkHeadAttr == null && !_bulkHeadPolicy.Enabled)
        )
        {
            return await next().ConfigureAwait(false);
        }

        _bulkHead ??= Polly
            .Policy
            .BulkheadAsync<TResponse>(
                bulkHeadAttr?.MaxParallelization ?? _bulkHeadPolicy.MaxParallelization,
                bulkHeadAttr?.MaxQueuingActions ?? _bulkHeadPolicy.MaxQueuingActions,
                (_) =>
                {
                    _logger.LogInformation(
                        "Bulkhead limit reached for request {name}",
                        _requestName);
                    return Task.CompletedTask;
                });

        return await _bulkHead.ExecuteAsync(() => next()).ConfigureAwait(false);
    }
}
