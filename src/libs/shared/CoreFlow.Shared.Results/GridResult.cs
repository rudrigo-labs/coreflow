using CoreFlow.Shared.Results;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CoreFlow.Shared.Results
{
    /// <summary>
    /// Resultado simples para dados tabulares (grids).
    /// Herda de Result<IEnumerable<T>> para manter compatibilidade com as extensões existentes.
    /// </summary>
    public class GridResult<T> : Result<IEnumerable<T>>
    {
        /// <summary>
        /// Obtém ou define o número de linhas retornadas nesta página.
        /// </summary>
        /// <value>O número de itens retornados na página atual.</value>
        public int RowCount { get; set; }

        /// <summary>
        /// Obtém ou define o total de registros na consulta completa (quando paginado).
        /// </summary>
        /// <value>O total de registros disponíveis. Null quando não paginado ou quando o total não é conhecido.</value>
        public int? Total { get; set; }

        /// <summary>
        /// Obtém ou define o número da página atual (baseado em 1).
        /// </summary>
        /// <value>O número da página, onde 1 é a primeira página. Null quando não paginado.</value>
        public int? Page { get; set; }

        /// <summary>
        /// Obtém ou define o número de itens por página.
        /// </summary>
        /// <value>O tamanho da página. Null quando não paginado.</value>
        public int? PageSize { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se há mais páginas disponíveis.
        /// </summary>
        /// <value><c>true</c> se há mais páginas; <c>false</c> se esta é a última página; null quando não pode ser determinado.</value>
        public bool? HasMore { get; set; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="GridResult{T}"/>.
        /// </summary>
        public GridResult() : base() { }

        /// <summary>
        /// Define os itens do grid e tenta inferir o RowCount a partir da coleção.
        /// </summary>
        /// <param name="items">A coleção de itens a ser definida como dados do grid.</param>
        /// <returns>Esta instância para permitir encadeamento de chamadas.</returns>
        public GridResult<T> WithItems(IEnumerable<T> items)
        {
            this.Data = items;
            this.RowCount = InferCount(items);
            return SyncMetadata();
        }

        /// <summary>
        /// Define o número de linhas e o total de registros.
        /// </summary>
        /// <param name="rowCount">O número de linhas retornadas nesta página.</param>
        /// <param name="total">O total de registros na consulta completa. Opcional.</param>
        /// <returns>Esta instância para permitir encadeamento de chamadas.</returns>
        public GridResult<T> WithCounts(int rowCount, int? total = null)
        {
            this.RowCount = rowCount;
            this.Total = total;
            return SyncMetadata();
        }

        /// <summary>
        /// Define os parâmetros de paginação (número da página é baseado em 1).
        /// </summary>
        /// <param name="page">O número da página (baseado em 1).</param>
        /// <param name="pageSize">O tamanho da página (número de itens por página).</param>
        /// <returns>Esta instância para permitir encadeamento de chamadas.</returns>
        public GridResult<T> WithPaging(int? page, int? pageSize)
        {
            this.Page = page;
            this.PageSize = pageSize;
            // calcula HasMore quando possível (assumindo página 1-based)
            if (this.Total.HasValue && page.HasValue && pageSize.HasValue)
            {
                var shown = Math.Max(0, (page.Value - 1) * pageSize.Value) + Math.Min(pageSize.Value, this.RowCount);
                this.HasMore = shown < this.Total.Value;
            }
            return SyncMetadata();
        }

        /// <summary>
        /// Sincroniza as propriedades de grid no dicionário Metadata do resultado.
        /// </summary>
        /// <returns>Esta instância para permitir encadeamento de chamadas.</returns>
        public GridResult<T> SyncMetadata()
        {
            this.AddMetadata("rowCount", this.RowCount);
            if (this.Total.HasValue) this.AddMetadata("total", this.Total.Value);
            if (this.Page.HasValue) this.AddMetadata("page", this.Page.Value);
            if (this.PageSize.HasValue) this.AddMetadata("pageSize", this.PageSize.Value);
            if (this.HasMore.HasValue) this.AddMetadata("hasMore", this.HasMore.Value);
            return this;
        }

        /// <summary>
        /// Infere o número de itens na coleção de forma eficiente.
        /// </summary>
        /// <param name="items">A coleção de itens.</param>
        /// <returns>O número de itens na coleção. Retorna 0 se a coleção for null.</returns>
        private static int InferCount(IEnumerable<T> items)
        {
            if (items == null) return 0;

            var colT = items as ICollection<T>;
            if (colT != null) return colT.Count;

            var col = items as ICollection;
            if (col != null) return col.Count;

            // fallback (pode enumerar a fonte)
            return items.Count();
        }
    }
}

