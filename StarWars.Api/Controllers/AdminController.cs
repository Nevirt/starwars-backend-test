using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarWars.Api.Services;

namespace StarWars.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireAdmin")]
public class AdminController : ControllerBase
{
    private readonly ISwapiService _swapiService;

    public AdminController(ISwapiService swapiService)
    {
        _swapiService = swapiService;
    }

    [HttpPost("sync-films")]
    public async Task<ActionResult<object>> SyncFilms()
    {
        var added = await _swapiService.SyncFilmsAsync();
        return Ok(new { added });
    }
}


