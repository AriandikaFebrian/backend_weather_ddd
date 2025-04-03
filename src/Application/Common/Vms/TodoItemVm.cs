// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Globalization;
using NetCa.Application.Common.Mappings;
using NetCa.Domain.Common;

namespace NetCa.Application.Common.Vms;

/// <summary>
/// TodoItemVm
/// </summary>
public record TodoItemVm : BaseAuditableEntity, IMapFrom<TodoItem>
{
    /// <summary>
    /// /// Gets or sets Title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets StartDate
    /// </summary>
    /// <example>2023-12-12</example>
    public string StartDate { get; set; }

    /// <summary>
    /// Mapping
    /// </summary>
    /// <param name="profile"></param>
    public void Mapping(Profile profile)
    {
        profile
            .CreateMap<TodoItem, TodoItemVm>()
            .ForMember(
                dest => dest.StartDate,
                conf => conf.MapFrom(src => src.StartDate.ToString(new DateTimeFormatInfo()))
            );
    }
}
