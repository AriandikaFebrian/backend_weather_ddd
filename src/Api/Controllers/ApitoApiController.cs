// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace NetCa.Api.Controllers;

/// <summary>
/// Represents RESTful of ApitoApi
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/apitoapi")]
public class ApitoApiController : ApiControllerBase
{
}
