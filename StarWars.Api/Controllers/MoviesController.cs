using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarWars.Api.DTOs;
using StarWars.Api.Models;
using StarWars.Api.Services;

namespace StarWars.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<MovieResponse>>> GetAll()
    {
        var movies = await _movieService.GetAllAsync();
        var result = movies.Select(m => new MovieResponse(m.Id, m.Title, m.Description, m.ReleaseYear, m.Director, m.Producer, m.ExternalId)).ToList();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "RequireRegularUser")]
    public async Task<ActionResult<MovieResponse>> GetById(int id)
    {
        var movie = await _movieService.GetByIdAsync(id);
        if (movie is null) return NotFound();
        return Ok(new MovieResponse(movie.Id, movie.Title, movie.Description, movie.ReleaseYear, movie.Director, movie.Producer, movie.ExternalId));
    }

    [HttpPost]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<ActionResult<MovieResponse>> Create([FromBody] MovieCreateRequest request)
    {
        var movie = await _movieService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = movie.Id }, new MovieResponse(movie.Id, movie.Title, movie.Description, movie.ReleaseYear, movie.Director, movie.Producer, movie.ExternalId));
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<ActionResult<MovieResponse>> Update(int id, [FromBody] MovieUpdateRequest request)
    {
        var movie = await _movieService.UpdateAsync(id, request);
        if (movie is null) return NotFound();
        return Ok(new MovieResponse(movie.Id, movie.Title, movie.Description, movie.ReleaseYear, movie.Director, movie.Producer, movie.ExternalId));
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _movieService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}


