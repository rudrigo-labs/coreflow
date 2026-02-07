using CoreFlow.Shared.Logging;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreFlow.Shared.Logging
{
    /// <summary>
    /// Fachada estática para logs. Use AppLog.Info/Warn/Error/Debug e AppLog.For&lt;T&gt;().
    /// Helpers para colunas extras (UserId, RequestId, PaymentId, CustomerId, App, CorrelationId).
    /// </summary>
    public static class AppLog
    {
        // ===== Contexto / Sources =====
        public static ILogger For<T>() => Log.ForContext<T>();
        public static ILogger For(string source) => Log.ForContext("SourceContext", source ?? "Unknown");

        // ===== Helpers de propriedades únicas =====
        public static IDisposable Scope(string name, object value) =>
            LogContext.PushProperty(name, value ?? "null");

        public static IDisposable WithApp(string appName) =>
            Scope("App", string.IsNullOrWhiteSpace(appName) ? "PayBridge" : appName);

        public static IDisposable WithUser(string userId) =>
            Scope("UserId", userId ?? string.Empty);

        public static IDisposable WithRequest(string requestId) =>
            Scope("RequestId", requestId ?? string.Empty);

        public static IDisposable WithCorrelation(string correlationId) =>
            Scope("CorrelationId", correlationId ?? string.Empty);

        public static ILogger WithPayment(string paymentId) =>
            Log.ForContext("PaymentId", paymentId ?? string.Empty);

        public static ILogger WithCustomer(string customerId) =>
            Log.ForContext("CustomerId", customerId ?? string.Empty);

        // ===== Helpers de múltiplas propriedades =====
        public static IDisposable ScopeMany(IDictionary<string, object> props)
        {
            if (props == null || props.Count == 0)
                return new NoopDisposable();

            var stack = new CompositeDisposable();
            foreach (var kv in props)
                stack.Add(LogContext.PushProperty(kv.Key, kv.Value ?? "null"));
            return stack;
        }

        /// <summary>
        /// Scope padrão para operações: UserId, RequestId, PaymentId, CustomerId e CorrelationId.
        /// </summary>
        public static IDisposable BeginOperationScope(
            string userId = null,
            string requestId = null,
            string paymentId = null,
            string customerId = null,
            string correlationId = null,
            string appName = null)
        {
            var props = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrEmpty(appName)) props["App"] = appName;
            if (!string.IsNullOrEmpty(userId)) props["UserId"] = userId;
            if (!string.IsNullOrEmpty(requestId)) props["RequestId"] = requestId;
            if (!string.IsNullOrEmpty(paymentId)) props["PaymentId"] = paymentId;
            if (!string.IsNullOrEmpty(customerId)) props["CustomerId"] = customerId;
            if (!string.IsNullOrEmpty(correlationId)) props["CorrelationId"] = correlationId;

            return ScopeMany(props);
        }

        // ===== Níveis =====
        public static void Debug(string messageTemplate, params object[] args) => Log.Debug(messageTemplate, args);
        public static void Info(string messageTemplate, params object[] args) => Log.Information(messageTemplate, args);
        public static void Warn(string messageTemplate, params object[] args) => Log.Warning(messageTemplate, args);
        public static void Error(string messageTemplate, params object[] args) => Log.Error(messageTemplate, args);
        public static void Fatal(string messageTemplate, params object[] args) => Log.Fatal(messageTemplate, args);

        public static void Warn(Exception ex, string messageTemplate, params object[] args) => Log.Warning(ex, messageTemplate, args);
        public static void Error(Exception ex, string messageTemplate, params object[] args) => Log.Error(ex, messageTemplate, args);
        public static void Fatal(Exception ex, string messageTemplate, params object[] args) => Log.Fatal(ex, messageTemplate, args);

        // ===== Utilitários internos =====
        private sealed class NoopDisposable : IDisposable { public void Dispose() { } }

        private sealed class CompositeDisposable : IDisposable
        {
            private readonly Stack<IDisposable> _items = new();
            public void Add(IDisposable d)
            {
                if (d != null) _items.Push(d);
            }
            public void Dispose()
            {
                while (_items.Count > 0)
                {
                    try { _items.Pop().Dispose(); } catch { /* ignore */ }
                }
            }
        }
    }
}

