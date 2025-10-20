namespace StarWars.Api.Models;

public enum UserRole
{
    User = 0,
    Admin = 1
}

public enum ErrorType
{
    Validation = 0,
    Unauthorized = 1,
    NotFound = 2,
    ExternalService = 3,
    ServerError = 4
}