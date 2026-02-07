using CoreFlow.Shared.Results;
using System.Collections.Generic;

namespace CoreFlow.Shared.Results
{
    /// <summary>
    /// Interface base para representar o resultado de uma operação.
    /// Fornece informações sobre sucesso/falha, mensagens e erros associados.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Obtém a coleção de erros associados ao resultado.
        /// </summary>
        /// <value>Coleção somente leitura de erros. Vazia quando a operação foi bem-sucedida.</value>
        IReadOnlyCollection<IResultError> Errors { get; }

        /// <summary>
        /// Obtém a mensagem descritiva do resultado.
        /// </summary>
        /// <value>Mensagem que descreve o resultado da operação. Pode ser null.</value>
        string Message { get; }

        /// <summary>
        /// Indica se a operação foi executada com sucesso.
        /// </summary>
        /// <value><c>true</c> se a operação foi bem-sucedida; caso contrário, <c>false</c>.</value>
        bool Succeeded { get; }
    }

    /// <summary>
    /// Interface genérica para representar o resultado de uma operação que retorna dados.
    /// </summary>
    /// <typeparam name="T">O tipo dos dados retornados pela operação.</typeparam>
    public interface IResult<out T> : IResult
    {
        /// <summary>
        /// Obtém os dados retornados pela operação.
        /// </summary>
        /// <value>Os dados da operação. Pode ser null para tipos de referência.</value>
        T Data { get; }
    }
}

