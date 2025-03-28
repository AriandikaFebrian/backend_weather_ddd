// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using NetCa.Api.Controllers;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace NetCa.Api.Infrastructures.Processors;

/// <summary>
/// MyControllerProcessor
/// </summary>
public class MyControllerProcessor : IOperationProcessor
{
    /// <summary>
    /// Process
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public bool Process(OperationProcessorContext context)
    {
        return context.ControllerType != typeof(DevelopmentController);
    }
}
