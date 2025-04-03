// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace NetCa.Application.Common.Models;

/// <summary>
/// Result
/// </summary>
public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    /// <summary>
    /// Gets or sets a value indicating whether succeeded
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Gets or sets errors
    /// </summary>
    public string[] Errors { get; set; }

    /// <summary>
    /// Success
    /// </summary>
    /// <returns></returns>
    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    /// <summary>
    /// Failure
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }
}
