namespace CoreFlow.Api.Extensions;

/// <summary>
/// Extensões para configuração de CORS.
/// </summary>
public static class CorsExtensions
{
    /// <summary>
    /// Adiciona configuração de CORS aos serviços.
    /// </summary>
    /// <param name="services">A coleção de serviços.</param>
    /// <param name="configuration">A configuração da aplicação.</param>
    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .SetIsOriginAllowed(origin => IsAllowedOrigin(configuration, origin))
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("X-Client-Id");
            });
        });

        return services;
    }

    /// <summary>
    /// Obtém as origens permitidas para CORS.
    /// </summary>
    private static bool IsAllowedOrigin(IConfiguration configuration, string? origin)
    {
        if (string.IsNullOrWhiteSpace(origin))
            return false;

        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
            return false;

        // Opcional: só permitir http/https
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            return false;

        // Permitir localhost / loopback (dev)
        if (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
            uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase))
            return true;

        // Domínio base (permite raiz + qualquer subdomínio)
        var baseDomain = "connectsolutions.com.br";

        // Ex.: connectsolutions.com.br
        if (uri.Host.Equals(baseDomain, StringComparison.OrdinalIgnoreCase))
            return true;

        // Ex.: qualquercoisa.connectsolutions.com.br
        if (uri.Host.EndsWith("." + baseDomain, StringComparison.OrdinalIgnoreCase))
            return true;

        // Origens customizadas no appsettings.json (lista exata)
        var customOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<List<string>>();
        if (customOrigins != null && customOrigins.Count > 0)
        {
            // Comparar contra a origin completa (scheme+host+porta), igual ao que o browser envia
            return customOrigins.Any(o => string.Equals(o?.Trim(), origin, StringComparison.OrdinalIgnoreCase));
        }

        return false;
    }
}

