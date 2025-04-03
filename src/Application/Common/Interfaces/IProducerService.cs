// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Application.Common.Interfaces;

/// <summary>
/// IProducerService
/// </summary>
public interface IProducerService
{
    /// <summary>
    /// SendAsync
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    Task<bool> SendAsync(string topic, string message);
}
