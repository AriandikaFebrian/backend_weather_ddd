// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Http;

namespace NetCa.Application.Common.Extensions;

/// <summary>
/// File Extension
/// </summary>
public static class FileExtensions
{
    /// <summary>
    /// Save File from Base64
    /// </summary>
    /// <param name="value"></param>
    /// <param name="filePath"></param>
    public static void SaveBase64ToFile(string value, string filePath)
    {
        if (value.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            var commaIndex = value.IndexOf(',');
            if (commaIndex != -1)
            {
                value = value[(commaIndex + 1)..];
            }
        }

        var bytes = Convert.FromBase64String(value);

        System.IO.File.WriteAllBytes(filePath, bytes);
    }

    /// <summary>
    /// Save File from IFormFile
    /// </summary>
    /// <param name="value"></param>
    /// <param name="filePath"></param>
    public static void SaveFormFileToFile(IFormFile value, string filePath)
    {
        using var stream = System.IO.File.Create(filePath);

        value.CopyTo(stream);

        stream.Close();
    }
}
