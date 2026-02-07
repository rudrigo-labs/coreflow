using BCrypt.Net;
using CoreFlow.Api.Middleware;

namespace CoreFlow.Api.Middleware;

/// <summary>
/// Middleware para autenticação via ClientID e ClientSecret.
/// O ClientSecret é armazenado como hash bcrypt e validado usando bcrypt.
/// </summary>
public class ClientAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ClientAuthenticationMiddleware> _logger;
    private readonly Dictionary<string, string> _authorizedClients;

    public ClientAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<ClientAuthenticationMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _authorizedClients = LoadAuthorizedClients(configuration);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Permitir acesso sem autenticação para:
        // - Health check
        // - Swagger UI e JSON
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
        
        if (path == "/health" || 
            path.StartsWith("/swagger") ||
            path == "/" ||
            path == "/swagger/v1/swagger.json")
        {
            await _next(context);
            return;
        }

        // Verificar se ClientID e ClientSecret foram fornecidos
        var clientId = context.Request.Headers["X-Client-Id"].FirstOrDefault();
        var clientSecret = context.Request.Headers["X-Client-Secret"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
        {
            _logger.LogWarning("Tentativa de acesso sem autenticação: {Path}", path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                error = "Unauthorized",
                message = "ClientID e ClientSecret são obrigatórios. Envie via headers: X-Client-Id e X-Client-Secret"
            }));
            return;
        }

        // Validar credenciais
        if (!ValidateClient(clientId, clientSecret))
        {
            _logger.LogWarning("Tentativa de acesso com credenciais inválidas. ClientID: {ClientId}", clientId);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                error = "Unauthorized",
                message = "ClientID ou ClientSecret inválidos"
            }));
            return;
        }

        _logger.LogDebug("Autenticação bem-sucedida para ClientID: {ClientId}", clientId);
        await _next(context);
    }

    private bool ValidateClient(string clientId, string clientSecret)
    {
        if (!_authorizedClients.TryGetValue(clientId, out var storedHash))
        {
            return false;
        }

        // Validar usando bcrypt
        // O storedHash deve ser um hash bcrypt gerado anteriormente
        try
        {
            // BCrypt.Verify compara o texto plano com o hash bcrypt
            return BCrypt.Net.BCrypt.Verify(clientSecret, storedHash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar ClientSecret com bcrypt para ClientID: {ClientId}", clientId);
            return false;
        }
    }

    private static Dictionary<string, string> LoadAuthorizedClients(IConfiguration configuration)
    {
        var clients = new Dictionary<string, string>();

        // Prioridade 1: Variáveis de ambiente (mais seguro para produção)
        // Formato: CORE_SERVICES_CLIENT_{ClientID}=hash_bcrypt
        // Exemplo: CORE_SERVICES_CLIENT_meu-app=$2a$11$hash...
        var envPrefix = "CORE_SERVICES_CLIENT_";
        foreach (var envVar in Environment.GetEnvironmentVariables().Cast<System.Collections.DictionaryEntry>())
        {
            var key = envVar.Key?.ToString() ?? string.Empty;
            if (key.StartsWith(envPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var clientId = key[envPrefix.Length..];
                var clientSecretHash = envVar.Value?.ToString();
                
                if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecretHash))
                {
                    clients[clientId] = clientSecretHash;
                }
            }
        }

        // Prioridade 2: Configuração do appsettings.json (fallback para desenvolvimento)
        // O ClientSecret aqui deve ser um hash bcrypt, não o texto plano
        var clientsSection = configuration.GetSection("AuthorizedClients");
        if (clientsSection != null && clientsSection.Exists())
        {
            foreach (var client in clientsSection.GetChildren())
            {
                var clientId = client["ClientId"];
                var clientSecretHash = client["ClientSecret"]; // Deve ser hash bcrypt

                if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecretHash))
                {
                    // Variáveis de ambiente têm prioridade, então só adiciona se não existir
                    if (!clients.ContainsKey(clientId))
                    {
                        clients[clientId] = clientSecretHash;
                    }
                }
            }
        }

        return clients;
    }
}

