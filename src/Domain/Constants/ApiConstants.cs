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
        public static readonly string StatusOpen = "Open";
        public static readonly string StatusInProgress = "In Progress";
        public static readonly string StatusResolved = "Resolved";
        public static readonly string StatusClosed = "Closed";

        public static readonly string PriorityLow = "Low";
        public static readonly string PriorityMedium = "Medium";
        public static readonly string PriorityHigh = "High";

        public static readonly string SeverityCritical = "Critical";
        public static readonly string SeverityMajor = "Major>";
        public static readonly string SeverityMinor = "Minor";

        public static readonly string DivisionIT = "IT";
        public static readonly string DivisionQA = "QA";
        public static readonly string DivisionDevlopment = "Devlopment";
        public static readonly string DivisionCustomerSupport = "Customer Support";

        public static readonly string EnvironmentProduction = "Production";
        public static readonly string EnvironmentStaging = "Staging";
        public static readonly string EnvironmentDevelopment = "Development";

    }

    public readonly record struct CryptoConstants
    {
        public const string SymbolBTS = "BTC";
        public const string SymbolETH = "ETH";
        public const string SymbolBNB = "BNB";

        public const string CurrentUSD = "USD";
        public const string CurrentIDR = "IDR";
        public const string CurrentEUR = "EUR";

        public const string MarketTypeSpot = "Spot";
        public const string MarketTypeFutures = "Futures";

        public const string PriceInvalid = "Price value is invalid.";
        public const string SymbolRequired = "Symbol is required.";
        public const string MarketCapInvalid = "Market cap value is invalid.";
        public const string VolumeInvalid = "Volume value is invalid.";
        public const string ChangeInvalid = "Percentage change value is invalid.";
    }
}
