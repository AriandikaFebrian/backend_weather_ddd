using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using NetCa.Application.Common.Vms;
using NetCa.Application.Weathers.Commands.GetWeather;
using NetCa.Application.Weathers.Queries;
using NetCa.Domain.Constants;
using NSwag.Annotations;
using Microsoft.Extensions.Logging;

namespace NetCa.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("Api/v{version:apiVersion}/weathers")]
    [ServiceFilter(typeof(ApiAuthorizeFilterAttribute))]
    public class WeathersController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<WeathersController> _logger;

        public WeathersController(IMediator mediator, ILogger<WeathersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [Produces(HeaderConstants.Json)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(WeatherVm), Description = "Successfully created new weather record.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(string), Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(string), Description = "Internal Server Error")]
        public async Task<ActionResult<WeatherVm>> CreateWeatherAsync(
            [FromBody] CreateWeatherCommand command,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new weather record...");
            var result = await _mediator.Send(command, cancellationToken);
            _logger.LogInformation("Weather record created successfully.");
            return Ok(result);
        }

        // 📌 READ ALL: Ambil semua data cuaca dari database
[HttpGet]
[Produces("application/json")]
[SwaggerResponse(200, typeof(List<WeatherVm>), Description = "Successfully retrieved all weather data.")]
[SwaggerResponse(500, typeof(string), Description = "Internal Server Error")]
public async Task<ActionResult<List<WeatherVm>>> GetAllWeatherAsync(CancellationToken cancellationToken)
{
    _logger.LogInformation("Fetching all weather records...");
    try
    {
        var result = await _mediator.Send(new GetAllWeatherQuery(), cancellationToken); // Send the query to fetch all records
        if (result != null && result.Any())
        {
            _logger.LogInformation("Successfully retrieved weather records.");
            return Ok(result);
        }
        else
        {
            _logger.LogWarning("No weather records found.");
            return NotFound("No weather records found.");
        }
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error occurred while fetching all weather records: {ex.Message}");
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}

        [AllowAnonymous]
        [HttpGet("public")]
        [Produces("application/json")]
        [SwaggerResponse(200, typeof(List<WeatherVm>), Description = "Successfully retrieved public weather data.")]
        [SwaggerResponse(400, typeof(string), Description = "Bad Request")]
        [SwaggerResponse(500, typeof(string), Description = "Internal Server Error")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<List<WeatherVm>>> GetPublicWeatherAsync(
            [FromQuery] string city,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest("City parameter is required.");
            }

            // Normalize city to lowercase before using it
            var cityNormalized = city.Trim().ToLowerInvariant();

            var result = await _mediator.Send(new GetWeatherQuery(cityNormalized), cancellationToken);
            return Ok(result);
        }


        [HttpGet("{id}")]
        [Produces("application/json")]
        [SwaggerResponse(200, typeof(WeatherVm), Description = "Successfully retrieved weather data by ID.")]
        [SwaggerResponse(404, typeof(string), Description = "Weather record not found.")]
        [SwaggerResponse(500, typeof(string), Description = "Internal Server Error")]
        public async Task<ActionResult<WeatherVm>> GetWeatherByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Fetching weather record with ID: {id}");
            try
            {
                var result = await _mediator.Send(new GetWeatherByIdQuery(id), cancellationToken);
                if (result != null)
                {
                    _logger.LogInformation($"Successfully retrieved weather record with ID: {id}");
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning($"Weather record with ID {id} not found.");
                    return NotFound("Weather record not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while fetching weather record by ID: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Produces(HeaderConstants.Json)]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(string), Description = "Successfully deleted the weather record.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), Description = "Weather record not found.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(string), Description = "Internal Server Error")]
        public async Task<ActionResult> DeleteWeatherAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(new DeleteWeatherCommand(id), cancellationToken);

                if (result)
                {
                    return NoContent();
                }

                return NotFound("Weather record not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        [SwaggerResponse(200, typeof(WeatherVm), Description = "Successfully updated the weather record.")]
        [SwaggerResponse(404, typeof(string), Description = "Weather record not found.")]
        [SwaggerResponse(500, typeof(string), Description = "Internal Server Error")]
        public async Task<ActionResult<WeatherVm>> UpdateWeatherAsync(
    [FromRoute] Guid id,  // The weather record's ID
    [FromBody] UpdateWeatherCommand command,  // The data to update the weather record
    CancellationToken cancellationToken)
        {
            if (id != command.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                return Ok(result);  // Return the updated WeatherVm
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);  // Return 404 if the weather record was not found
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");  // Handle any other errors
            }
        }
    }
}
