// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NetCa.Application.Common.Models;

/// <summary>
/// CustomValidator
/// </summary>
public static class CustomValidator
{
    /// <summary>
    /// IsValidName
    /// </summary>
    /// <param name="methodName"></param>
    /// <returns></returns>
    internal static Func<string, bool> IsValidName(string methodName)
    {
        var method = typeof(RegexConstants).GetMethod(methodName);

        return (x) => !((Regex)method.Invoke(null, [])).IsMatch(x);
    }

    /// <summary>
    /// IsValidGuid
    /// </summary>
    /// <param name="notEmpty"></param>
    /// <returns></returns>
    internal static Func<string, bool> IsValidGuid(bool notEmpty = false)
    {
        return (x) =>
        {
            if (x == null)
            {
                return !notEmpty;
            }

            try
            {
                return !string.IsNullOrEmpty(x) && Guid.TryParse(x, out _);
            }
            catch
            {
                return false;
            }
        };
    }

    /// <summary>
    /// IsDateNotNull
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    internal static bool IsDateNotNull(DateTime? arg)
    {
        return arg is not null;
    }

    internal static bool IsValidDate(DateTime? arg)
    {
        return !arg.Equals(default(DateTime));
    }

    /// <summary>
    /// IsValidUri
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    internal static bool IsValidUri(string arg)
    {
        return !string.IsNullOrWhiteSpace(arg)
            && Uri.TryCreate(arg, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// MaximumLengthBase64
    /// </summary>
    /// <param name="arg"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    internal static bool MaximumLengthBase64(
        string arg,
        int maxLength = ValidationConstants.MaximumLengthBase64
    )
    {
        var length = Encoding.UTF8.GetByteCount(arg);

        return length <= maxLength;
    }

    /// <summary>
    /// MaximumFileSize
    /// </summary>
    /// <param name="size"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    internal static bool MaximumFileSize(
        long size,
        int maxLength = ValidationConstants.MaximumFileSize
    )
    {
        return size <= maxLength;
    }

    /// <summary>
    /// IsValidFileName
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="fileNameList"></param>
    /// <returns></returns>
    internal static bool IsValidFileName(string fileName, List<string> fileNameList)
    {
        return fileNameList.Contains(fileName);
    }

    /// <summary>
    /// IsValidFileType
    /// </summary>
    /// <param name="fileType"></param>
    /// <param name="fileTypeList"></param>
    /// <returns></returns>
    internal static bool IsValidFileType(string fileType, List<string> fileTypeList)
    {
        return fileTypeList.Contains(fileType);
    }

    /// <summary>
    /// IsValidPattern
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static bool IsValidPattern(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        var combinePattern = RegexConstants
            .Pattern
            .Replace(
                "{pattern}",
                RegexConstants.CharPattern
                    + RegexConstants.NumericPattern
                    + " "
                    + RegexConstants.SymbolPattern
            );

        var pattern = new Regex(combinePattern);
        var isMatch = pattern.IsMatch(value);

        return isMatch;
    }

    /// <summary>
    /// IsDuplicate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static bool IsDuplicate<T>(List<T> value)
    {
        var data = new HashSet<T>();
        return value.TrueForAll(data.Add);
    }

    /// <summary>
    /// In
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <param name="validOptions"></param>
    /// <returns></returns>
    internal static IRuleBuilderOptions<T, TProperty> In<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder,
        params TProperty[] validOptions
    )
    {
        string formatted;

        if (validOptions == null || validOptions.Length == 0)
        {
            throw new ArgumentException(
                "At least one valid option is expected",
                nameof(validOptions)
            );
        }
        else if (validOptions.Length == 1)
        {
            formatted = validOptions[0].ToString();
        }
        else
        {
            // format like: option1, option2 or option3
            formatted =
                $"{string.Join(", ", validOptions.Select(vo => vo.ToString()).ToArray(), 0, validOptions.Length - 1)} or {validOptions[^1]}";
        }

        return ruleBuilder
            .Must(validOptions.Contains)
            .WithMessage($"{{PropertyName}} must be one of these values: {formatted}");
    }
}
