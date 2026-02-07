using CoreFlow.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreFlow.Shared.Results.Factories
{
    /// <summary>
    /// Extensões da factory para criar resultados de grid (<see cref="GridResult{T}"/>).
    /// Fornece métodos convenientes para criar grids com paginação e informações de contagem.
    /// </summary>
    public static partial class ResultFactory
    {
        /// <summary>
        /// Cria um resultado de sucesso para grid com itens e informações de paginação.
        /// Define flags de sucesso manualmente para manter o tipo GridResult durante todo o fluxo.
        /// </summary>
        /// <typeparam name="T">O tipo dos itens no grid.</typeparam>
        /// <param name="items">A coleção de itens a serem exibidos no grid.</param>
        /// <param name="total">O total de registros disponíveis (opcional).</param>
        /// <param name="page">O número da página atual, baseado em 1 (opcional).</param>
        /// <param name="pageSize">O tamanho da página (opcional).</param>
        /// <param name="message">A mensagem descritiva do resultado (opcional).</param>
        /// <returns>Uma nova instância de <see cref="GridResult{T}"/> com sucesso e dados configurados.</returns>
        public static GridResult<T> GridSuccess<T>(
            IEnumerable<T> items,
            int? total = null,
            int? page = null,
            int? pageSize = null,
            string message = null)
        {
            var grid = new GridResult<T>();

            // Dados + contagem (evita extensões do tipo base)
            grid.WithItems(items);

            // Total/RowCount
            var rowCount = grid.RowCount;
            if (total.HasValue)
                grid.WithCounts(rowCount, total);
            else
                grid.WithCounts(rowCount, null);

            // Paginação
            grid.WithPaging(page, pageSize);

            // Status OK
            grid.Succeeded = true;
            grid.Found = rowCount > 0;
            grid.StatusCode = System.Net.HttpStatusCode.OK;
            if (!string.IsNullOrWhiteSpace(message))
                grid.Message = message;

            return grid.SyncMetadata();
        }

        /// <summary>
        /// Cria um resultado de sucesso para grid com paginação obrigatória (valores não-nulos).
        /// </summary>
        /// <typeparam name="T">O tipo dos itens no grid.</typeparam>
        /// <param name="items">A coleção de itens a serem exibidos no grid.</param>
        /// <param name="total">O total de registros disponíveis.</param>
        /// <param name="page">O número da página atual, baseado em 1.</param>
        /// <param name="pageSize">O tamanho da página.</param>
        /// <param name="message">A mensagem descritiva do resultado (opcional).</param>
        /// <returns>Uma nova instância de <see cref="GridResult{T}"/> com sucesso e dados configurados.</returns>
        public static GridResult<T> GridSuccess<T>(
            IEnumerable<T> items,
            int total,
            int page,
            int pageSize,
            string message = null)
        {
            return GridSuccess(items, (int?)total, (int?)page, (int?)pageSize, message);
        }

        /// <summary>
        /// Cria um resultado de falha para grid com dados vazios e flags consistentes.
        /// </summary>
        /// <typeparam name="T">O tipo dos itens no grid.</typeparam>
        /// <param name="message">A mensagem de erro. Padrão: string vazia.</param>
        /// <param name="canContinue">Indica se o fluxo pode continuar após a falha. Padrão: false.</param>
        /// <param name="ex">A exceção associada à falha (opcional).</param>
        /// <returns>Uma nova instância de <see cref="GridResult{T}"/> com falha e dados vazios.</returns>
        public static GridResult<T> GridFailure<T>(
            string message = "",
            bool canContinue = false,
            Exception ex = null)
        {
            var grid = new GridResult<T>();
            grid.Data = new List<T>(); // vazio
            grid.RowCount = 0;
            grid.Total = 0;
            grid.Page = 1;
            grid.PageSize = 0;
            grid.HasMore = false;

            grid.Succeeded = false;
            grid.CanContinue = canContinue;
            grid.StatusCode = System.Net.HttpStatusCode.BadRequest;
            grid.Message = message;
            grid.Exception = ex;

            return grid.SyncMetadata();
        }

        /// <summary>
        /// Cria um resultado de "não encontrado" para grid: operação executada com sucesso, mas sem registros.
        /// </summary>
        /// <typeparam name="T">O tipo dos itens no grid.</typeparam>
        /// <param name="message">A mensagem descritiva (opcional).</param>
        /// <returns>Uma nova instância de <see cref="GridResult{T}"/> com sucesso mas sem dados.</returns>
        public static GridResult<T> GridNotFound<T>(string message = null)
        {
            var grid = new GridResult<T>();
            grid.Data = new List<T>();
            grid.RowCount = 0;
            grid.Total = 0;
            grid.Page = 1;
            grid.PageSize = 0;
            grid.HasMore = false;

            grid.Succeeded = true;  // operação executada
            grid.Found = false; // porém sem registros
            grid.StatusCode = System.Net.HttpStatusCode.OK;
            grid.Message = message;

            return grid.SyncMetadata();
        }

        /// <summary>
        /// Cria um resultado público de sucesso para grid (contrato público simplificado).
        /// </summary>
        /// <typeparam name="T">O tipo dos itens no grid.</typeparam>
        /// <param name="items">A coleção de itens a serem exibidos no grid.</param>
        /// <param name="total">O total de registros disponíveis (opcional).</param>
        /// <param name="page">O número da página atual, baseado em 1 (opcional).</param>
        /// <param name="pageSize">O tamanho da página (opcional).</param>
        /// <param name="message">A mensagem descritiva do resultado (opcional).</param>
        /// <returns>Uma nova instância de <see cref="PublicResult{IEnumerable{T}}"/> com sucesso e dados configurados.</returns>
        public static PublicResult<IEnumerable<T>> PublicGridSuccess<T>(
            IEnumerable<T> items,
            int? total = null,
            int? page = null,
            int? pageSize = null,
            string message = null)
        {
            var grid = GridSuccess(items, total, page, pageSize, message);
            return PublicResult<IEnumerable<T>>.From(grid);
        }

        /// <summary>
        /// Cria um resultado público de sucesso para grid com paginação obrigatória (valores não-nulos).
        /// </summary>
        /// <typeparam name="T">O tipo dos itens no grid.</typeparam>
        /// <param name="items">A coleção de itens a serem exibidos no grid.</param>
        /// <param name="total">O total de registros disponíveis.</param>
        /// <param name="page">O número da página atual, baseado em 1.</param>
        /// <param name="pageSize">O tamanho da página.</param>
        /// <param name="message">A mensagem descritiva do resultado (opcional).</param>
        /// <returns>Uma nova instância de <see cref="PublicResult{IEnumerable{T}}"/> com sucesso e dados configurados.</returns>
        public static PublicResult<IEnumerable<T>> PublicGridSuccess<T>(
            IEnumerable<T> items,
            int total,
            int page,
            int pageSize,
            string message = null)
        {
            var grid = GridSuccess(items, total, page, pageSize, message);
            return PublicResult<IEnumerable<T>>.From(grid);
        }

        /// <summary>
        /// Cria um resultado público de falha para grid.
        /// </summary>
        /// <typeparam name="T">O tipo dos itens no grid.</typeparam>
        /// <param name="message">A mensagem de erro. Padrão: string vazia.</param>
        /// <param name="canContinue">Indica se o fluxo pode continuar após a falha. Padrão: false.</param>
        /// <param name="ex">A exceção associada à falha (opcional).</param>
        /// <returns>Uma nova instância de <see cref="PublicResult{IEnumerable{T}}"/> com falha.</returns>
        public static PublicResult<IEnumerable<T>> PublicGridFailure<T>(
            string message = "",
            bool canContinue = false,
            Exception ex = null)
        {
            var grid = GridFailure<T>(message, canContinue, ex);
            return PublicResult<IEnumerable<T>>.From(grid);
        }

        /// <summary>
        /// Cria um resultado público de "não encontrado" para grid.
        /// </summary>
        /// <typeparam name="T">O tipo dos itens no grid.</typeparam>
        /// <param name="message">A mensagem descritiva (opcional).</param>
        /// <returns>Uma nova instância de <see cref="PublicResult{IEnumerable{T}}"/> com sucesso mas sem dados.</returns>
        public static PublicResult<IEnumerable<T>> PublicGridNotFound<T>(string message = null)
        {
            var grid = GridNotFound<T>(message);
            return PublicResult<IEnumerable<T>>.From(grid);
        }
    }
}

