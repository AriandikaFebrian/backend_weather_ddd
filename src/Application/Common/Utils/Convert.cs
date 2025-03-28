// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;

namespace NetCa.Application.Common.Utils;

/// <summary>
/// Convert
/// </summary>
public static class Convert
{
    /// <summary>
    /// ConvertStringToArray
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static List<string> ConvertStringToArray(this string data)
    {
        return [.. data.Split(",")];
    }

    /// <summary>
    /// ConvertTimeStamp
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string ToTimeStamp(this DateTime date)
    {
        var timestamp = date.Ticks - DateTime.UnixEpoch.Ticks;
        timestamp /= TimeSpan.TicksPerSecond;
        return timestamp.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// ToDateTime
    /// </summary>
    /// <param name="stringValue"></param>
    /// <returns></returns>
    public static DateTime? ToDateTime(string stringValue)
    {
        DateTime.TryParse(stringValue, CultureInfo.InvariantCulture, out var value);
        return DateExtensions.IsMinDate(value) ? null : value;
    }

    /// <summary>
    /// ToInt
    /// </summary>
    /// <param name="stringValue"></param>
    /// <returns></returns>
    public static int ToInt(string stringValue)
    {
        int.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var value);
        return value;
    }

    /// <summary>
    /// ToFloat
    /// </summary>
    /// <param name="stringValue"></param>
    /// <returns></returns>
    public static float ToFloat(string stringValue)
    {
        float.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var value);
        return value;
    }

    /// <summary>
    /// ToDecimal
    /// </summary>
    /// <param name="stringValue"></param>
    /// <returns></returns>
    public static decimal ToDecimal(string stringValue)
    {
        decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var value);
        return value;
    }

    /// <summary>
    /// GenerateCode
    /// </summary>
    /// <param name="head"></param>
    /// <param name="lengthMin"></param>
    /// <param name="newValue"></param>
    /// <returns></returns>
    public static string GenerateCode(string head, int lengthMin = 11, int newValue = 1)
    {
        var length = head.Length + newValue.ToString(CultureInfo.InvariantCulture).Length;
        if (length <= lengthMin)
        {
            length = lengthMin;
        }

        var newCode = head.PadRight(length, '0');
        newCode = newCode.Remove(length - newValue.ToString(CultureInfo.InvariantCulture).Length);
        return $"{newCode}{newValue}";
    }
}
