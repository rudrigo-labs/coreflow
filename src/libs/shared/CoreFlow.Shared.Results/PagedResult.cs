using CoreFlow.Shared.Results;

namespace CoreFlow.Shared.Results
{
    /// <summary>
    /// Representa um resultado paginado genérico com informações completas de paginação.
    /// Fornece propriedades calculadas para navegação e informações sobre a página atual.
    /// </summary>
    /// <typeparam name="T">O tipo dos itens contidos no resultado paginado.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Obtém ou define a coleção de itens da página atual.
        /// </summary>
        /// <value>Os itens da página atual. Inicializado como uma lista vazia.</value>
        public IEnumerable<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Obtém ou define o número da página atual (baseado em 1).
        /// </summary>
        /// <value>O número da página, onde 1 é a primeira página.</value>
        public int PageNumber { get; set; }

        /// <summary>
        /// Obtém ou define o tamanho da página (número de itens por página).
        /// </summary>
        /// <value>O número de itens por página.</value>
        public int PageSize { get; set; }

        /// <summary>
        /// Obtém ou define o total de registros disponíveis em todas as páginas.
        /// </summary>
        /// <value>O número total de registros.</value>
        public int TotalCount { get; set; }

        /// <summary>
        /// Obtém o número total de páginas disponíveis.
        /// </summary>
        /// <value>O número total de páginas, calculado como o teto de TotalCount / PageSize.</value>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// Obtém um valor que indica se existe uma página anterior.
        /// </summary>
        /// <value><c>true</c> se existe uma página anterior; caso contrário, <c>false</c>.</value>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Obtém um valor que indica se existe uma próxima página.
        /// </summary>
        /// <value><c>true</c> se existe uma próxima página; caso contrário, <c>false</c>.</value>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Obtém o índice do primeiro item na página atual (baseado em 1).
        /// </summary>
        /// <value>O índice do primeiro item (1-based) ou 0 se não houver itens.</value>
        public int FirstItemOnPage => TotalCount > 0 ? ((PageNumber - 1) * PageSize) + 1 : 0;

        /// <summary>
        /// Obtém o índice do último item na página atual (baseado em 1).
        /// </summary>
        /// <value>O índice do último item (1-based) na página atual.</value>
        public int LastItemOnPage => Math.Min(PageNumber * PageSize, TotalCount);

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PagedResult{T}"/>.
        /// </summary>
        public PagedResult()
        {
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PagedResult{T}"/> com os parâmetros especificados.
        /// </summary>
        /// <param name="items">A coleção de itens da página atual.</param>
        /// <param name="count">O número total de registros disponíveis.</param>
        /// <param name="pageNumber">O número da página atual (baseado em 1).</param>
        /// <param name="pageSize">O tamanho da página (número de itens por página).</param>
        public PagedResult(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        /// <summary>
        /// Cria um resultado paginado vazio com os parâmetros de paginação especificados.
        /// </summary>
        /// <param name="pageNumber">O número da página (padrão: 1).</param>
        /// <param name="pageSize">O tamanho da página (padrão: 10).</param>
        /// <returns>Uma nova instância de <see cref="PagedResult{T}"/> vazia.</returns>
        public static PagedResult<T> Empty(int pageNumber = 1, int pageSize = 10)
        {
            return new PagedResult<T>(new List<T>(), 0, pageNumber, pageSize);
        }

        /// <summary>
        /// Cria um resultado paginado a partir de itens e informações de paginação.
        /// </summary>
        /// <param name="items">A coleção de itens da página atual.</param>
        /// <param name="totalCount">O número total de registros disponíveis.</param>
        /// <param name="pageNumber">O número da página atual (baseado em 1).</param>
        /// <param name="pageSize">O tamanho da página (número de itens por página).</param>
        /// <returns>Uma nova instância de <see cref="PagedResult{T}"/> com os dados especificados.</returns>
        public static PagedResult<T> Create(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
        }
    }

}

