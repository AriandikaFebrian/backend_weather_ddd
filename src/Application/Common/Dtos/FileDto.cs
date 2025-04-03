// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Application.Common.Dtos;

/// <summary>
/// FileDto
/// </summary>
public record FileDto
{
    /// <summary>
    /// Gets or sets FileName
    /// </summary>
    /// <value></value>
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets Value
    /// </summary>
    /// <value></value>
    public string Value { get; set; }
}

/// <summary>
/// FileResponseDto
/// </summary>
public record FileResponseDto
{
    /// <summary>
    /// Gets or sets FileName
    /// </summary>
    /// <value></value>
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets ContentType
    /// </summary>
    /// <value></value>
    public string ContentType { get; set; }

    /// <summary>
    /// Gets or sets Value
    /// </summary>
    /// <value></value>
    public byte[] Value { get; set; }
}
