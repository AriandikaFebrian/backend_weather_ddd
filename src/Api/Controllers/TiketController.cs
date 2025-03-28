using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCa.Application.Common.Interfaces;
using NetCa.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetCa.Api.Controllers
{
    /// <summary>
    /// API Controller for managing Tiket (Ticket) data.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/tickets")]
    [ApiController]
    public class TiketController : ControllerBase
    {
        private readonly IApplicationDbContext _context;

        /// <summary>
        /// Constructor to inject dependencies.
        /// </summary>
        /// <param name="context">Database context for ticket data.</param>
        public TiketController(IApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all ticket records from the database.
        /// </summary>
        /// <param name="cancellationToken">Token for canceling the async operation.</param>
        /// <returns>A list of ticket records.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTicketsAsync(CancellationToken cancellationToken)
        {
            var ticketList = await _context.Tikets.ToListAsync(cancellationToken);
            return Ok(ticketList);
        }

        /// <summary>
        /// Retrieves a specific ticket record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the ticket record.</param>
        /// <param name="cancellationToken">Token for canceling the async operation.</param>
        /// <returns>The requested ticket record.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var ticket = await _context.Tikets.FindAsync(new object[] { id }, cancellationToken);
            if (ticket == null)
            {
                return NotFound("Ticket not found");
            }
            return Ok(ticket);
        }

        /// <summary>
        /// Creates a new ticket record.
        /// </summary>
        /// <param name="ticket">The ticket data to create.</param>
        /// <param name="cancellationToken">Token for canceling the async operation.</param>
        /// <returns>The created ticket record.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateTicketAsync([FromBody] Tiket ticket, CancellationToken cancellationToken)
        {
            if (ticket == null)
            {
                return BadRequest("Ticket data is required.");
            }

            try
            {
                // Ensure ReportedAt is set before adding to the database
                ticket.ReportedAt = DateTime.UtcNow;

                // Add the ticket data to the database context
                _context.Tikets.Add(ticket);

                // Save changes to the database
                await _context.SaveChangesAsync(cancellationToken);

                // Return the created ticket data with 201 status
                return CreatedAtAction(nameof(GetTicketByIdAsync), new { id = ticket.Title }, ticket);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing ticket record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the ticket record.</param>
        /// <param name="ticketUpdate">The updated ticket data.</param>
        /// <param name="cancellationToken">Token for canceling the async operation.</param>
        /// <returns>The updated ticket record.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicketAsync(Guid id, [FromBody] Tiket ticketUpdate, CancellationToken cancellationToken)
        {
            var existingTicket = await _context.Tikets.FindAsync(new object[] { id }, cancellationToken);
            if (existingTicket == null)
            {
                return NotFound("Ticket not found");
            }

            // Update the ticket fields
            existingTicket.Title = ticketUpdate.Title;
            existingTicket.Description = ticketUpdate.Description;
            existingTicket.ReportedByDivision = ticketUpdate.ReportedByDivision;
            existingTicket.Status = ticketUpdate.Status;
            existingTicket.Priority = ticketUpdate.Priority;
            existingTicket.Severity = ticketUpdate.Severity;
            existingTicket.ResolvedAt = ticketUpdate.ResolvedAt;
            existingTicket.Resolution = ticketUpdate.Resolution;
            existingTicket.AssignedTo = ticketUpdate.AssignedTo;
            existingTicket.AssignedDivision = ticketUpdate.AssignedDivision;
            existingTicket.Notes = ticketUpdate.Notes;
            existingTicket.AffectedVersion = ticketUpdate.AffectedVersion;
            existingTicket.Environment = ticketUpdate.Environment;
            existingTicket.AttachmentUrl = ticketUpdate.AttachmentUrl;

            // Save changes to the database
            await _context.SaveChangesAsync(cancellationToken);
            return Ok(existingTicket);
        }

        /// <summary>
        /// Deletes a ticket record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the ticket record.</param>
        /// <param name="cancellationToken">Token for canceling the async operation.</param>
        /// <returns>No content if deletion is successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicketAsync(Guid id, CancellationToken cancellationToken)
        {
            var ticket = await _context.Tikets.FindAsync(new object[] { id }, cancellationToken);
            if (ticket == null)
            {
                return NotFound("Ticket not found");
            }

            _context.Tikets.Remove(ticket);
            await _context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        [HttpGet("monthly-report")]
public async Task<IActionResult> GetMonthlyReportAsync(int month, int year, CancellationToken cancellationToken)
{
    // Query tiket yang selesai pada bulan dan tahun tertentu
    var ticketsCompleted = await _context.Tikets
        .Where(t => t.Status == "Closed" && t.ResolvedAt.HasValue && t.ResolvedAt.Value.Month == month && t.ResolvedAt.Value.Year == year)
        .ToListAsync(cancellationToken);

    var completedTicketsCount = ticketsCompleted.Count();

    // Bisa tambah laporan lainnya, misalnya, divisi yang paling banyak menangani tiket
    var divisionStats = ticketsCompleted
        .GroupBy(t => t.AssignedDivision)
        .Select(group => new
        {
            Division = group.Key,
            TicketCount = group.Count()
        })
        .ToList();

    return Ok(new
    {
        CompletedTickets = completedTicketsCount,
        DivisionStats = divisionStats
    });
}

[HttpGet("problem-report")]
public async Task<IActionResult> GetProblemReportAsync(CancellationToken cancellationToken)
{
    // Query tiket berdasarkan judul atau masalah yang sering disebutkan
    var problemStats = await _context.Tikets
        .GroupBy(t => t.Title) // Atau bisa diganti dengan t.Description atau field lain yang menggambarkan masalah
        .Select(group => new
        {
            Problem = group.Key, // Ini adalah judul masalah atau masalah yang sering dilaporkan
            TicketCount = group.Count() // Menghitung jumlah tiket dengan masalah yang sama
        })
        .OrderByDescending(x => x.TicketCount) // Mengurutkan berdasarkan jumlah tiket
        .ToListAsync(cancellationToken);

    // Mengembalikan daftar masalah yang sering dilaporkan beserta jumlah tiketnya dalam objek JSON
    return Ok(new
    {
        ProblemReport = problemStats
    });
}
    }

}
