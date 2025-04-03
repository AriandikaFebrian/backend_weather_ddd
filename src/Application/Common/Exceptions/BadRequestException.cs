// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace NetCa.Application.Common.Exceptions;

/// <summary>
/// BadRequestException
/// </summary>
[Serializable]
public class BadRequestException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class.
    /// </summary>
    /// <param name="message"></param>
    public BadRequestException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected BadRequestException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
