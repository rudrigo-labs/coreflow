using CoreFlow.Shared.Results;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace CoreFlow.Shared.Results
{
    /// <summary>
    /// DTO (Data Transfer Object) público simplificado para exposição em APIs.
    /// Remove informações internas e expõe apenas dados essenciais para clientes externos.
    /// </summary>
    public class PublicResult
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a operação foi executada com sucesso.
        /// </summary>
        /// <value><c>true</c> se a operação foi bem-sucedida; caso contrário, <c>false</c>.</value>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Obtém ou define a mensagem descritiva do resultado.
        /// </summary>
        /// <value>A mensagem do resultado. Não será serializada se for null.</value>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Message { get; set; }

        /// <summary>
        /// Obtém ou define a lista de mensagens de erro.
        /// </summary>
        /// <value>Lista de strings contendo mensagens de erro. Não será serializada se for null.</value>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string> Errors { get; set; }

        /// <summary>
        /// Cria uma instância de <see cref="PublicResult"/> a partir de um <see cref="Result"/>.
        /// </summary>
        /// <param name="result">O resultado interno a ser convertido.</param>
        /// <returns>Uma nova instância de <see cref="PublicResult"/> com os dados do resultado convertidos.</returns>
        public static PublicResult From(Result result)
        {
            var pr = new PublicResult
            {
                Succeeded = result != null && result.Succeeded,
                Message = (result != null && !string.IsNullOrWhiteSpace(result.Message)) ? result.Message : null
            };

            if (result != null && result.Errors != null)
            {
                var list = result.Errors
                    .Where(e => e != null && !string.IsNullOrWhiteSpace(e.Error))
                    .Select(e => e.Error)
                    .ToList();

                pr.Errors = list.Count > 0 ? list : null;
            }

            return pr;
        }
    }

    /// <summary>
    /// DTO (Data Transfer Object) público genérico com dados tipados para exposição em APIs.
    /// Estende <see cref="PublicResult"/> para incluir dados de tipo específico.
    /// </summary>
    /// <typeparam name="T">O tipo dos dados a serem retornados.</typeparam>
    public class PublicResult<T> : PublicResult
    {
        /// <summary>
        /// Obtém ou define os dados retornados pela operação.
        /// </summary>
        /// <value>Os dados da operação. Não será serializado se for null.</value>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T Data { get; set; }

        /// <summary>
        /// Cria uma instância de <see cref="PublicResult{T}"/> a partir de um <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="result">O resultado interno genérico a ser convertido.</param>
        /// <returns>Uma nova instância de <see cref="PublicResult{T}"/> com os dados do resultado convertidos.</returns>
        public static PublicResult<T> From(Result<T> result)
        {
            var pr = new PublicResult<T>
            {
                Succeeded = result != null && result.Succeeded,
                Message = (result != null && !string.IsNullOrWhiteSpace(result.Message)) ? result.Message : null,
                Data = result != null ? result.Data : default
            };

            if (result != null && result.Errors != null)
            {
                var list = result.Errors
                    .Where(e => e != null && !string.IsNullOrWhiteSpace(e.Error))
                    .Select(e => e.Error)
                    .ToList();

                pr.Errors = list.Count > 0 ? list : null;
            }

            return pr;
        }
    }
}

