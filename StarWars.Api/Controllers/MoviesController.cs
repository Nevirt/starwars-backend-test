using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StarWars.Api.DTOs;
using StarWars.Api.Models;
using StarWars.Api.Services;

namespace StarWars.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(IMovieService movieService, ILogger<MoviesController> logger)
    {
        _movieService = movieService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene el listado de películas.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<MovieResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<MovieResponse>>> GetAll()
    {
        try
        {
            _logger.LogInformation("Fetching movies");
            var movies = await _movieService.GetAllAsync();
            var result = movies.Select(m => new MovieResponse(m.Id, m.Title, m.Description, m.ReleaseYear, m.Director, m.Producer, m.ExternalId)).ToList();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching movies");
            Response.Headers.Append("X-Error", "Unexpected error");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorType.ServerError, "Unexpected error fetching movies"));
        }
    }

    /// <summary>
    /// Obtiene los detalles de una película por Id. Requiere rol Usuario Regular.
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "RequireRegularUser")]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MovieResponse>> GetById(int id)
    {
        try
        {
            _logger.LogInformation("Fetching movie {Id}", id);
            var movie = await _movieService.GetByIdAsync(id);
            if (movie is null)
            {
                _logger.LogWarning("Movie not found {Id}", id);
                Response.Headers.Append("X-Error", "Not Found");
                return NotFound(new ErrorResponse(ErrorType.NotFound, "Movie not found"));
            }
            return Ok(new MovieResponse(movie.Id, movie.Title, movie.Description, movie.ReleaseYear, movie.Director, movie.Producer, movie.ExternalId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching movie {Id}", id);
            Response.Headers.Append("X-Error", "Unexpected error");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorType.ServerError, "Unexpected error fetching movie"));
        }
    }

    /// <summary>
    /// Crea una nueva película. Requiere rol Administrador.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequireAdmin")]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MovieResponse>> Create([FromBody] MovieCreateRequest request)
    {
        try
        {
            _logger.LogInformation("Creating movie {Title}", request.Title);
            var movie = await _movieService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = movie.Id }, new MovieResponse(movie.Id, movie.Title, movie.Description, movie.ReleaseYear, movie.Director, movie.Producer, movie.ExternalId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating movie {Title}", request.Title);
            Response.Headers.Append("X-Error", "Unexpected error");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorType.ServerError, "Unexpected error creating movie"));
        }
    }

    /// <summary>
    /// Actualiza una película existente. Requiere rol Administrador.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "RequireAdmin")]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MovieResponse>> Update(int id, [FromBody] MovieUpdateRequest request)
    {
        try
        {
            _logger.LogInformation("Updating movie {Id}", id);
            var movie = await _movieService.UpdateAsync(id, request);
            if (movie is null)
            {
                _logger.LogWarning("Movie not found for update {Id}", id);
                Response.Headers.Append("X-Error", "Not Found");
                return NotFound(new ErrorResponse(ErrorType.NotFound, "Movie not found"));
            }
            return Ok(new MovieResponse(movie.Id, movie.Title, movie.Description, movie.ReleaseYear, movie.Director, movie.Producer, movie.ExternalId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating movie {Id}", id);
            Response.Headers.Append("X-Error", "Unexpected error");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorType.ServerError, "Unexpected error updating movie"));
        }
    }

    /// <summary>
    /// Elimina una película. Requiere rol Administrador.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "RequireAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation("Deleting movie {Id}", id);
            var deleted = await _movieService.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Movie not found for delete {Id}", id);
                Response.Headers.Append("X-Error", "Not Found");
                return NotFound(new ErrorResponse(ErrorType.NotFound, "Movie not found"));
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting movie {Id}", id);
            Response.Headers.Append("X-Error", "Unexpected error");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorType.ServerError, "Unexpected error deleting movie"));
        }
    }
}