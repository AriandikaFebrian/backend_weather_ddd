// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace NetCa.Application.Common.Exceptions;

/// <summary>
/// ThrowException
/// </summary>
[Serializable]
public class ThrowException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThrowException"/> class.
    /// </summary>
    /// <param name="message"></param>
    public ThrowException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThrowException"/> class.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected ThrowException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
