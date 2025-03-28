// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Threading;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Exceptions;
using NetCa.Domain.Constants;
using Newtonsoft.Json;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.RateLimit;

namespace NetCa.Api.Infrastructures.Handlers;

/// <summary>
/// CustomExceptionHandler
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CustomExceptionHandler"/> class.
/// </remarks>
public class CustomExceptionHandler : IExceptionHandler
{
    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;
    private readonly JsonSerializerSettings _jsonSettings =
        JsonExtensions.ErrorSerializerSettings();

    private readonly ILogger<CustomExceptionHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomExceptionHandler"/> class.
    /// </summary>
    /// <param name="logger"></param>
    public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
    {
        _logger = logger;

        // Register known exception types and handlers.
        _exceptionHandlers = new()
        {
            { typeof(ValidationException), HandleValidationException },
            { typeof(BadRequestException), HandleBadRequestException },
            { typeof(NotFoundException), HandleNotFoundException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
            { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
            { typeof(BulkheadRejectedException), HandleServiceUnavailableException },
            { typeof(BrokenCircuitException), HandleServiceUnavailableException },
            { typeof(RateLimitRejectedException), HandleTooManyRequestsException },
        };
    }

    /// <inheritdoc/>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var exceptionType = exception.GetType();

        if (_exceptionHandlers.TryGetValue(exceptionType, out var value))
        {
            await value.Invoke(httpContext, exception).ConfigureAwait(false);
            return true;
        }

        await HandleInternalServerError(httpContext, exception).ConfigureAwait(false);
        return true;
    }

    private async Task HandleValidationException(HttpContext httpContext, Exception ex)
    {
        var exception = (ValidationException)ex;

        const int code = StatusCodes.Status400BadRequest;
        var result = JsonApiExtensions.ToJsonApi(
            new object(),
            new Status { Code = code, Desc = exception.Errors });

        httpContext.Response.ContentType = HeaderConstants.Json;
        httpContext.Response.StatusCode = code;

        await httpContext
            .Response
            .WriteAsync(JsonConvert.SerializeObject(result, _jsonSettings))
            .ConfigureAwait(false);
    }

    private async Task HandleBadRequestException(HttpContext httpContext, Exception ex)
    {
        var exception = (BadRequestException)ex;

        const int code = StatusCodes.Status400BadRequest;
        var result = JsonApiExtensions.ToJsonApi(
            new object(),
            new Status { Code = code, Desc = exception.Message });

        httpContext.Response.ContentType = HeaderConstants.Json;
        httpContext.Response.StatusCode = code;

        await httpContext
            .Response
            .WriteAsync(JsonConvert.SerializeObject(result, _jsonSettings))
            .ConfigureAwait(false);
    }

    private async Task HandleNotFoundException(HttpContext httpContext, Exception ex)
    {
        var exception = (NotFoundException)ex;

        const int code = StatusCodes.Status404NotFound;
        var result = JsonApiExtensions.ToJsonApi(
            new object(),
            new Status { Code = code, Desc = exception.Message });

        httpContext.Response.ContentType = HeaderConstants.Json;
        httpContext.Response.StatusCode = code;

        await httpContext
            .Response
            .WriteAsync(JsonConvert.SerializeObject(result, _jsonSettings))
            .ConfigureAwait(false);
    }

    private async Task HandleUnauthorizedAccessException(HttpContext httpContext, Exception ex)
    {
        const int code = StatusCodes.Status401Unauthorized;
        var result = JsonApiExtensions.ToJsonApi(
            new object(),
            new Status { Code = code, Desc = "Unauthorize" });

        httpContext.Response.ContentType = HeaderConstants.Json;
        httpContext.Response.StatusCode = code;

        await httpContext
            .Response
            .WriteAsync(JsonConvert.SerializeObject(result, _jsonSettings))
            .ConfigureAwait(false);
    }

    private async Task HandleForbiddenAccessException(HttpContext httpContext, Exception ex)
    {
        const int code = StatusCodes.Status403Forbidden;
        var result = JsonApiExtensions.ToJsonApi(
            new object(),
            new Status { Code = code, Desc = "Forbidden" });

        httpContext.Response.ContentType = HeaderConstants.Json;
        httpContext.Response.StatusCode = code;

        await httpContext
            .Response
            .WriteAsync(JsonConvert.SerializeObject(result, _jsonSettings))
            .ConfigureAwait(false);
    }

    private async Task HandleServiceUnavailableException(HttpContext httpContext, Exception ex)
    {
        const int code = StatusCodes.Status503ServiceUnavailable;
        var result = JsonApiExtensions.ToJsonApi(
            new object(),
            new Status { Code = code, Desc = ex.Message });

        httpContext.Response.ContentType = HeaderConstants.Json;
        httpContext.Response.StatusCode = code;

        await httpContext
            .Response
            .WriteAsync(JsonConvert.SerializeObject(result, _jsonSettings))
            .ConfigureAwait(false);
    }

    private async Task HandleTooManyRequestsException(HttpContext httpContext, Exception ex)
    {
        const int code = StatusCodes.Status429TooManyRequests;
        var result = JsonApiExtensions.ToJsonApi(
            new object(),
            new Status { Code = code, Desc = ex.Message });

        httpContext.Response.ContentType = HeaderConstants.Json;
        httpContext.Response.StatusCode = code;

        await httpContext
            .Response
            .WriteAsync(JsonConvert.SerializeObject(result, _jsonSettings))
            .ConfigureAwait(false);
    }

    private async Task HandleInternalServerError(HttpContext httpContext, Exception ex)
    {
        const int code = StatusCodes.Status500InternalServerError;
        var result = JsonApiExtensions.ToJsonApi(
            new object(),
            new Status { Code = code, Desc = ex.Message });

        httpContext.Response.ContentType = HeaderConstants.Json;
        httpContext.Response.StatusCode = code;

        await httpContext
            .Response
            .WriteAsJsonAsync(JsonConvert.SerializeObject(result, _jsonSettings))
            .ConfigureAwait(false);

        _logger.LogError(ex, "Internal Server Error: {source} - {message}", ex.Source, ex.Message);
    }
}
