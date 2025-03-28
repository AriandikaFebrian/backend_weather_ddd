using System.Collections.Generic;
using NetCa.Application.Common.Vms;

namespace NetCa.Application.Tikets.Queries.GetTiket
{
    /// <summary>
    /// GetTiketQuery for fetching tickets with sorting and pagination.
    /// </summary>
    public record GetTiketQuery : QueryModel, IRequest<List<TiketVm>>
    {
        public string Sorts { get; set; } = "-reportedat";  // Default sort by "reportedat"
        public int PageNumber { get; set; } = 1;  // Default to page 1
        public int PageSize { get; set; } = 10;   // Default to 10 items per page
    }
}
