using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using StarWars.Api.DTOs;
using StarWars.Api.Services;
using StarWars.Api.Models;

namespace StarWars.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Registra un nuevo usuario y retorna un JWT.
    /// </summary>
    [HttpPost("signup")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponse>> SignUp([FromBody] SignUpRequest request)
    {
        try
        {
            _logger.LogInformation("Signup attempt for {Email}", request.Email);
            var result = await _authService.SignUpAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Signup validation failed for {Email}", request.Email);
            Response.Headers.Append("X-Error", "Signup validation failed");
            return BadRequest(new ErrorResponse(ErrorType.Validation, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during signup for {Email}", request.Email);
            Response.Headers.Append("X-Error", "Unexpected error");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorType.ServerError, "Unexpected error during signup"));
        }
    }

    /// <summary>
    /// Autentica un usuario y retorna un JWT.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Login attempt for {Email}", request.Email);
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Invalid credentials for {Email}", request.Email);
            Response.Headers.Append("X-Error", "Invalid credentials");
            return Unauthorized(new ErrorResponse(ErrorType.Unauthorized, "Invalid credentials"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for {Email}", request.Email);
            Response.Headers.Append("X-Error", "Unexpected error");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorType.ServerError, "Unexpected error during login"));
        }
    }
}