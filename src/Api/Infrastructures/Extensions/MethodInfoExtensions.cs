// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Reflection;

namespace NetCa.Api.Infrastructures.Extensions;

/// <summary>
/// MethodInfoExtensions
/// </summary>
public static class MethodInfoExtensions
{
    /// <summary>
    /// IsAnonymous
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool IsAnonymous(this MethodInfo method)
    {
        var invalidChars = new[] { '<', '>' };
        return method.Name.Any(invalidChars.Contains);
    }

    /// <summary>
    /// AnonymousMethod
    /// </summary>
    /// <param name="guardClause"></param>
    /// <param name="input"></param>
    public static void AnonymousMethod(this IGuardClause guardClause, Delegate input)
    {
        if (input.Method.IsAnonymous())
        {
            throw new ArgumentException("The endpoint name must be specified when using anonymous handlers.");
        }
    }
}
