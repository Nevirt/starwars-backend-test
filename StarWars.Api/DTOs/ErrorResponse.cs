using StarWars.Api.Models;

namespace StarWars.Api.DTOs;

public record ErrorResponse(ErrorType Type, string Message);