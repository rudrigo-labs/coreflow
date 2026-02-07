using CoreFlow.Shared.Logging;
using CoreFlow.Shared.Extensions;
using CoreFlow.Shared.Results;
using CoreFlow.Shared.Results.Extensions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CoreFlow.Shared.Logging
{
    public class LoggerService<T> : ILoggerService<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerService(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void Log(string message, SeverityLevel level, Exception? exception = null)
        {
            // Sanitiza a mensagem antes de logar
            var sanitizedMessage = LogSanitizer.Sanitize(message);

            // 1. Persistência com ILogger<T>
            var logLevel = MapSeverityToLogLevel(level);
            
            // Sanitiza a exception se existir - loga a exception original mas com mensagem sanitizada
            if (exception != null)
            {
                // Sanitiza a mensagem da exception antes de logar
                var originalMessage = exception.Message;
                var sanitizedExceptionMessage = LogSanitizer.Sanitize(originalMessage);
                
                // Se a mensagem foi alterada, cria uma exception temporária para logar
                if (sanitizedExceptionMessage != originalMessage)
                {
                    var sanitizedEx = new Exception(sanitizedExceptionMessage, exception.InnerException);
                    _logger.Log(logLevel, sanitizedEx, "[{Context}] {Message}", typeof(T).Name, sanitizedMessage);
                }
                else
                {
                    _logger.Log(logLevel, exception, "[{Context}] {Message}", typeof(T).Name, sanitizedMessage);
                }
            }
            else
            {
                _logger.Log(logLevel, "[{Context}] {Message}", typeof(T).Name, sanitizedMessage);
            }

            // 2. Exibição no console com cor (usa mensagem sanitizada)
            WriteToConsole(level, sanitizedMessage, exception);
        }

        public void Log(Result result, string? area = null, SeverityLevel level = SeverityLevel.Error)
        {
            if (result.Succeeded) return;

            var message = result.GetMessageOrError();
            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = false });
            var sanitizedJson = LogSanitizer.Sanitize(json);

            Log($"[{area ?? typeof(T).Name}] {message} | Details: {sanitizedJson}", level, result.Exception);
        }

        public void Log<TData>(Result<TData> result, string? area = null, SeverityLevel level = SeverityLevel.Error)
        {
            if (result.Succeeded) return;

            var message = result.GetMessageOrError();
            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = false });
            var sanitizedJson = LogSanitizer.Sanitize(json);

            Log($"[{area ?? typeof(T).Name}] {message} | Details: {sanitizedJson}", level, result.Exception);
        }

        private static LogLevel MapSeverityToLogLevel(SeverityLevel level) =>
            level switch
            {
                SeverityLevel.Verbose => LogLevel.Trace,
                SeverityLevel.Information => LogLevel.Information,
                SeverityLevel.Warning => LogLevel.Warning,
                SeverityLevel.Error => LogLevel.Error,
                SeverityLevel.Critical => LogLevel.Critical,
                _ => LogLevel.Information
            };

        private void WriteToConsole(SeverityLevel level, string message, Exception? exception = null)
        {
            var originalColor = Console.ForegroundColor;

            Console.ForegroundColor = level switch
            {
                SeverityLevel.Information => ConsoleColor.Cyan,
                SeverityLevel.Warning => ConsoleColor.Yellow,
                SeverityLevel.Error => ConsoleColor.Red,
                SeverityLevel.Critical => ConsoleColor.DarkRed,
                SeverityLevel.Verbose => ConsoleColor.Gray,
                _ => ConsoleColor.White
            };

            var prefix = $"[{level.ToString().ToUpper()}]";
            Console.WriteLine($"{prefix} {message}");

            if (exception is not null)
                Console.WriteLine($"Exception: {exception.Message}");

            Console.ForegroundColor = originalColor;
        }
    }
}

