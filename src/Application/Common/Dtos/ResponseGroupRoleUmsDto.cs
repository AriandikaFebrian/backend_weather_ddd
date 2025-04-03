// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace NetCa.Application.Common.Dtos;

#pragma warning disable

public record ResponseGroupRoleUmsVm
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<ResponseGroupRoleUms> ResponseGroups { get; set; }
}

public record ResponseGroupRoleUms
{
    public string Name { get; set; }
    public List<string> ItemList { get; set; }
    public ResponseGroupRoleUmsDto Response { get; set; }
}

public record ResponseGroupRoleUmsDto
{
    public string Request { get; set; }
    public string Status { get; set; }
}
