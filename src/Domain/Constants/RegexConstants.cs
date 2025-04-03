// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace NetCa.Domain.Constants;

/// <summary>
/// RegexConstants
/// </summary>
public abstract partial class RegexConstants
{
    /// <summary>
    /// RegexPattern
    /// </summary>
    public const string Pattern = @"^[{pattern}]+$";

    /// <summary>
    /// RegexChar
    /// </summary>
    public const string CharPattern = "A-Za-z";

    /// <summary>
    /// RegexNumeric
    /// </summary>
    public const string NumericPattern = "0-9";

    /// <summary>
    /// RegexSymbol
    /// </summary>
    public const string SymbolPattern = @"\&()@_:;/.-";

    /// <summary>
    /// Match a character in the set [^0-9A-Za-z].
    /// </summary>
    [GeneratedRegex(@"[^0-9A-Za-z]")]
    public static partial Regex CharNumeric();

    /// <summary>
    /// Match a character in the set [^0-9a-z].
    /// </summary>
    [GeneratedRegex(@"[^0-9a-z]")]
    public static partial Regex LowerCharNumeric();

    /// <summary>
    /// Match a character in the set [^-0-9A-Za-z].
    /// </summary>
    [GeneratedRegex(@"[^0-9A-Za-z_]")]
    public static partial Regex CharNumericUnderscore();

    /// <summary>
    /// Match a character in the set [^ -0-9A-Za-z].
    /// </summary>
    [GeneratedRegex(@"[^0-9A-Za-z _]")]
    public static partial Regex CharNumericUnderscoreSpace();
}
