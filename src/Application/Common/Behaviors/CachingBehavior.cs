// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace NetCa.Application.Common.Behaviors;

/// <summary>
/// MediatR Caching Pipeline Behavior
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="CachingBehavior{TRequest, TResponse}"/> class.
/// </remarks>
/// <param name="_cache"></param>
/// <param name="_userAuthorizationService"></param>
/// <param name="_logger"></param>
/// <param name="_cachePolicies"></param>
/// <param name="_environment"></param>
public class CachingBehavior<TRequest, TResponse>(
    ICache _cache,
    IUserAuthorizationService _userAuthorizationService,
    ILogger<CachingBehavior<TRequest, TResponse>> _logger,
    IEnumerable<ICachePolicy<TRequest, TResponse>> _cachePolicies,
    IWebHostEnvironment _environment
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
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

        var cachePolicy = _cachePolicies.FirstOrDefault();

        if (cachePolicy == null)
        {
            return await next().ConfigureAwait(false);
        }

        var customerCode = _userAuthorizationService.GetCustomerCode();
        var attributes = await _userAuthorizationService
            .GetUserAttributesAsync(cancellationToken)
            .ConfigureAwait(false);

        var cacheKey = cachePolicy.GetCacheKey(request, customerCode, attributes);
        var cachedResponse = await _cache
            .GetAsync<TResponse>(cacheKey, cancellationToken)
            .ConfigureAwait(false);

        var requestName = typeof(TRequest).Name;

        if (cachedResponse != null)
        {
            _logger.LogDebug(
                "Response retrieved {requestName} from cache. CacheKey: {cacheKey}",
                requestName,
                cacheKey);

            return cachedResponse;
        }

        var response = await next().ConfigureAwait(false);

        _logger.LogDebug(
            "Caching response for {requestName} with cache key: {cacheKey}",
            requestName,
            cacheKey);

        await _cache
            .SetAsync(
                cacheKey,
                response,
                cachePolicy.SlidingExpiration,
                cachePolicy.AbsoluteExpiration,
                cachePolicy.AbsoluteExpirationRelativeToNow,
                cancellationToken)
            .ConfigureAwait(false);

        return response;
    }
}
