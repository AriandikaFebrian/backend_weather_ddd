// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace NetCa.Domain.Constants;

/// <summary>
/// FilterAndSortSeparatorConstants
/// </summary>
public abstract partial class FilterAndSortSeparatorConstants
{
    /// <summary>
    /// Match a character in the set [^-0-9A-Za-z].
    /// </summary>
    [GeneratedRegex(@"(?<!($|[^\\])(\\\\)*?\\),")]
    public static partial Regex EscapedCommaPattern();

    /// <summary>
    /// <summary>
    /// Match a character in the set [^-0-9A-Za-z].
    /// </summary>
    [GeneratedRegex(@"(?<!($|[^\\])(\\\\)*?\\)\|")]
    public static partial Regex EscapedPipePattern();

    /// <summary>
    /// Comma Separator
    /// </summary>
    public static readonly string[] Operators =
    {
        "!@=*",
        "!_=*",
        "!=*",
        "!@=",
        "!_=",
        "==*",
        "@=*",
        "_=*",
        "==",
        "!=",
        ">=",
        "<=",
        ">",
        "<",
        "@=",
        "_=",
        "="
    };
}
