using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetCa.Application.Common.Vms;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetCa.Application.Tikets.Queries.GetTiket
{
    /// <summary>
    /// Handler for processing the GetTiketQuery and returning a paginated list of TiketVm.
    /// </summary>
    public class GetTiketQueryHandler : IRequestHandler<GetTiketQuery, List<TiketVm>>
    {
        private readonly IApplicationDbContext _context; // Database context to query the tickets
        private readonly IMapper _mapper; // AutoMapper to map entities to view models

        // Constructor to inject dependencies (context and mapper)
        public GetTiketQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Handle method to process GetTiketQuery
        /// </summary>
        /// <param name="request">The query request containing parameters like sorting</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>List of TiketVm (paginated)</returns>
        public async Task<List<TiketVm>> Handle(GetTiketQuery request, CancellationToken cancellationToken)
        {
            // Default sorting if not provided
            request.Sorts ??= "-reportedat"; // Default sort by "reportedat"

            // Default PageNumber to 1 if it's null, and PageSize to 10 if it's null
            int pageNumber = request.PageNumber;  // Default to page 1
            int pageSize = request.PageSize;      // Default to 10 items per page

            // Get the queryable Tikets
            var query = _context.Tikets.AsQueryable();

            // Apply dynamic sorting based on the Sorts string
            if (request.Sorts.StartsWith("-"))
            {
                var sortField = request.Sorts.Substring(1); // Remove the "-" for sorting direction
                query = sortField switch
                {
                    "reportedat" => query.OrderByDescending(t => t.ReportedAt),
                    "resolvedat" => query.OrderByDescending(t => t.ResolvedAt),
                    "title" => query.OrderByDescending(t => t.Title),
                    _ => query.OrderByDescending(t => t.ReportedAt) // Default fallback
                };
            }
            else
            {
                var sortField = request.Sorts; // Use full field name directly
                query = sortField switch
                {
                    "reportedat" => query.OrderBy(t => t.ReportedAt),
                    "resolvedat" => query.OrderBy(t => t.ResolvedAt),
                    "title" => query.OrderBy(t => t.Title),
                    _ => query.OrderBy(t => t.ReportedAt) // Default fallback
                };
            }

            // Apply pagination logic (Skip and Take)
            var tiketList = await query
                .Skip((pageNumber - 1) * pageSize) // Skip based on page number and page size
                .Take(pageSize) // Limit the number of items per page
                .ProjectTo<TiketVm>(_mapper.ConfigurationProvider) // AutoMapper projection to view model
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false); // ConfigureAwait to avoid deadlocks in libraries

            // Return only the list of tickets (without pagination metadata)
            return tiketList;
        }
    }
}
