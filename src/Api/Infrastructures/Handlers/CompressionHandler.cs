// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using NetCa.Domain.Constants;

namespace NetCa.Api.Infrastructures.Handlers;

/// <summary>
/// CompressionHandler
/// </summary>
public static class CompressionHandler
{
    /// <summary>
    /// ApplyCompress
    /// </summary>
    /// <param name="services"></param>
    public static void ApplyCompress(IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            var mimeTypes = new[]
            {
                HeaderConstants.JsonVndApi,
                HeaderConstants.Pdf,
                HeaderConstants.TextPlain,
                HeaderConstants.ImageJpg,
                HeaderConstants.Json,
                HeaderConstants.OctetStream,
                HeaderConstants.ProblemJson,
                HeaderConstants.TextCsv,
                HeaderConstants.ExcelXls,
                HeaderConstants.ExcelXlsx
            };
            options.EnableForHttps = true;
            options.MimeTypes = mimeTypes;
            options.Providers.Add<GzipCompressionProvider>();
            options.Providers.Add<BrotliCompressionProvider>();
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });
        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });
    }
}

/// <summary>
/// AddCompressionHandlerExtension
/// </summary>
public static class AddCompressionHandlerExtension
{
    /// <summary>
    /// AddCompressionHandler
    /// </summary>
    /// <param name="services"></param>
    public static void AddCompressionHandler(this IServiceCollection services)
    {
        CompressionHandler.ApplyCompress(services);
    }
}
