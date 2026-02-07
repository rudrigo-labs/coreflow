using CoreFlow.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace CoreFlow.Shared.Exceptions
{
    /// <summary>
    /// Exceção base global da solução.
    /// Inclui Code, HttpStatus (opcional), Details, CorrelationId e Metadata.
    /// Compatível com .NET Framework (serializável).
    /// </summary>
    [Serializable]
    public class AppException : Exception
    {
        /// <summary>
        /// Obtém o código de erro associado à exceção.
        /// </summary>
        /// <value>O código de erro que identifica o tipo de erro ocorrido.</value>
        public string Code { get; private set; }

        /// <summary>
        /// Obtém o código de status HTTP associado à exceção (quando aplicável).
        /// </summary>
        /// <value>O código de status HTTP. Null se não aplicável.</value>
        public HttpStatusCode? StatusCode { get; private set; }

        /// <summary>
        /// Obtém detalhes adicionais sobre a exceção.
        /// </summary>
        /// <value>Informações detalhadas sobre o erro. Pode ser null.</value>
        public string Details { get; private set; }

        /// <summary>
        /// Obtém o ID de correlação para rastreamento em logs e tracing.
        /// </summary>
        /// <value>O identificador de correlação. Pode ser null.</value>
        public string CorrelationId { get; private set; }

        /// <summary>
        /// Obtém metadados adicionais associados à exceção.
        /// Evita reutilizar Exception.Data (que é IDictionary); mantém Metadata separado e tipado.
        /// </summary>
        /// <value>Dicionário de metadados adicionais. Nunca é null após inicialização.</value>
        public IDictionary<string, object> Metadata { get; private set; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="AppException"/> com valores padrão.
        /// </summary>
        public AppException()
            : this("An unexpected error has occurred.", "error", null, null, null) { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="AppException"/> com uma mensagem.
        /// </summary>
        /// <param name="message">A mensagem que descreve o erro.</param>
        public AppException(string message)
            : this(message, "error", null, null, null) { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="AppException"/> com uma mensagem e exceção interna.
        /// </summary>
        /// <param name="message">A mensagem que descreve o erro.</param>
        /// <param name="inner">A exceção que é a causa da exceção atual.</param>
        public AppException(string message, Exception inner)
            : this(message, "error", null, null, inner) { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="AppException"/> com mensagem e código de erro.
        /// </summary>
        /// <param name="message">A mensagem que descreve o erro.</param>
        /// <param name="code">O código de erro que identifica o tipo de erro.</param>
        public AppException(string message, string code)
            : this(message, code, null, null, null) { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="AppException"/> com mensagem, código e exceção interna.
        /// </summary>
        /// <param name="message">A mensagem que descreve o erro.</param>
        /// <param name="code">O código de erro que identifica o tipo de erro.</param>
        /// <param name="inner">A exceção que é a causa da exceção atual.</param>
        public AppException(string message, string code, Exception inner)
            : this(message, code, null, null, inner) { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="AppException"/> com todos os parâmetros.
        /// </summary>
        /// <param name="message">A mensagem que descreve o erro.</param>
        /// <param name="code">O código de erro que identifica o tipo de erro. Se null ou vazio, será definido como "error".</param>
        /// <param name="statusCode">O código de status HTTP associado (opcional).</param>
        /// <param name="details">Detalhes adicionais sobre o erro (opcional).</param>
        /// <param name="inner">A exceção que é a causa da exceção atual (opcional).</param>
        public AppException(string message, string code, HttpStatusCode? statusCode, string details, Exception inner)
            : base(message, inner)
        {
            Code = string.IsNullOrWhiteSpace(code) ? "error" : code;
            StatusCode = statusCode;
            Details = details;
            Metadata = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Define ou atualiza o CorrelationId para rastreamento em logs e tracing.
        /// </summary>
        /// <param name="correlationId">O identificador de correlação a ser definido.</param>
        /// <returns>Esta instância para permitir encadeamento de chamadas.</returns>
        public AppException WithCorrelation(string correlationId)
        {
            CorrelationId = correlationId;
            return this;
        }

        /// <summary>
        /// Adiciona ou atualiza um item no dicionário de metadados.
        /// </summary>
        /// <param name="key">A chave do metadado. Se null ou vazio, nenhum metadado será adicionado.</param>
        /// <param name="value">O valor do metadado a ser armazenado.</param>
        /// <returns>Esta instância para permitir encadeamento de chamadas.</returns>
        public AppException WithMeta(string key, object value)
        {
            if (!string.IsNullOrWhiteSpace(key))
                Metadata[key] = value;
            return this;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="AppException"/> com dados de serialização.
        /// </summary>
        /// <param name="info">O objeto <see cref="SerializationInfo"/> que contém os dados serializados.</param>
        /// <param name="context">O <see cref="StreamingContext"/> que contém informações sobre a origem ou destino.</param>
        protected AppException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Code = info.GetString(nameof(Code));
            Details = info.GetString(nameof(Details));
            CorrelationId = info.GetString(nameof(CorrelationId));

            var status = info.GetInt32(nameof(StatusCode) + "_Has");
            if (status == 1)
            {
                var sc = info.GetInt32(nameof(StatusCode) + "_Val");
                StatusCode = (HttpStatusCode)sc;
            }

            // Metadata simples: serializa como pares chave/valor (string ? object.ToString()).
            var count = info.GetInt32(nameof(Metadata) + "_Count");
            Metadata = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < count; i++)
            {
                var k = info.GetString(nameof(Metadata) + "_K_" + i);
                var v = info.GetString(nameof(Metadata) + "_V_" + i);
                Metadata[k] = v;
            }
        }

        /// <summary>
        /// Define o <see cref="SerializationInfo"/> com informações sobre a exceção.
        /// </summary>
        /// <param name="info">O <see cref="SerializationInfo"/> que contém os dados serializados do objeto.</param>
        /// <param name="context">O <see cref="StreamingContext"/> que contém informações contextuais sobre a origem ou destino.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(Code), Code);
            info.AddValue(nameof(Details), Details);
            info.AddValue(nameof(CorrelationId), CorrelationId);

            if (StatusCode.HasValue)
            {
                info.AddValue(nameof(StatusCode) + "_Has", 1);
                info.AddValue(nameof(StatusCode) + "_Val", (int)StatusCode.Value);
            }
            else
            {
                info.AddValue(nameof(StatusCode) + "_Has", 0);
            }

            // Metadata como pares string/string
            var count = Metadata != null ? Metadata.Count : 0;
            info.AddValue(nameof(Metadata) + "_Count", count);
            if (count > 0)
            {
                int i = 0;
                foreach (var kv in Metadata)
                {
                    info.AddValue(nameof(Metadata) + "_K_" + i, kv.Key);
                    info.AddValue(nameof(Metadata) + "_V_" + i, kv.Value != null ? kv.Value.ToString() : null);
                    i++;
                }
            }
        }
    }
}

