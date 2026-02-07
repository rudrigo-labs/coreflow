using CoreFlow.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreFlow.Shared.Logging
{
    /// <summary>
    /// Cria escopos de log padronizados para operações do sistema.
    /// </summary>
    public static class LogScopes
    {
        /// <summary>
        /// Escopo completo para uma operação de pagamento.
        /// </summary>
        public static IDisposable PaymentOperation(
            string userId,
            string paymentId,
            string requestId = null,
            string correlationId = null,
            string appName = "PayBridge")
        {
            requestId ??= Guid.NewGuid().ToString("N");
            correlationId ??= Guid.NewGuid().ToString("N");

            return AppLog.BeginOperationScope(
                userId: userId,
                requestId: requestId,
                paymentId: paymentId,
                correlationId: correlationId,
                appName: appName
            );
        }

        /// <summary>
        /// Escopo genérico (quando não é pagamento).
        /// </summary>
        public static IDisposable Default(string userId, string requestId = null, string appName = "PayBridge")
        {
            requestId ??= Guid.NewGuid().ToString("N");

            return AppLog.BeginOperationScope(
                userId: userId,
                requestId: requestId,
                appName: appName
            );
        }
    }
}

