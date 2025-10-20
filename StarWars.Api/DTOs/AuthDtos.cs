using StarWars.Api.Models;

namespace StarWars.Api.DTOs;

public record SignUpRequest(string Email, string Password, UserRole Role);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, string Email, UserRole Role);