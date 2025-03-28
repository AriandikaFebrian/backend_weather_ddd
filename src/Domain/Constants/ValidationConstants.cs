// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Domain.Constants;

/// <summary>
/// ValidationConstants
/// </summary>
public abstract class ValidationConstants
{
    /// <summary>
    /// Maximum Length Base64 in byte
    /// </summary>
    public const int MaximumLengthBase64 = 512 * 1024;

    /// <summary>
    /// Maximum File Size in byte
    /// </summary>
    public const int MaximumFileSize = 3072 * 1024;

    /// <summary>
    /// Maximum Body Size in byte
    /// </summary>
    public const int MaximumBodySize = 10240 * 1024;

    /// <summary>
    /// Image File Extensions
    /// </summary>
    public static readonly IReadOnlyList<string> ImageFileExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".tiff",
        ".heif",
        ".bmp"
    ];

    /// <summary>
    /// Validation Message
    /// </summary>
    public static readonly ImmutableDictionary<string, string> Message =
        ImmutableDictionary.CreateRange(
            new[]
            {
                KeyValuePair.Create(
                    "CharNumericOnly",
                    "'{PropertyName}' must contain only characters and numbers"
                ),
                KeyValuePair.Create(
                    "CharNumericHyphenSpaceOnly",
                    "'{PropertyName}' must contain only characters, numbers, space and hyphen symbol"
                ),
                KeyValuePair.Create(
                    "IsDuplicate",
                    "'{PropertyName}' has duplicate value"
                ),
                KeyValuePair.Create(
                    "IsValidUri",
                    "'{PropertyName}' is not a valid Uri"
                ),
                KeyValuePair.Create(
                    "IsValidGuid",
                    "'{PropertyName}' is not a valid Guid"
                ),
                KeyValuePair.Create(
                    "MustValidFileExtension",
                    "File is not in the correct format for image files"
                ),
                KeyValuePair.Create(
                    "MustLessMaximumFileSize",
                    "File exceeds the file size limit"
                ),
            }
        );
}
