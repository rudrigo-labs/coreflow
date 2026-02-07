using CoreFlow.Shared.Logging;
using CoreFlow.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreFlow.Shared.Logging
{
    public static class LoggingExtensions
    {
        public static void TrackResultTrace<T>(this ILoggerService<T> logger, Result result, string area, SeverityLevel level = SeverityLevel.Warning)
        {
            var message = result.Message ?? "Resultado sem mensagem explícita";

            var details = result.ToProperties();
            details["Area"] = area;
            details["TraceType"] = "Result";

            var logMessage = $"[{area}] {message}";

            logger.Log(logMessage, level, result.Exception);
        }

        public static void TrackResultTrace<T, TData>(this ILoggerService<T> logger, Result<TData> result, string area, SeverityLevel level = SeverityLevel.Warning)
        {
            logger.TrackResultTrace((Result)result, area, level);
        }

        public static Dictionary<string, string> ToProperties(this Result result)
        {
            var dict = new Dictionary<string, string>
            {
                { "Succeeded", result.Succeeded.ToString() },
                { "StatusCode", result.StatusCode.ToString() }
            };

            if (!string.IsNullOrWhiteSpace(result.Message))
                dict["Message"] = result.Message;

            if (result.Errors is { Count: > 0 })
                dict["Errors"] = string.Join(" | ", result.Errors.Select(e => e.Error));

            if (result.Exception != null)
            {
                dict["ExceptionMessage"] = result.Exception.Message;

                if (result.Exception.InnerException != null)
                    dict["InnerException"] = result.Exception.InnerException.Message;
            }

            if (result.Metadata is { Count: > 0 })
            {
                foreach (var entry in result.Metadata)
                {
                    if (!dict.ContainsKey(entry.Key) && entry.Value != null)
                        dict[entry.Key] = entry.Value.ToString()!;
                }
            }

            return dict;
        }

        public static Dictionary<string, string> ToProperties<TData>(this Result<TData> result)
        {
            var dict = ((Result)result).ToProperties();

            if (result.Data != null)
            {
                dict["DataType"] = typeof(TData).Name;
                dict["DataString"] = result.Data.ToString()!;
            }

            return dict;
        }
    }
}

