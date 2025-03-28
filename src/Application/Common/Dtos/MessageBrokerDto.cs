// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Application.Common.Dtos;

#pragma warning disable
public record MessageBrokerDto
{
    public string Name { get; set; }
    public string Value { get; set; }
}
