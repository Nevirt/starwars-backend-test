using System.Net;
using Microsoft.Extensions.Options;

namespace StarWars.Api.Security;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApiKeyConfiguration _config;
    private const string ProblemContentType = "application/json";

    public ApiKeyMiddleware(RequestDelegate next, IOptions<ApiKeyConfiguration> options)
    {
        _next = next;
        _config = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Allow swagger, health and development exceptions bypass
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/swagger") || path.StartsWith("/health"))
        {
            await _next(context);
            return;
        }

        if (string.IsNullOrWhiteSpace(_config.Key))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(_config.Header, out var provided) || provided.Count == 0)
        {
            context.Response.Headers.Append("WWW-Authenticate", $"ApiKey realm=\"{_config.Realm}\"");
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = ProblemContentType;
            await context.Response.WriteAsync("{\"type\":\"Unauthorized\",\"message\":\"Missing API Key\"}");
            return;
        }

        if (!string.Equals(provided.ToString(), _config.Key, StringComparison.Ordinal))
        {
            context.Response.Headers.Append("WWW-Authenticate", $"ApiKey realm=\"{_config.Realm}\" error=\"invalid_api_key\"");
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = ProblemContentType;
            await context.Response.WriteAsync("{\"type\":\"Unauthorized\",\"message\":\"Invalid API Key\"}");
            return;
        }

        await _next(context);
    }
}


