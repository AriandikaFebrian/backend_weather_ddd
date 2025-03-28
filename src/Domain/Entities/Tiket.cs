using NetCa.Domain.Common;

namespace NetCa.Domain.Entities;

public record Tiket : BaseAuditableEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string ReportedByDivision { get; set; } // Division reporting the ticket
    public string Status { get; set; } // Ticket Status (Open, In Progress, Closed)
    public string Priority { get; set; } // Priority level (Low, Medium, High)
    public string Severity { get; set; } // Severity (Critical, Major, Minor)
    public DateTime ReportedAt { get; set; } // When the issue was reported
    public DateTime? ResolvedAt { get; set; } // When the issue was resolved
    public string Resolution { get; set; } // Resolution or fix applied
    public string AssignedTo { get; set; } // Who the ticket is assigned to (if applicable)
    public string AssignedDivision { get; set; } // Which division is handling the ticket
    public string Notes { get; set; } // Additional notes or comments
    public string AffectedVersion { get; set; } // Version the bug was found in
    public string Environment { get; set; } // Environment (Production, Staging, Development)
    public string AttachmentUrl { get; set; } // Link to attached files or screenshots
}
