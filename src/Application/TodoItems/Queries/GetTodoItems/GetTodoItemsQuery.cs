// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using NetCa.Application.Common.Mappings;
using NetCa.Application.Common.Vms;

namespace NetCa.Application.TodoItems.Queries.GetTodoItems;

/// <summary>
/// GetTodoItemsQuery
/// </summary>
public record GetTodoItemsQuery : QueryModel, IRequest<DocumentRootJson<List<TodoItemVm>>> { }

/// <summary>
/// GetTodoItemsQueryHandler
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GetTodoItemsQueryHandler"/> class.
/// </remarks>
/// <param name="_context"></param>
/// <param name="_mapper"></param>
public class GetTodoItemsQueryHandler(IApplicationDbContext _context, IMapper _mapper)
    : IRequestHandler<GetTodoItemsQuery, DocumentRootJson<List<TodoItemVm>>>
{
    /// <summary>
    /// Handle
    /// /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<DocumentRootJson<List<TodoItemVm>>> Handle(
        GetTodoItemsQuery request,
        CancellationToken cancellationToken
    )
    {
        request.Sorts ??= "-updatedate";

        return await _context
            .TodoItems
            .ProjectTo<TodoItemVm>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request)
            .ConfigureAwait(false);
    }
}
