// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using NetCa.Application.Common.Vms;
using NetCa.Application.TodoItems.Commands.CreateTodoItem;
using NetCa.Application.TodoItems.Queries.GetTodoItems;
using NetCa.Domain.Constants;
using NSwag.Annotations;

namespace NetCa.Api.Controllers;

/// <summary>
/// Represents RESTful of TodoItemsController
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/todoitems")]
[ServiceFilter(typeof(ApiAuthorizeFilterAttribute))]
public class TodoItemsController : ApiControllerBase
{
    /// <summary>
    /// Create New TodoItem
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Produces(HeaderConstants.Json)]
    [SwaggerResponse(
        HttpStatusCode.OK,
        typeof(Unit),
        Description = "Successfully to Create New TodoItem")]
    [SwaggerResponse(
        HttpStatusCode.BadRequest,
        typeof(Unit),
        Description = ApiConstants.ApiErrorDescription.BadRequest)]
    [SwaggerResponse(
        HttpStatusCode.Unauthorized,
        typeof(Unit),
        Description = ApiConstants.ApiErrorDescription.Unauthorized)]
    [SwaggerResponse(
        HttpStatusCode.Forbidden,
        typeof(Unit),
        Description = ApiConstants.ApiErrorDescription.Forbidden)]
    [SwaggerResponse(
        HttpStatusCode.InternalServerError,
        typeof(Unit),
        Description = ApiConstants.ApiErrorDescription.InternalServerError)]
    public async Task<ActionResult<Unit>> CreateTodoItemAsync(
        [FromBody] CreateTodoItemCommand command,
        CancellationToken cancellationToken)
    {
        return await Mediator.Send(command, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Get List TodoItem
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Produces(HeaderConstants.Json)]
    [SwaggerResponse(
        HttpStatusCode.OK,
        typeof(DocumentRootJson<List<TodoItemVm>>),
        Description = "Successfully to Get List TodoItem")]
    [SwaggerResponse(
        HttpStatusCode.BadRequest,
        typeof(Unit),
        Description = ApiConstants.ApiErrorDescription.BadRequest)]
    [SwaggerResponse(
        HttpStatusCode.Unauthorized,
        typeof(Unit),
        Description = ApiConstants.ApiErrorDescription.Unauthorized)]
    [SwaggerResponse(
        HttpStatusCode.Forbidden,
        typeof(Unit),
        Description = ApiConstants.ApiErrorDescription.Forbidden)]
    [SwaggerResponse(
        HttpStatusCode.InternalServerError,
        typeof(Unit),
        Description = ApiConstants.ApiErrorDescription.InternalServerError)]
    public async Task<ActionResult<DocumentRootJson<List<TodoItemVm>>>> GetTodoItemsAsync(
        [FromQuery] GetTodoItemsQuery query,
        CancellationToken cancellationToken)
    {
        return await Mediator.Send(query, cancellationToken).ConfigureAwait(false);
    }
}
