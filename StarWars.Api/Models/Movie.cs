namespace StarWars.Api.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ReleaseYear { get; set; }
    public string? Director { get; set; }
    public string? Producer { get; set; }
    public string? ExternalId { get; set; }
}


