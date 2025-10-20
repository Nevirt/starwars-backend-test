using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StarWars.Api.Data;
using StarWars.Api.Models;

namespace StarWars.Api.Services;

public class SwapiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
}

public interface ISwapiService
{
    Task<int> SyncFilmsAsync(CancellationToken cancellationToken = default);
}

public class SwapiService : ISwapiService
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _db;
    private readonly SwapiSettings _settings;

    public SwapiService(HttpClient httpClient, AppDbContext db, IOptions<SwapiSettings> settings)
    {
        _httpClient = httpClient;
        _db = db;
        _settings = settings.Value;
    }

    private sealed class SwapiFilmsResponse
    {
        public int total_records { get; set; }
        public List<SwapiFilm> result { get; set; } = new();
    }

    private sealed class SwapiFilm
    {
        public string uid { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public SwapiFilmProperties properties { get; set; } = new();
    }

    private sealed class SwapiFilmProperties
    {
        public string title { get; set; } = string.Empty;
        public string director { get; set; } = string.Empty;
        public string producer { get; set; } = string.Empty;
        public string release_date { get; set; } = string.Empty;
    }

    public async Task<int> SyncFilmsAsync(CancellationToken cancellationToken = default)
    {
        var url = $"{_settings.BaseUrl}/films";
        var response = await _httpClient.GetFromJsonAsync<SwapiFilmsResponse>(url, cancellationToken);
        if (response == null) return 0;

        var count = 0;
        foreach (var film in response.result)
        {
            var year = int.TryParse(film.properties.release_date.Split('-').FirstOrDefault(), out var y) ? y : (int?)null;
            var existing = await _db.Movies.FirstOrDefaultAsync(m => m.ExternalId == film.uid, cancellationToken);
            if (existing is null)
            {
                _db.Movies.Add(new Movie
                {
                    Title = film.properties.title,
                    Description = film.description,
                    Director = film.properties.director,
                    Producer = film.properties.producer,
                    ReleaseYear = year,
                    ExternalId = film.uid
                });
                count++;
            }
            else
            {
                existing.Title = film.properties.title;
                existing.Description = film.description;
                existing.Director = film.properties.director;
                existing.Producer = film.properties.producer;
                existing.ReleaseYear = year;
            }
        }
        await _db.SaveChangesAsync(cancellationToken);
        return count;
    }
}