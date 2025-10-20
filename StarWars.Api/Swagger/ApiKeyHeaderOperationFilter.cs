using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using StarWars.Api.Security;

namespace StarWars.Api.Swagger;

public class ApiKeyHeaderOperationFilter : IOperationFilter
{
    private readonly ApiKeyConfiguration _config;

    public ApiKeyHeaderOperationFilter(IOptions<ApiKeyConfiguration> options)
    {
        _config = options.Value;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (string.IsNullOrWhiteSpace(_config.Header)) return;

        operation.Parameters ??= new List<OpenApiParameter>();

        // Avoid duplicates
        if (operation.Parameters.Any(p => string.Equals(p.Name, _config.Header, StringComparison.OrdinalIgnoreCase)))
            return;

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = _config.Header,
            In = ParameterLocation.Header,
            Description = "ApiKey para poder autorizar petición",
            Required = true,
            Schema = new OpenApiSchema { Type = "string" }
        });
    }
}


