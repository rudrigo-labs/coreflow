using CoreFlow.Api.Endpoints;
using CoreFlow.Shared.Results;

namespace CoreFlow.Api.Extensions;

/// <summary>
/// Extensões para registro de endpoints.
/// </summary>
public static class EndpointsExtensions
{
    /// <summary>
    /// Registra todos os endpoints da aplicação.
    /// </summary>
    /// <param name="app">A aplicação web.</param>
    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        // Health check endpoint
        app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
            .WithName("HealthCheck")
            .WithTags("Health")
            .Produces(StatusCodes.Status200OK);

        // Registrar endpoints por serviço
        app.MapEmailEndpoints();
        app.MapBlobEndpoints();
        app.MapCustomerEndpoints();
        return app;
    }
}
