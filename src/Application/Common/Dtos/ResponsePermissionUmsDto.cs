// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace NetCa.Application.Common.Dtos;

#pragma warning disable
public record ResponsePermissionUmsVm
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<ResponsePermissionUmsDto> ResponsePermissionDtos { get; set; }
}

public record ResponsePermissionUmsDto
{
    public string Name { get; set; }
    public string Status { get; set; }
}
