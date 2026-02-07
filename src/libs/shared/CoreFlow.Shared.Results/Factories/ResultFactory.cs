using CoreFlow.Shared.Results;
using CoreFlow.Shared.Results.Extensions;
using System;

namespace CoreFlow.Shared.Results.Factories
{
    /// <summary>
    /// Factory estática para criar instâncias de <see cref="Result"/> e <see cref="PublicResult"/>.
    /// Fornece métodos convenientes para criar resultados de sucesso, falha e aviso.
    /// </summary>
    public static partial class ResultFactory
    {
        #region Result<T> - Sucesso

        /// <summary>
        /// Cria um resultado de sucesso com dados tipados.
        /// </summary>
        /// <typeparam name="T">O tipo dos dados retornados.</typeparam>
        /// <param name="data">Os dados a serem retornados no resultado.</param>
        /// <returns>Uma nova instância de <see cref="Result{T}"/> com sucesso e dados.</returns>
        public static Result<T> Success<T>(T data)
            => new Result<T>().Successful(data);

        /// <summary>
        /// Cria um resultado de sucesso sem dados.
        /// </summary>
        /// <returns>Uma nova instância de <see cref="Result"/> com sucesso.</returns>
        public static Result Success()
            => new Result().Successful();

        /// <summary>
        /// Cria um resultado de sucesso com uma mensagem.
        /// </summary>
        /// <param name="message">A mensagem descritiva do sucesso.</param>
        /// <returns>Uma nova instância de <see cref="Result"/> com sucesso e mensagem.</returns>
        public static Result Success(string message)
            => new Result().Successful(message);

        #endregion

        #region Result<T> - Aviso

        /// <summary>
        /// Cria um resultado de aviso (warning) sem dados.
        /// </summary>
        /// <typeparam name="T">O tipo dos dados retornados.</typeparam>
        /// <param name="message">A mensagem de aviso. Padrão: string vazia.</param>
        /// <returns>Uma nova instância de <see cref="Result{T}"/> com status de aviso.</returns>
        public static Result<T> Warning<T>(string message = "")
           => new Result<T>().Warning(message);

        /// <summary>
        /// Cria um resultado de aviso (warning) com dados.
        /// </summary>
        /// <typeparam name="T">O tipo dos dados retornados.</typeparam>
        /// <param name="data">Os dados a serem retornados no resultado.</param>
        /// <param name="message">A mensagem de aviso. Padrão: string vazia.</param>
        /// <returns>Uma nova instância de <see cref="Result{T}"/> com status de aviso e dados.</returns>
        public static Result<T> Warning<T>(T data, string message = "")
          => new Result<T>().Warning(data, message);

        #endregion

        #region Result<T> - Falha

        /// <summary>
        /// Cria um resultado de falha com mensagem e opção de continuar.
        /// </summary>
        /// <typeparam name="T">O tipo dos dados retornados.</typeparam>
        /// <param name="message">A mensagem de erro. Padrão: string vazia.</param>
        /// <param name="canContinue">Indica se o fluxo pode continuar após a falha. Padrão: false.</param>
        /// <returns>Uma nova instância de <see cref="Result{T}"/> com falha.</returns>
        public static Result<T> Failure<T>(string message = "", bool canContinue = false)
            => new Result<T>().Failed(message, canContinue);

        /// <summary>
        /// Cria um resultado de falha com mensagem.
        /// </summary>
        /// <typeparam name="T">O tipo dos dados retornados.</typeparam>
        /// <param name="message">A mensagem de erro. Padrão: string vazia.</param>
        /// <returns>Uma nova instância de <see cref="Result{T}"/> com falha.</returns>
        public static Result<T> Failure<T>(string message = "")
           => new Result<T>().Failed(message);

        /// <summary>
        /// Cria um resultado de falha com mensagem e dados.
        /// </summary>
        /// <typeparam name="T">O tipo dos dados retornados.</typeparam>
        /// <param name="message">A mensagem de erro.</param>
        /// <param name="data">Os dados a serem retornados no resultado (mesmo em caso de falha).</param>
        /// <returns>Uma nova instância de <see cref="Result{T}"/> com falha e dados.</returns>
        public static Result<T> Failure<T>(string message, T data)
            => new Result<T>().Failed(message, data);

        /// <summary>
        /// Cria um resultado de falha sem dados tipados.
        /// </summary>
        /// <param name="message">A mensagem de erro. Padrão: string vazia.</param>
        /// <returns>Uma nova instância de <see cref="Result"/> com falha.</returns>
        public static Result Failure(string message = "")
            => new Result().Failed(message);

