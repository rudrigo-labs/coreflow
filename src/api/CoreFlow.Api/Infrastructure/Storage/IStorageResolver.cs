using CoreFlow.Api.Infrastructure.Storage;

namespace CoreFlow.Api.Infrastructure.Storage
{
    /// <summary>
    /// Resolve um caminho físico com base em:
    /// - storageKey: chave configurada em "Storages" (ex.: site-data)
    /// - relativePath: caminho relativo escolhido pelo cliente (ex.: catalogo.json, a/b/c.txt)
    ///
    /// ESSENCIAL:
    /// - Impedir path traversal (../)
    /// - Garantir que o caminho final esteja dentro do root do storage
    /// </summary>
    public interface IStorageResolver
    {
        string Resolve(string storageKey, string relativePath);
    }

    /// <summary>
    /// Implementação simples baseada em IConfiguration.
    /// Espera ter algo como:
    ///   "Storages": {
    ///      "site-data": "/opt/projetos/nada-mais-importa/site/content/data"
    ///   }
    /// </summary>
    public sealed class StorageResolver : IStorageResolver
    {
        private readonly IConfiguration _cfg;

        public StorageResolver(IConfiguration cfg) => _cfg = cfg;

        public string Resolve(string storageKey, string relativePath)
        {
            var root = _cfg[$"Storages:{storageKey}"];
            if (string.IsNullOrWhiteSpace(root))
                throw new ArgumentException($"Storage inválido: {storageKey}");

            // Normaliza o path relativo (evita barra invertida e paths absolutos)
            relativePath = (relativePath ?? "")
                .Replace('\\', '/')
                .TrimStart('/');

            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Path é obrigatório.");

            // Monta full paths para validar "dentro do root"
            var fullRoot = Path.GetFullPath(root);
            var fullPath = Path.GetFullPath(Path.Combine(fullRoot, relativePath));

            // Segurança: impede sair do root via ../
            if (!fullPath.StartsWith(fullRoot, StringComparison.Ordinal))
                throw new InvalidOperationException("Path inválido (fora do storage).");

            // Garante diretório do arquivo
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            return fullPath;
        }
    }
}
