// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Domain.Constants;

/// <summary>
/// ProcessConstants
/// </summary>
public abstract class ProcessConstants
{
    /// <summary>
    /// DefaultTotalMaxProcess
    /// </summary>
    public const byte DefaultTotalMaxProcess = 25;

    /// <summary>
    /// File Extensions
    /// </summary>
    public static readonly IReadOnlyList<string> FileExtensions = new List<string>
    {
        "csv",
        "xls",
        "xlsx"
    };
}