        /// <summary>
        /// Cria um resultado de falha com mensagem e exceção.
        /// </summary>
        /// <typeparam name="T">O tipo dos dados retornados.</typeparam>
        /// <param name="message">A mensagem de erro.</param>
        /// <param name="exception">A exceção associada à falha.</param>
        /// <returns>Uma nova instância de <see cref="Result{T}"/> com falha e exceção.</returns>
        public static Result<T> Failure<T>(string message, Exception exception)
            => new Result<T>().Failed(message, exception);

        /// <summary>
        /// Cria um resultado de falha sem dados tipados, com mensagem e exceção.
        /// </summary>
        /// <param name="message">A mensagem de erro.</param>
        /// <param name="exception">A exceção associada à falha.</param>
        /// <returns>Uma nova instância de <see cref="Result"/> com falha e exceção.</returns>
        public static Result Failure(string message, Exception exception)
            => new Result().Failed(message, exception);

        #endregion

        #region PublicResult - Sucesso

        /// <summary>
        /// Cria um resultado público de sucesso com dados tipados.
        /// </summary>
        /// <typeparam name="T">O tipo dos dados retornados.</typeparam>
        /// <param name="data">Os dados a serem retornados no resultado.</param>
        /// <returns>Uma nova instância de <see cref="PublicResult{T}"/> com sucesso e dados.</returns>
        public static PublicResult<T> PublicSuccess<T>(T data)
            => PublicResult<T>.From(Success(data));

        /// <summary>
        /// Cria um resultado público de sucesso com mensagem.
        /// </summary>
        /// <param name="message">A mensagem descritiva do sucesso. Pode ser null.</param>
        /// <returns>Uma nova instância de <see cref="PublicResult"/> com sucesso e mensagem.</returns>
        public static PublicResult PublicSuccess(string message = null)
            => PublicResult.From(Success(message));

        #endregion

        #region PublicResult - Falha

        /// <summary>
        /// Cria um resultado público de falha com mensagem.
        /// </summary>
        /// <typeparam name="T">O tipo dos dados retornados.</typeparam>
        /// <param name="message">A mensagem de erro. Padrão: string vazia.</param>
        /// <returns>Uma nova instância de <see cref="PublicResult{T}"/> com falha.</returns>
        public static PublicResult<T> PublicFailure<T>(string message = "")
            => PublicResult<T>.From(Failure<T>(message));

        /// <summary>
        /// Cria um resultado público de falha com mensagem e dados.
        /// </summary>
        /// <typeparam name="T">O tipo dos dados retornados.</typeparam>
        /// <param name="message">A mensagem de erro.</param>
        /// <param name="data">Os dados a serem retornados no resultado.</param>
        /// <returns>Uma nova instância de <see cref="PublicResult{T}"/> com falha e dados.</returns>
        public static PublicResult<T> PublicFailure<T>(string message, T data)
            => PublicResult<T>.From(Failure(message, data));

        /// <summary>
        /// Cria um resultado público de falha sem dados tipados.
        /// </summary>
        /// <param name="message">A mensagem de erro. Padrão: string vazia.</param>
        /// <returns>Uma nova instância de <see cref="PublicResult"/> com falha.</returns>
        public static PublicResult PublicFailure(string message = "")
            => PublicResult.From(Failure(message));

        /// <summary>
        /// Cria um resultado público de falha com mensagem e exceção.
        /// </summary>
        /// <typeparam name="T">O tipo dos dados retornados.</typeparam>
        /// <param name="message">A mensagem de erro.</param>
        /// <param name="ex">A exceção associada à falha.</param>
        /// <returns>Uma nova instância de <see cref="PublicResult{T}"/> com falha e exceção.</returns>
        public static PublicResult<T> PublicFailure<T>(string message, Exception ex)
            => PublicResult<T>.From(Failure<T>(message, ex));

        /// <summary>
        /// Cria um resultado público de falha sem dados tipados, com mensagem e exceção.
        /// </summary>
        /// <param name="message">A mensagem de erro.</param>
        /// <param name="ex">A exceção associada à falha.</param>
        /// <returns>Uma nova instância de <see cref="PublicResult"/> com falha e exceção.</returns>
        public static PublicResult PublicFailure(string message, Exception ex)
            => PublicResult.From(Failure(message, ex));

        #endregion
    }
}

