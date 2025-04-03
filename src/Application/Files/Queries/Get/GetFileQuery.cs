// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.AspNetCore.StaticFiles;

namespace NetCa.Application.Files.Queries.Get;

/// <summary>
/// GetFileQuery
/// </summary>
public record GetFileQuery : IRequest<FileResponseDto>
{
    /// <summary>
    /// Gets or sets Path
    /// </summary>
    /// <value></value>
    public string Path { get; set; }

    /// <summary>
    /// Gets or sets FileName
    /// </summary>
    /// <value></value>
    public string FileName { get; set; }
}

/// <summary>
/// Handling GetFileQuery
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GetFileQueryHandler"/> class.
/// </remarks>
/// <param name="_appSetting">Set context to get application setting provider</param>
public class GetFileQueryHandler(AppSetting _appSetting)
    : IRequestHandler<GetFileQuery, FileResponseDto>
{
    /// <summary>
    /// Handle Get File
    /// </summary>
    /// <param name="request">
    /// The encapsulated request body
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to perform cancel the operation
    /// </param>
    /// <returns>Response</returns>
    public async Task<FileResponseDto> Handle(
        GetFileQuery request,
        CancellationToken cancellationToken
    )
    {
        var filePath = $"{_appSetting.Files.Path}/{request.Path}/{request.FileName}";

        if (!System.IO.File.Exists(filePath))
        {
            throw new BadRequestException("The file does not exist");
        }

        var provider = new FileExtensionContentTypeProvider();

        if (!provider.TryGetContentType(filePath, out var contentType))
        {
            contentType = HeaderConstants.OctetStream;
        }

        var value = System.IO.File.ReadAllBytes(filePath);

        var data = new FileResponseDto
        {
            FileName = request.FileName,
            ContentType = contentType,
            Value = value
        };

        return data;
    }
}

/// <summary>
/// GetFileQueryValidator
/// </summary>
public class GetFileQueryValidator : AbstractValidator<GetFileQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetFileQueryValidator"/> class.
    /// </summary>
    public GetFileQueryValidator()
    {
        RuleFor(x => x.FileName).NotEmpty();

        RuleFor(x => x.Path)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(250)
            .In(FileConstants.PathTypes.Application);
    }
}
