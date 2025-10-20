using Microsoft.EntityFrameworkCore;
using StarWars.Api.Data;
using StarWars.Api.DTOs;
using StarWars.Api.Models;

namespace StarWars.Api.Services;

public interface IMovieService
{
    Task<List<Movie>> GetAllAsync();
    Task<Movie?> GetByIdAsync(int id);
    Task<Movie> CreateAsync(MovieCreateRequest request);
    Task<Movie?> UpdateAsync(int id, MovieUpdateRequest request);
    Task<bool> DeleteAsync(int id);
}

public class MovieService : IMovieService
{
    private readonly AppDbContext _db;

    public MovieService(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Movie>> GetAllAsync()
    {
        return _db.Movies.AsNoTracking().OrderBy(m => m.Title).ToListAsync();
    }

    public Task<Movie?> GetByIdAsync(int id)
    {
        return _db.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Movie> CreateAsync(MovieCreateRequest request)
    {
        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            ReleaseYear = request.ReleaseYear,
            Director = request.Director,
            Producer = request.Producer
        };
        _db.Movies.Add(movie);
        await _db.SaveChangesAsync();
        return movie;
    }

    public async Task<Movie?> UpdateAsync(int id, MovieUpdateRequest request)
    {
        var movie = await _db.Movies.FindAsync(id);
        if (movie is null) return null;
        movie.Title = request.Title;
        movie.Description = request.Description;
        movie.ReleaseYear = request.ReleaseYear;
        movie.Director = request.Director;
        movie.Producer = request.Producer;
        await _db.SaveChangesAsync();
        return movie;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        if (movie is null) return false;
        _db.Movies.Remove(movie);
        await _db.SaveChangesAsync();
        return true;
    }
}