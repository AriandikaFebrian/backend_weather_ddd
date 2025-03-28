// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Globalization;

namespace NetCa.Application.Common.Dtos;

/// <summary>
/// TodoItemDto
/// </summary>
public class TodoItemDto
{
    /// <summary>
    /// Gets or sets Title
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
            .CreateMap<TodoItemDto, TodoItem>()
            .ForMember(
                dest => dest.StartDate,
                conf => conf.MapFrom(src => DateOnly.Parse(src.StartDate, new DateTimeFormatInfo()))
            );
    }
}
