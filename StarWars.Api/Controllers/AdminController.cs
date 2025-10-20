using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StarWars.Api.Services;
using StarWars.Api.DTOs;
using StarWars.Api.Models;

namespace StarWars.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireAdmin")]
public class AdminController : ControllerBase
{
    private readonly ISwapiService _swapiService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(ISwapiService swapiService, ILogger<AdminController> logger)
    {
        _swapiService = swapiService;
        _logger = logger;
    }

    /// <summary>
    /// Sincroniza el listado de películas desde SWAPI. Solo Administradores.
    /// </summary>
    [HttpPost("sync-films")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> SyncFilms()
    {
        try
        {
            _logger.LogInformation("Syncing films from SWAPI");
            var added = await _swapiService.SyncFilmsAsync();
            return Ok(new { added });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "SWAPI unreachable");
            Response.Headers.Append("X-Error", "External service error");
            return StatusCode(StatusCodes.Status502BadGateway, new ErrorResponse(ErrorType.ExternalService, "Failed to fetch films from SWAPI"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error syncing films");
            Response.Headers.Append("X-Error", "Unexpected error");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorType.ServerError, "Unexpected error syncing films"));
        }
    }
}