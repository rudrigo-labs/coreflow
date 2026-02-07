namespace CoreFlow.Shared.Results
{
    /// <summary>
    /// Interface que representa um erro específico em um resultado de operação.
    /// Fornece informações sobre o erro, incluindo mensagem e código de erro.
    /// </summary>
    public interface IResultError
    {
        /// <summary>
        /// Obtém a mensagem de erro descritiva.
        /// </summary>
        /// <value>Mensagem que descreve o erro ocorrido.</value>
        string Error { get; }

        /// <summary>
        /// Obtém o código de erro associado.
        /// </summary>
        /// <value>Código identificador do erro. Pode ser usado para tratamento programático de erros específicos.</value>
        string Code { get; }
    }
}

