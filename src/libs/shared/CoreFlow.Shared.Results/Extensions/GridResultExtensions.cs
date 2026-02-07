using CoreFlow.Shared.Results;
using CoreFlow.Shared.Results.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CoreFlow.Shared.Results.Extensions
{
    /// <summary>
    /// Extensões para manipulação de resultados de grid (<see cref="GridResult{T}"/>).
    /// Fornece métodos para converter coleções em grids, configurar paginação e sincronizar metadados.
    /// </summary>
    public static class GridResultExtensions
    {
        #region Paging Metadata em Result/Result<T>

        /// <summary>
        /// Adiciona o número de linhas aos metadados do resultado.
        /// </summary>
        /// <param name="result">O resultado a ser modificado.</param>
        /// <param name="count">O número de linhas a ser adicionado aos metadados.</param>
        /// <returns>O resultado modificado para permitir encadeamento de chamadas.</returns>
        public static Result WithRowCount(this Result result, int count)
        {
            result.AddMetadata("rowCount", count);
            return result;
        }

        public static Result<T> WithRowCount<T>(this Result<T> result, int count)
        {
            result.AddMetadata("rowCount", count);
            return result;
        }

        /// <summary>
        /// Infere o número de linhas a partir dos dados do resultado.
        /// Cuidado: pode enumerar IEnumerable não materializado.
        /// </summary>
        /// <typeparam name="T">O tipo dos dados no resultado.</typeparam>
        /// <param name="result">O resultado do qual inferir o número de linhas.</param>
        /// <returns>O resultado modificado com RowCount adicionado aos metadados, ou null se o resultado for null.</returns>
        public static Result<T> WithRowCountFromData<T>(this Result<T> result)
        {
            if (result == null) return null;

            var dataObj = (object)result.Data;
            if (dataObj == null) return result;

            var col = dataObj as ICollection;
            if (col != null)
            {
                result.AddMetadata("rowCount", col.Count);
                return result;
            }

            var en = dataObj as System.Collections.IEnumerable;
            if (en != null)
            {
                int count = 0;
                var it = en.GetEnumerator();
                try { while (it.MoveNext()) count++; }
                finally { var d = it as IDisposable; if (d != null) d.Dispose(); }
                result.AddMetadata("rowCount", count);
            }
            return result;
        }
        #endregion

        #region GridResult<T> Builders

        /// <summary>
        /// Converte uma coleção de itens em um <see cref="GridResult{T}"/> com informações de paginação.
        /// </summary>
        /// <typeparam name="T">O tipo dos itens na coleção.</typeparam>
        /// <param name="items">A coleção de itens a ser convertida.</param>
        /// <param name="total">O total de registros disponíveis (opcional).</param>
        /// <param name="page">O número da página atual, baseado em 1 (opcional).</param>
        /// <param name="pageSize">O tamanho da página (opcional).</param>
        /// <param name="message">A mensagem descritiva do resultado (opcional).</param>
        /// <returns>Uma nova instância de <see cref="GridResult{T}"/> configurada com os parâmetros fornecidos.</returns>
        public static GridResult<T> ToGridResult<T>(
            this IEnumerable<T> items,
            int? total = null,
            int? page = null,
            int? pageSize = null,
            string message = null)
        {
            var grid = new GridResult<T>();

            // Dados + contagem
            grid.Data = items;
            grid.RowCount = items.SafeCount();

            // Totais/paginação
            grid.Total = total;
            grid.Page = page;
            grid.PageSize = pageSize;
            if (grid.Total.HasValue && grid.Page.HasValue && grid.PageSize.HasValue)
            {
                var shownUntilPrev = Math.Max(0, (grid.Page.Value - 1) * grid.PageSize.Value);
                var shownThisPage = Math.Min(grid.PageSize.Value, grid.RowCount);
                grid.HasMore = shownUntilPrev + shownThisPage < grid.Total.Value;
            }

            // Status
            grid.Succeeded = true;
            grid.Found = grid.RowCount > 0;
            grid.StatusCode = HttpStatusCode.OK;
            if (!string.IsNullOrWhiteSpace(message))
                grid.Message = message;

            return grid.SyncMetadata();
        }

        public static GridResult<T> ToGridResult<T>(
            this IEnumerable<T> items,
            int total,
            int page,
            int pageSize,
            string message = null)
        {
            return items.ToGridResult(total, (int?)page, (int?)pageSize, message);
        }

        public static PublicResult<IEnumerable<T>> ToPublicGridResult<T>(
            this IEnumerable<T> items,
            int? total = null,
            int? page = null,
            int? pageSize = null,
            string message = null)
        {
            var grid = items.ToGridResult(total, page, pageSize, message);
            return PublicResult<IEnumerable<T>>.From(grid);
        }
        #endregion

        #region GridResult<T> Mutators
        public static GridResult<T> WithRowCount<T>(this GridResult<T> grid, int rowCount)
        {
            grid.RowCount = rowCount;
            return grid.SyncMetadata();
        }

        public static GridResult<T> WithTotal<T>(this GridResult<T> grid, int? total)
        {
            grid.Total = total;
            return grid.SyncMetadata();
        }

        public static GridResult<T> WithGridPaging<T>(this GridResult<T> grid, int? page, int? pageSize)
        {
            grid.Page = page;
            grid.PageSize = pageSize;

            if (grid.Total.HasValue && page.HasValue && pageSize.HasValue)
            {
                var shownUntilPrev = Math.Max(0, (page.Value - 1) * pageSize.Value);
                var shownThisPage = Math.Min(pageSize.Value, grid.RowCount);
                grid.HasMore = shownUntilPrev + shownThisPage < grid.Total.Value;
            }
            return grid.SyncMetadata();
        }

        public static GridResult<T> SyncMetadata<T>(this GridResult<T> grid)
        {
            grid.AddMetadata("rowCount", grid.RowCount);
            if (grid.Total.HasValue) grid.AddMetadata("total", grid.Total.Value);
            if (grid.Page.HasValue) grid.AddMetadata("page", grid.Page.Value);
            if (grid.PageSize.HasValue) grid.AddMetadata("pageSize", grid.PageSize.Value);
            if (grid.HasMore.HasValue) grid.AddMetadata("hasMore", grid.HasMore.Value);
            return grid;
        }
        #endregion

        #region Utils

        /// <summary>
        /// Conta os itens de uma coleção de forma segura e eficiente.
        /// Tenta usar Count() quando disponível, evitando enumerar coleções não materializadas quando possível.
        /// </summary>
        /// <typeparam name="T">O tipo dos itens na coleção.</typeparam>
        /// <param name="items">A coleção a ser contada.</param>
        /// <returns>O número de itens na coleção. Retorna 0 se a coleção for null.</returns>
        public static int SafeCount<T>(this IEnumerable<T> items)
        {
            if (items == null) return 0;

            var colT = items as ICollection<T>;
            if (colT != null) return colT.Count;

            var col = items as ICollection;
            if (col != null) return col.Count;

            int count = 0;
            var en = items.GetEnumerator();
            try { while (en.MoveNext()) count++; }
            finally { var d = en as IDisposable; if (d != null) d.Dispose(); }
            return count;
        }
        #endregion
    }
}

