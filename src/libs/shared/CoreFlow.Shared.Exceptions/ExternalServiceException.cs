using CoreFlow.Shared.Exceptions;
using System;
using System.Net;

namespace CoreFlow.Shared.Exceptions
{
    /// <summary>
    /// Exceção lançada quando ocorre um erro em comunicação com serviços externos.
    /// Armazena informações sobre o provedor do serviço, código de status HTTP e corpo da resposta.
    /// </summary>
    [Serializable]
    public class ExternalServiceException : AppException
    {
        /// <summary>
        /// Obtém o nome do provedor do serviço externo que causou o erro.
        /// </summary>
        /// <value>O nome do provedor. Padrão: "external" se não especificado.</value>
        public string Provider { get; private set; }

        /// <summary>
        /// Obtém o corpo da resposta recebida do serviço externo.
        /// </summary>
        /// <value>O conteúdo da resposta HTTP. Pode ser null.</value>
        public string ResponseBody { get; private set; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ExternalServiceException"/>.
        /// </summary>
        /// <param name="provider">O nome do provedor do serviço externo.</param>
        /// <param name="message">A mensagem que descreve o erro.</param>
        /// <param name="statusCode">O código de status HTTP retornado pelo serviço externo.</param>
        /// <param name="responseBody">O corpo da resposta HTTP recebida (opcional).</param>
        public ExternalServiceException(string provider, string message, HttpStatusCode statusCode, string responseBody)
            : base(message, "external_service_error", statusCode, null, null)
        {
            Provider = provider ?? "external";
            ResponseBody = responseBody;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ExternalServiceException"/> com exceção interna.
        /// </summary>
        /// <param name="provider">O nome do provedor do serviço externo.</param>
        /// <param name="message">A mensagem que descreve o erro.</param>
        /// <param name="statusCode">O código de status HTTP retornado pelo serviço externo.</param>
        /// <param name="responseBody">O corpo da resposta HTTP recebida (opcional).</param>
        /// <param name="inner">A exceção que é a causa da exceção atual.</param>
        public ExternalServiceException(string provider, string message, HttpStatusCode statusCode, string responseBody, Exception inner)
            : base(message, "external_service_error", statusCode, null, inner)
        {
            Provider = provider ?? "external";
            ResponseBody = responseBody;
        }
    }
}

