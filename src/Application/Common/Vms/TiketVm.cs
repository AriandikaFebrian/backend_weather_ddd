using System.Globalization;
using NetCa.Application.Common.Mappings;

namespace NetCa.Application.Common.Vms
{
    public record TiketVm : IMapFrom<Tiket>
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public string ReportedByDivision { get; init; }
        public string Status { get; init; }
        public string Priority { get; init; }
        public string Severity { get; init; }
        public string ReportedAt { get; init; }  // This will be a string formatted date
        public string ResolvedAt { get; init; }  // This could be null
        public string Resolution { get; init; }
        public string AssignedTo { get; init; }
        public string AssignedDivision { get; init; }
        public string Notes { get; init; }
        public string AffectedVersion { get; init; }
        public string Environment { get; init; }
        public string AttachmentUrl { get; init; }

        // Implement the mapping logic from Tiket entity
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Tiket, TiketVm>()
                .ForMember(dest => dest.ReportedAt, 
                    opt => opt.MapFrom(src => src.ReportedAt.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture))) // Format the date
                .ForMember(dest => dest.ResolvedAt,
                    opt => opt.MapFrom(src => src.ResolvedAt.HasValue ? src.ResolvedAt.Value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) : null)) // Handle nullability manually
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? "N/A"))  // Handle potential null values
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority ?? "N/A"))
                .ForMember(dest => dest.Severity, opt => opt.MapFrom(src => src.Severity ?? "N/A"));
        }
    }
}
