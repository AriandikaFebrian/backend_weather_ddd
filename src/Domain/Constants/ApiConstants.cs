// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Domain.Constants;

/// <summary>
/// ApiConstants
/// </summary>
public abstract class ApiConstants
{
    /// <summary>
    /// ApiErrorDescription
    /// </summary>
    public readonly record struct ApiErrorDescription
    {
        /// <summary>
        /// BadRequest
        /// </summary>
        public const string BadRequest = "BadRequest";

        /// <summary>
        /// Unauthorized
        /// </summary>
        public const string Unauthorized = "Unauthorized";

        /// <summary>
        /// Forbidden
        /// </summary>
        public const string Forbidden = "Forbidden";

        /// <summary>
        /// InternalServerError
        /// </summary>
        public const string InternalServerError = "InternalServerError";
    }

    /// <summary>
    /// Weather-related constants
    /// </summary>
    public readonly record struct WeatherConstants
    {
        /// <summary>
        /// InvalidTemperature
        /// </summary>
        public const string InvalidTemperature = "Temperature value is invalid.";

        /// <summary>
        /// MissingSummary
        /// </summary>
        public const string MissingSummary = "Weather summary is required.";

        /// <summary>
        /// InvalidDateFormat
        /// </summary>
        public const string InvalidDateFormat = "Date format is invalid. Expected format: 'YYYY-MM-DD'.";
    }

    public readonly record struct TiketConstants
{
    public const string StatusOpen = "Open";
    public const string StatusInProgress = "InProgress";
    public const string StatusClosed = "Closed";
    public const string StatusPending = "Pending";

    public const string PriorityLow = "Low";
    public const string PriorityMedium = "Medium";
    public const string PriorityHigh = "High";
    public const string PriorityUrgent = "Urgent";

    public const string CategoryBug = "Bug";
    public const string CategoryFeature = "Feature Request";
    public const string CategorySupport = "Support";
}

}
