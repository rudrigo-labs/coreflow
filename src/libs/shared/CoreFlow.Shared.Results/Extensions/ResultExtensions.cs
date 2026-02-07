using CoreFlow.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoreFlow.Shared.Results.Extensions
{
    /// <summary>
    /// Extensões para manipulação de instâncias de <see cref="Result"/> e <see cref="Result{T}"/>.
    /// Fornece métodos fluent para configurar status, mensagens, erros e metadados.
    /// </summary>
    public static class ResultExtensions
    {
        #region Falhas (Result)

        public static Result Failed(this Result result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.WithStatus(false, HttpStatusCode.BadRequest);
        }

        public static Result Failed(this Result result, string message)
            => result.Failed().WithMessage(message);

        public static Result Failed(this Result result, string message, Exception? exception)
            => result.Failed(message).WithException(exception);

        public static Result NotFound(this Result result, string? message = null)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.WithStatus(false, HttpStatusCode.NotFound).WithMessage(message);
        }

        public static Result InternalServerError(this Result result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.WithStatus(false, HttpStatusCode.InternalServerError);
        }

        public static Result Unauthorized(this Result result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.WithStatus(false, HttpStatusCode.Unauthorized);
        }

        public static Result HandleServiceError(this Result result, string errorMessage, Exception? ex = null)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.Failed(errorMessage).WithException(ex);
        }

        #endregion

        #region Sucesso/Aviso (Result)

        public static Result Successful(this Result result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.WithStatus(true, HttpStatusCode.OK).RegisterFound();
        }

        public static Result Successful(this Result result, string message)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.Successful().WithMessage(message);
        }

        #endregion

        #region Mensagens/Erros (Result)

        public static Result WithMessage(this Result result, string? message)
        {
            ArgumentNullException.ThrowIfNull(result);
            result.Message = message;
            return result;
        }

        public static Result WithError(this Result result, string? error)
        {
            ArgumentNullException.ThrowIfNull(result);

            if (!string.IsNullOrWhiteSpace(error))
                result.AddError(error);

            return result;
        }

        public static Result WithError(this Result result, string error, string code)
        {
            ArgumentNullException.ThrowIfNull(result);
            result.AddError(error, code);
            return result;
        }

        public static Result WithErrors(this Result result, IEnumerable<IResultError> errors)
        {
            ArgumentNullException.ThrowIfNull(result);
            result.AddErrors(errors);
            return result;
        }

        #endregion

        #region Estado/Metadados/Exceção (Result)

        public static Result WithException(this Result result, Exception? exception)
        {
            ArgumentNullException.ThrowIfNull(result);
            result.Exception = exception;
            return result;
        }

        public static Result RegisterFound(this Result result)
        {
            ArgumentNullException.ThrowIfNull(result);
            result.Found = true;
            return result;
        }

        public static Result RegisterNotFound(this Result result)
        {
            ArgumentNullException.ThrowIfNull(result);
            result.Found = false;
            return result;
        }

        #endregion

        #region Falhas (Result<T>)

        public static Result<T> Failed<T>(this Result<T> result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.WithStatus(false, HttpStatusCode.BadRequest);
        }

        public static Result<T> Failed<T>(this Result<T> result, string message)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.Failed().WithMessage(message);
        }

        public static Result<T> Failed<T>(this Result<T> result, string message, bool canContinue = true)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.Failed(message).CanContinue(canContinue);
        }

        public static Result<T> Failed<T>(this Result<T> result, string message, T data)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.Failed(message).WithData(data);
        }

        public static Result<T> Failed<T>(this Result<T> result, string message, Exception? ex)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.Failed(message).WithException(ex);
        }

        public static Result<T> NotFound<T>(this Result<T> result, string? message = null)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.WithStatus(false, HttpStatusCode.NotFound).WithMessage(message);
        }

        public static Result<T> InternalServerError<T>(this Result<T> result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.WithStatus(false, HttpStatusCode.InternalServerError);
        }

        public static Result<T> Unauthorized<T>(this Result<T> result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.WithStatus(false, HttpStatusCode.Unauthorized);
        }

        public static Result<T> HandleServiceError<T>(this Result<T> result, string errorMessage, Exception? ex = null)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.Failed(errorMessage).WithException(ex).WithData(default!);
        }

        #endregion

        #region Sucesso/Aviso (Result<T>)

        public static Result<T> Successful<T>(this Result<T> result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.WithStatus(true, HttpStatusCode.OK).RegisterFound();
        }

        

        public static Result Created(this Result result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.WithStatus(true, HttpStatusCode.Created).RegisterFound();
        }

        public static Result<T> Created<T>(this Result<T> result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.WithStatus(true, HttpStatusCode.Created).RegisterFound();
        }
