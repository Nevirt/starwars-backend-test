namespace StarWars.Api.DTOs;

public record MovieCreateRequest(string Title, string? Description, int? ReleaseYear, string? Director, string? Producer);
public record MovieUpdateRequest(string Title, string? Description, int? ReleaseYear, string? Director, string? Producer);
public record MovieResponse(int Id, string Title, string? Description, int? ReleaseYear, string? Director, string? Producer, string? ExternalId);


