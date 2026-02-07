using CoreFlow.Shared.Results;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;

namespace CoreFlow.Shared.Results
{
    /// <summary>Wrapper de resultado para uso em operações internas.</summary>
    public class Result : IResult
    {
        private readonly List<IResultError> _errors = new List<IResultError>();

        /// <summary>Lista de erros (vazia quando não há erros).</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyCollection<IResultError> Errors
        {
            get { return _errors.AsReadOnly(); }
        }

        /// <summary>Indica se o resultado falhou.</summary>
        [JsonIgnore]
        public bool Failed { get { return !Succeeded; } }

        /// <summary>Mensagem associada.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Message { get; set; }

        /// <summary>Metadados adicionais.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object> Metadata { get; internal set; }

        /// <summary>Mensagem combinada com erros (para depuração).</summary>
        [JsonIgnore]
        public string MessageWithErrors
        {
            get
            {
                if (Succeeded) return null;
                var baseMsg = Message ?? string.Empty;
                return baseMsg + Environment.NewLine + string.Join(",", _errors);
            }
        }

        /// <summary>Pode continuar o fluxo após esse resultado.</summary>
        public bool CanContinue { get; set; } = true;

        /// <summary>Indica sucesso.</summary>
        public bool Succeeded { get; set; }

        /// <summary>Exceção associada (se houver).</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Exception Exception { get; set; }

        /// <summary>Código HTTP associado.</summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>Indica se um registro foi encontrado (para queries).</summary>
        public bool Found { get; set; }

        /// <summary>
        /// Adiciona um erro à coleção de erros do resultado.
        /// </summary>
        /// <param name="errorMessage">A mensagem de erro descritiva a ser adicionada.</param>
        public void AddError(string errorMessage)
        {
            _errors.Add(new ResultError(errorMessage));
        }

        /// <summary>
        /// Adiciona um erro com código à coleção de erros do resultado.
        /// </summary>
        /// <param name="errorMessage">A mensagem de erro descritiva a ser adicionada.</param>
        /// <param name="errorCode">O código de erro associado à mensagem.</param>
        public void AddError(string errorMessage, string errorCode)
        {
            _errors.Add(new ResultError(errorMessage, errorCode));
        }

        /// <summary>
        /// Adiciona múltiplos erros à coleção de erros do resultado.
        /// </summary>
        /// <param name="errors">A coleção de erros a ser adicionada. Se null, nenhum erro será adicionado.</param>
        public void AddErrors(IEnumerable<IResultError> errors)
        {
            if (errors == null) return;
            _errors.AddRange(errors);
        }

        /// <summary>
        /// Adiciona ou atualiza um metadado no dicionário de metadados.
        /// </summary>
        /// <param name="key">A chave do metadado. Não pode ser null ou vazio.</param>
        /// <param name="value">O valor do metadado a ser armazenado.</param>
        /// <exception cref="ArgumentException">Lançada quando key é null ou vazio.</exception>
        public void AddMetadata(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            
            if (Metadata == null) Metadata = new Dictionary<string, object>();
            Metadata[key] = value;
        }

        /// <summary>
        /// Obtém um metadado tipado (struct) pelo nome.
        /// </summary>
        /// <typeparam name="T">O tipo do valor do metadado (deve ser um tipo struct).</typeparam>
        /// <param name="key">A chave do metadado a ser recuperado.</param>
        /// <returns>O valor do metadado convertido para o tipo especificado.</returns>
        /// <exception cref="ArgumentException">Lançada quando key é null ou vazio.</exception>
        /// <exception cref="KeyNotFoundException">Lançada quando o metadado não é encontrado ou quando Metadata é null.</exception>
        /// <exception cref="InvalidCastException">Lançada quando o valor não pode ser convertido para o tipo especificado.</exception>
        public T GetMetadata<T>(string key) where T : struct
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            
            if (Metadata == null || !Metadata.ContainsKey(key))
                throw new KeyNotFoundException($"Metadata item not found: {key}");
            
            var value = Metadata[key];
            if (value == null)
                throw new InvalidOperationException($"Metadata value for key '{key}' is null and cannot be converted to {typeof(T).Name}.");
            
            try
            {
                if (value is T directValue)
                    return directValue;
                
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
            {
                throw new InvalidCastException($"Cannot convert metadata value for key '{key}' from {value.GetType().Name} to {typeof(T).Name}.", ex);
            }
        }
    }

    /// <summary>
    /// Wrapper genérico de resultado que contém dados tipados.
    /// Estende <see cref="Result"/> para incluir dados de tipo específico.
    /// </summary>
    /// <typeparam name="T">O tipo dos dados retornados pela operação.</typeparam>
    public class Result<T> : Result, IResult<T>
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Result{T}"/>.
        /// </summary>
        public Result() : base() { }

        /// <summary>
        /// Obtém ou define os dados retornados pela operação.
        /// </summary>
        /// <value>Os dados da operação. Pode ser null para tipos de referência.</value>
        public T Data { get; set; }

        /// <summary>
        /// Obtém o tipo dos dados retornados.
        /// </summary>
        /// <value>O <see cref="Type"/> que representa o tipo genérico T.</value>
        [JsonIgnore]
        public Type DataType
        {
            get { return typeof(T); }
        }
    }
}

