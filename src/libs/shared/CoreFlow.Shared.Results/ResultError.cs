using CoreFlow.Shared.Results;

namespace CoreFlow.Shared.Results
{
    /// <summary>
    /// Representa um erro específico em um resultado de operação.
    /// Implementa <see cref="IResultError"/> fornecendo mensagem e código de erro.
    /// </summary>
    public class ResultError : IResultError
    {
        /// <summary>
        /// Obtém a mensagem de erro descritiva.
        /// </summary>
        /// <value>Mensagem que descreve o erro ocorrido.</value>
        public string Error { get; private set; }

        /// <summary>
        /// Obtém o código de erro associado.
        /// </summary>
        /// <value>Código identificador do erro. Pode ser usado para tratamento programático de erros específicos.</value>
        public string Code { get; private set; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ResultError"/>.
        /// </summary>
        public ResultError() { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ResultError"/> com uma mensagem de erro.
        /// </summary>
        /// <param name="error">A mensagem de erro descritiva.</param>
        public ResultError(string error)
        {
            Error = error;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ResultError"/> com mensagem e código de erro.
        /// </summary>
        /// <param name="error">A mensagem de erro descritiva.</param>
        /// <param name="code">O código de erro associado.</param>
        public ResultError(string error, string code) : this(error)
        {
            Code = code;
        }

        /// <summary>
        /// Retorna uma representação em string do erro.
        /// </summary>
        /// <returns>String formatada contendo o código (se disponível) e a mensagem de erro.</returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Code))
                return $"Error[{Code}]: {Error}";
            return Error ?? string.Empty;
        }
    }
}

