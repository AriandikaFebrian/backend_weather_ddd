using DocumentFormat.OpenXml.InkML;

namespace NetCa.Application.Tikets.Commands
{
    /// <summary>
    /// CreateTiketCommand
    /// </summary>
    public class CreateTiketCommand : IRequest<Unit>
    {

        public string Title { get; init; }
        public string Description { get; init; }
        public string ReportedByDivision { get; init; } // Division reporting the ticket
        public string Status { get; init; } // Ticket Status (Open, In Progress, Closed)
        public string Priority { get; init; } // Priority level (Low, Medium, High)
        public string Severity { get; init; } // Severity (Critical, Major, Minor)
        public DateTime ReportedAt { get; init; } // When the issue was reported
        public DateTime? ResolvedAt { get; init; } // When the issue was resolved
        public string Resolution { get; init; } // Resolution or fix applied
        public string AssignedTo { get; init; } // Who the ticket is assigned to (if applicable)
        public string AssignedDivision { get; init; } // Which division is handling the ticket
        public string Notes { get; init; } // Additional notes or comments
        public string AffectedVersion { get; init; } // Version the bug was found in
        public string Environment { get; init; } // Environment (Production, Staging, Development)
        public string AttachmentUrl { get; init; } // Link to attached files or screenshots
    }

    /// <summary>
    /// Handler for creating a new ticket
    /// </summary>
    public class CreateTiketCommandHandler : IRequestHandler<CreateTiketCommand, Unit>
    {
        private readonly IApplicationDbContext _context; // Repository to save Tiket

        // Constructor to inject the repository
        public CreateTiketCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        // Handle method to process the CreateTiketCommand
        public async Task<Unit> Handle(CreateTiketCommand request, CancellationToken cancellationToken)
        {
            // Validation logic (keeping the structure as you want)
            if (string.IsNullOrEmpty(request.Title))
                throw new ArgumentException("Title is required.");

            if (string.IsNullOrEmpty(request.Description))
                throw new ArgumentException("Description is required.");

            if (string.IsNullOrEmpty(request.Status) || !new[] { "Open", "In Progress", "Resolved", "Closed" }.Contains(request.Status))
                throw new ArgumentException("Invalid status value.");

            if (string.IsNullOrEmpty(request.Priority) || !new[] { "Low", "Medium", "High" }.Contains(request.Priority))
                throw new ArgumentException("Invalid priority value.");

            if (string.IsNullOrEmpty(request.Severity) || !new[] { "Critical", "Major", "Minor" }.Contains(request.Severity))
                throw new ArgumentException("Invalid severity value.");

            // Map CreateTiketCommand to Tiket entity
            var tiket = new Tiket
            {
                Title = request.Title,
                Description = request.Description,
                ReportedByDivision = request.ReportedByDivision,
                Status = request.Status,
                Priority = request.Priority,
                Severity = request.Severity,
                ReportedAt = request.ReportedAt,
                ResolvedAt = request.ResolvedAt,
                Resolution = request.Resolution,
                AssignedTo = request.AssignedTo,
                AssignedDivision = request.AssignedDivision,
                Notes = request.Notes,
                AffectedVersion = request.AffectedVersion,
                Environment = request.Environment,
                AttachmentUrl = request.AttachmentUrl
            };

            try
            {
                // Save the ticket to the repository (assuming asynchronous saving)
                _context.Tikets.Add(tiket);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "Error saving ticket");

                // Rethrow or handle specific exceptions (e.g., database connection issues)
                throw new ApplicationException("An error occurred while saving the ticket.", ex);
            }

            // Return Unit to indicate completion
            return Unit.Value;
        }
    }

    // Validator Class (kept as a placeholder in the same structure)
    public class CreateTiketCommandValidator : AbstractValidator<CreateTiketCommand>
    {
        public CreateTiketCommandValidator()
        {
            // Title should not be empty and should be a valid length
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(5, 100).WithMessage("Title should be between 5 and 100 characters.");

            // Description should not be empty and should be a valid length
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .Length(10, 500).WithMessage("Description should be between 10 and 500 characters.");

            // Status should be one of the predefined status values
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(status => status == "Open" || status == "In Progress" || status == "Resolved" || status == "Closed")
                .WithMessage("Invalid status value.");

            // Priority should be one of the predefined priority values
            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Priority is required.")
                .Must(priority => priority == "Low" || priority == "Medium" || priority == "High")
                .WithMessage("Invalid priority value.");

            // Severity should be one of the predefined severity values
            RuleFor(x => x.Severity)
                .NotEmpty().WithMessage("Severity is required.")
                .Must(severity => severity == "Critical" || severity == "Major" || severity == "Minor")
                .WithMessage("Invalid severity value.");
        }
    }
}