public static Result<T> Successful<T>(this Result<T> result, T data)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.Successful().WithData(data);
        }

        public static Result<T> Warning<T>(this Result<T> result, string message)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.WithStatus(true, HttpStatusCode.NoContent).WithMessage(message);
        }

        public static Result<T> Warning<T>(this Result<T> result, T data, string message)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.WithStatus(true, HttpStatusCode.NoContent).WithMessage(message).WithData(data);
        }

        #endregion

        #region Mensagens/Erros/Dados (Result<T>)

        public static Result<T> WithMessage<T>(this Result<T> result, string? message)
        {
            ArgumentNullException.ThrowIfNull(result);
            result.Message = message;
            return result;
        }

        public static Result<T> WithData<T>(this Result<T> result, T data)
        {
            ArgumentNullException.ThrowIfNull(result);
            result.Data = data;
            return result;
        }

        public static Result<T> WithError<T>(this Result<T> result, string? error)
        {
            ArgumentNullException.ThrowIfNull(result);

            if (!string.IsNullOrWhiteSpace(error))
                result.AddError(error);

            return result;
        }

        public static Result<T> WithError<T>(this Result<T> result, string error, string code)
        {
            ArgumentNullException.ThrowIfNull(result);
            result.AddError(error, code);
            return result;
        }

        public static Result<T> WithErrors<T>(this Result<T> result, IEnumerable<IResultError> errors)
        {
            ArgumentNullException.ThrowIfNull(result);
            result.AddErrors(errors);
            return result;
        }

        public static Result<T> WithException<T>(this Result<T> result, Exception? exception)
        {
            ArgumentNullException.ThrowIfNull(result);
            result.Exception = exception;
            return result;
        }

        #endregion

        #region Controle e Marcação (Result<T>)

        public static Result<T> CanContinue<T>(this Result<T> result, bool canContinue)
        {
            ArgumentNullException.ThrowIfNull(result);
            result.CanContinue = canContinue;
            return result;
        }

        public static Result<T> RegisterFound<T>(this Result<T> result)
        {
            ArgumentNullException.ThrowIfNull(result);
            result.Found = true;
            return result;
        }

        public static Result<T> RegisterNotFound<T>(this Result<T> result)
        {
            ArgumentNullException.ThrowIfNull(result);
            result.Found = false;
            return result;
        }

        #endregion

        #region Utilidades

        public static string GetMessageOrError(this IResult result)
        {
            ArgumentNullException.ThrowIfNull(result);

            if (result.Succeeded)
                return string.Empty;

            var builder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(result.Message))
                builder.AppendLine(result.Message);

            if (result.Errors != null && result.Errors.Any())
                builder.AppendJoin(" | ", result.Errors.Where(e => e != null).Select(x => x.Error ?? string.Empty))
                       .AppendLine();

            if (result is Result r && r.Exception != null)
                builder.AppendLine(r.Exception.Message);

            return builder.ToString();
        }

        public static string GetMessageOrErrorToJson(this IResult result, bool includeException = false)
        {
            ArgumentNullException.ThrowIfNull(result);
            return GetMessageOrErrorToJson(result, includeException, null);
        }

        public static string GetMessageOrErrorToJson(
            this IResult result,
            bool includeException,
            IEnumerable<string>? metadataKeysToInclude)
        {
            ArgumentNullException.ThrowIfNull(result);

            // Erros -> lista de strings (ou null se vazio)
            List<string>? errorsList = null;
            if (result.Errors != null)
            {
                errorsList = result.Errors
                    .Where(e => e != null && !string.IsNullOrWhiteSpace(e.Error))
                    .Select(e => e.Error!)
                    .ToList();

                if (errorsList.Count == 0)
                    errorsList = null;
            }

            // Data (se o runtime type tiver "Data")
            object? dataValue = null;
            var dataProp = result.GetType().GetProperty("Data");
            if (dataProp != null && dataProp.CanRead)
                dataValue = dataProp.GetValue(result, null);

            // Campos extras (apenas em Result/Result<T>)
            var r = result as Result;
            HttpStatusCode? statusCode = r != null ? (HttpStatusCode?)r.StatusCode : null;
            bool? found = r != null ? (bool?)r.Found : null;
            bool? canContinue = r != null ? (bool?)r.CanContinue : null;

            Dictionary<string, object>? metadata = null;
            if (r != null)
            {
                if (metadataKeysToInclude == null)
                {
                    metadata = r.Metadata;
                }
                else if (r.Metadata != null)
                {
                    metadata = new Dictionary<string, object>();
                    foreach (var key in metadataKeysToInclude)
                    {
                        if (string.IsNullOrWhiteSpace(key)) continue;

                        if (r.Metadata.TryGetValue(key, out var val))
                            metadata[key] = val;
                    }

                    if (metadata.Count == 0)
                        metadata = null;
                }
            }

            // Exceção (somente se solicitado)
            object? exceptionDto = null;
            if (includeException && r?.Exception != null)
            {
                var ex = r.Exception;
                exceptionDto = new
                {
                    Type = ex.GetType().FullName,
                    ex.Message,
                    ex.Source,
                    ex.StackTrace
                };
            }

            var dto = new
            {
                result.Succeeded,
                Message = string.IsNullOrWhiteSpace(result.Message) ? null : result.Message,
                Errors = errorsList,
                Data = dataValue,
                StatusCode = statusCode,
                Found = found,
                CanContinue = canContinue,
                Metadata = metadata,
                Exception = exceptionDto
            };

            return JsonSerializer.Serialize(
                dto,
                new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = false
                });
        }

        #endregion

        #region Helpers

        private static T WithStatus<T>(this T result, bool succeeded, HttpStatusCode statusCode) where T : Result
        {
            ArgumentNullException.ThrowIfNull(result);
            result.Succeeded = succeeded;
            result.StatusCode = statusCode;
            return result;
        }

        /// <summary>
        /// Converte Result&lt;TIn&gt; em Result&lt;TOut&gt; preservando status/erros/metadados e projetando Data.
        /// </summary>
        public static Result<TOut> MapResult<TIn, TOut>(this Result<TIn> input, Func<TIn, TOut> mapper)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentNullException.ThrowIfNull(mapper);

            var output = new Result<TOut>
            {
                Succeeded = input.Succeeded,
                StatusCode = input.StatusCode,
                Message = input.Message,
                Found = input.Found,
                CanContinue = input.CanContinue,
                Exception = input.Exception
            };

            output.AddErrors(input.Errors);

            if (input.Succeeded && input.Data != null)
                output.Data = mapper(input.Data);

            return output;
        }

        #endregion
    }
}
