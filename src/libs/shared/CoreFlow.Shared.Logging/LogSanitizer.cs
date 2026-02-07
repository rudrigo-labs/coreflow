using System.Text.RegularExpressions;

namespace CoreFlow.Shared.Logging;

/// <summary>
/// Utilitário para sanitizar dados sensíveis em logs.
/// Remove ou mascara informações como senhas, tokens, secrets, etc.
/// </summary>
public static class LogSanitizer
{
    private static readonly string[] SensitiveKeys = 
    {
        "password", "pwd", "pass", "passwd",
        "secret", "secrets", "secretkey", "secret_key",
        "token", "tokens", "accesstoken", "access_token", "refreshtoken", "refresh_token",
        "apikey", "api_key", "apisecret", "api_secret",
        "authorization", "auth", "bearer",
        "connectionstring", "connection_string", "connstring",
        "clientsecret", "client_secret", "clientid", "client_id",
        "accountsid", "account_sid", "authtoken", "auth_token",
        "openai", "openai_apikey", "openai_api_key"
    };

    private static readonly Regex SensitivePattern = new(
        $@"(?i)\b({string.Join("|", SensitiveKeys)})\s*[:=]\s*([^\s,}}""']+)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex JsonSensitivePattern = new(
        $@"""({string.Join("|", SensitiveKeys)})""\s*:\s*""([^""]+)""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Sanitiza uma mensagem de log removendo ou mascarando dados sensíveis.
    /// </summary>
    public static string Sanitize(string? message)
    {
        if (string.IsNullOrEmpty(message))
            return message ?? string.Empty;

        var sanitized = message;

        // Remove valores após chaves sensíveis (formato key:value ou key=value)
        sanitized = SensitivePattern.Replace(sanitized, match =>
        {
            var key = match.Groups[1].Value;
            return $"{key}=***REDACTED***";
        });

        // Remove valores em JSON (formato "key":"value")
        sanitized = JsonSensitivePattern.Replace(sanitized, match =>
        {
            var key = match.Groups[1].Value;
            return $"\"{key}\":\"***REDACTED***\"";
        });

        // Remove Authorization headers completos
        sanitized = Regex.Replace(sanitized, 
            @"(?i)(authorization|bearer)\s*:\s*[^\s,}}]+", 
            "$1: ***REDACTED***", 
            RegexOptions.IgnoreCase);

        // Remove connection strings completas
        sanitized = Regex.Replace(sanitized,
            @"(?i)(connectionstring|connection_string)\s*[:=]\s*[^;]+",
            "$1=***REDACTED***",
            RegexOptions.IgnoreCase);

        return sanitized;
    }

    /// <summary>
    /// Sanitiza um objeto serializando para JSON e depois sanitizando.
    /// </summary>
    public static string SanitizeObject(object? obj)
    {
        if (obj == null)
            return "null";

        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });
            return Sanitize(json);
        }
        catch
        {
            return "[Object serialization failed]";
        }
    }

    /// <summary>
    /// Sanitiza uma exception removendo dados sensíveis do stack trace e mensagem.
    /// </summary>
    public static string SanitizeException(Exception? exception)
    {
        if (exception == null)
            return string.Empty;

        var message = Sanitize(exception.Message);
        var stackTrace = Sanitize(exception.StackTrace);

        var result = $"Exception: {exception.GetType().Name}\nMessage: {message}";
        
        if (!string.IsNullOrEmpty(stackTrace))
        {
            result += $"\nStackTrace: {stackTrace}";
        }

        if (exception.InnerException != null)
        {
            result += $"\nInnerException: {SanitizeException(exception.InnerException)}";
        }

        return result;
    }

    /// <summary>
    /// Verifica se uma string contém dados potencialmente sensíveis.
    /// </summary>
    public static bool ContainsSensitiveData(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        return SensitivePattern.IsMatch(text) || JsonSensitivePattern.IsMatch(text);
    }
}

