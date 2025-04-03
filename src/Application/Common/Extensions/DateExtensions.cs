// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Globalization;

namespace NetCa.Application.Common.Extensions;

/// <summary>
/// DateExtensions
/// </summary>
public static class DateExtensions
{
    /// <summary>
    /// IsDateFormat
    /// </summary>
    /// <param name="date"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static bool IsDateFormat(string date, string format)
    {
        return DateTime.TryParseExact(
            date,
            format,
            CultureInfo.CurrentCulture,
            DateTimeStyles.None,
            out _);
    }

    /// <summary>
    /// FormatDate
    /// </summary>
    /// <param name="date"></param>
    /// <param name="formatDate"></param>
    /// <param name="defaultDate"></param>
    /// <param name="addHour"></param>
    /// <returns></returns>
    public static DateTime? FormatDate(
        string date,
        string formatDate,
        DateTime defaultDate,
        int addHour)
    {
        var result = DateTime.TryParseExact(
            date,
            formatDate,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out defaultDate);
        return result ? defaultDate.AddHours(addHour) : defaultDate;
    }

    /// <summary>
    /// FormatDate
    /// </summary>
    /// <param name="date"></param>
    /// <param name="formatDate"></param>
    /// <param name="time"></param>
    /// <param name="formatTime"></param>
    /// <param name="addHour"></param>
    /// <returns></returns>
    public static DateTime? FormatDate(
        string date,
        string formatDate,
        string time,
        string formatTime,
        int addHour)
    {
        if (IsDateFormat(date, formatDate))
        {
            var result = DateTime.ParseExact(date, formatDate, CultureInfo.CurrentCulture);
            if (time.Length == 5)
            {
                time = "0" + time;
            }

            var timeSpanFormat = new TimeSpan(
                0,
                int.Parse(time[..2], CultureInfo.InvariantCulture),
                int.Parse(time.AsSpan(2, 2), CultureInfo.InvariantCulture),
                int.Parse(time.AsSpan(4, 2), CultureInfo.InvariantCulture));

            result += timeSpanFormat;
            return result.AddHours(addHour);
        }

        return null;
    }

    /// <summary>
    /// GetUnixUtcTimestamp
    /// </summary>
    /// <returns></returns>
    public static long GetUnixUtcTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// GetUnixLocalTimestamp
    /// </summary>
    /// <returns></returns>
    public static long GetUnixLocalTimestamp()
    {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// IsMinDate
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static bool IsMinDate(DateTime? dateTime) => dateTime == DateTime.MinValue;

    /// <summary>
    /// IsMaxDate
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static bool IsMaxDate(DateTime? dateTime) => dateTime == DateTime.MaxValue;
}
