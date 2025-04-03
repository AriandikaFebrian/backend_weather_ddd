// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace NetCa.Application.Common.Dtos;

#pragma warning disable
public record ControllerListDto
{
    public Guid? Id { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Url { get; set; }
    public string Method { get; set; }
    public List<string> Groups { get; set; }
    public List<int> RoleLevel { get; set; }
}
