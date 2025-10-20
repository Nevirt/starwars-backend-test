namespace StarWars.Api.DTOs;

public record SignUpRequest(string Email, string Password, string Role);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, string Email, string Role);


