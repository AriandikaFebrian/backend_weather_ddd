// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace NetCa.Api.Controllers;

/// <summary>
/// Base class for object controllers.
/// </summary>
[ApiController]
public abstract class  ApiControllerBase : ControllerBase
{
    private IMediator _mediator;

    /// <summary>
    /// Gets protected variable to encapsulate request/response and publishing interaction patterns.
    /// </summary>
    /// <returns></returns>
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}
