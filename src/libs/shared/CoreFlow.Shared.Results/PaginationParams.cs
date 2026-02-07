namespace CoreFlow.Shared.Results
{
    /// <summary>
    /// Parâmetros de paginação reutilizáveis para consultas paginadas.
    /// Fornece validação automática e propriedades calculadas para Skip e Take.
    /// </summary>
    public class PaginationParams
    {
        /// <summary>
        /// Tamanho máximo permitido para uma página.
        /// </summary>
        private const int MaxPageSize = 100;

        /// <summary>
        /// Campo privado para armazenar o tamanho da página.
        /// </summary>
        private int _pageSize = 10;

        /// <summary>
        /// Obtém ou define o número da página atual (baseado em 1).
        /// </summary>
        /// <value>O número da página, onde 1 é a primeira página. Valor padrão: 1.</value>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Obtém ou define o tamanho da página (número de itens por página).
        /// O valor é automaticamente limitado ao <see cref="MaxPageSize"/>.
        /// </summary>
        /// <value>O tamanho da página. Valor padrão: 10. Máximo: 100.</value>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        /// <summary>
        /// Obtém o número de itens a serem ignorados (calculado para consultas SQL/LINQ).
        /// </summary>
        /// <value>O número de itens a serem ignorados antes de começar a retornar resultados.</value>
        public int Skip => (PageNumber - 1) * PageSize;

        /// <summary>
        /// Obtém o número de itens a serem retornados (equivalente ao PageSize).
        /// </summary>
        /// <value>O número de itens a serem retornados.</value>
        public int Take => PageSize;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PaginationParams"/> com valores padrão.
        /// </summary>
        public PaginationParams()
        {
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PaginationParams"/> com os parâmetros especificados.
        /// </summary>
        /// <param name="pageNumber">O número da página (baseado em 1). Se menor ou igual a 0, será definido como 1.</param>
        /// <param name="pageSize">O tamanho da página. Será limitado automaticamente ao máximo permitido.</param>
        public PaginationParams(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber > 0 ? pageNumber : 1;
            PageSize = pageSize;
        }

        /// <summary>
        /// Valida os parâmetros de paginação.
        /// </summary>
        /// <returns><c>true</c> se os parâmetros são válidos; caso contrário, <c>false</c>.</returns>
        /// <remarks>
        /// Um parâmetro é considerado válido quando:
        /// - PageNumber é maior que 0
        /// - PageSize é maior que 0 e menor ou igual a MaxPageSize
        /// </remarks>
        public bool IsValid()
        {
            return PageNumber > 0 && PageSize > 0 && PageSize <= MaxPageSize;
        }
    }

}

