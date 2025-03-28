// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Dynamic;

namespace NetCa.Application.Common.Extensions;

/// <summary>
/// ClassExtensions
/// </summary>
public static class DynamicExtensions
{
    /// <summary>
    /// AddProperty
    /// </summary>
    /// <param name="expando"></param>
    /// <param name="propertyName"></param>
    /// <param name="propertyValue"></param>
    /// <param name="replace"></param>
    public static void AddProperty(
        ExpandoObject expando, string propertyName, object propertyValue, bool replace = true)
    {
        var exDict = expando as IDictionary<string, object>;
        if (exDict.ContainsKey(propertyName))
        {
            if (replace)
            {
                exDict[propertyName] = propertyValue;
            }
        }
        else
        {
            exDict.Add(propertyName, propertyValue);
        }
    }
}
