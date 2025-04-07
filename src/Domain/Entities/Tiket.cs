using NetCa.Domain.Common;

namespace NetCa.Domain.Entities;

public record Tiket : BaseAuditableEntity
{
    public Tiket()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; init; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Status { get; set; } = "Open";
    public string Priority { get; set; } = "Medium";
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? Category { get; set; }
    public string? AttachmentUrl { get; set; }
}
