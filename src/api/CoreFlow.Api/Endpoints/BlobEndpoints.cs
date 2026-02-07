using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using CoreFlow.Api.Infrastructure.Storage;

namespace CoreFlow.Api.Endpoints
{
    public static class BlobEndpoints
    {
        /// <summary>
        /// Endpoints genéricos de Blob (upload e download).
        ///
        /// CONCEITO GERAL
        /// ---------------
        /// - A API conhece os caminhos físicos reais (config "Storages" no appsettings).
        /// - O CLIENTE informa:
        ///     - {storage}: qual raiz usar (chave do storage)
        ///     - {path}: caminho relativo + nome do arquivo (catch-all)
        ///
        /// Exemplo:
        ///   POST /api/blobs/site-data/catalogo.json
        ///
        /// O servidor resolve isso internamente para:
        ///   /opt/projetos/nada-mais-importa/site/content/data/catalogo.json
        ///
        /// Isso mantém:
        /// - endpoint genérico
        /// - segurança (sem path traversal)
        /// - infra desacoplada do cliente
        /// </summary>
        public static void MapBlobEndpoints(this WebApplication app)
        {
            // Grupo base de blobs
            var group = app.MapGroup("/api/blobs")
                .WithTags("Blobs");

            // =========================================================
            // UPLOAD VIA MULTIPART (IFormFile)
            // =========================================================
            //
            // Rota:
            //   POST /api/blobs/{storage}/{**path}?overwrite=true|false
            //
            // Exemplos:
            //   POST /api/blobs/site-data/catalogo.json
            //   POST /api/blobs/site-data/a/b/c/arquivo.txt?overwrite=false
            //
            // Regras:
            // - overwrite:
            //     - true  (default) → sobrescreve se existir
            //     - false           → retorna 409 se o arquivo já existir
            //
            group.MapPost("/{storage}/{**path}", async (
                string storage,              // chave do storage (ex.: site-data)
                string path,                 // caminho relativo + nome do arquivo
                bool? overwrite,             // parâmetro opcional via querystring
                IFormFile file,              // arquivo enviado via multipart
                IStorageResolver resolver,   // resolve caminho físico com segurança
                CancellationToken ct) =>
            {
                // Validação básica do arquivo
                if (file is null || file.Length == 0)
                    return Results.BadRequest("Arquivo 'file' é obrigatório.");

                // Limite de tamanho (defensivo)
                const long maxBytes = 5 * 1024 * 1024; // 5 MB
                if (file.Length > maxBytes)
                    return Results.BadRequest("Arquivo muito grande.");

                // Resolve o caminho físico final (storage + path)
                // Aqui ocorre a validação contra "../" e paths inválidos
                string targetPath;
                try
                {
                    targetPath = resolver.Resolve(storage, path);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }

                // Controle explícito de sobrescrita
                var canOverwrite = overwrite ?? true; // default: true

                // Se overwrite=false e o arquivo já existir → conflito
                if (!canOverwrite && File.Exists(targetPath))
                    return Results.Conflict("Arquivo já existe e overwrite=false.");

                // Escrita atômica:
                // grava primeiro em .tmp e depois faz o swap
                var tmpPath = targetPath + ".tmp";

                try
                {
                    // Copia o conteúdo do arquivo recebido para o arquivo temporário
                    await using (var input = file.OpenReadStream())
                    await using (var tmp = new FileStream(
                        tmpPath,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None,
                        bufferSize: 64 * 1024,
                        useAsync: true))
                    {
                        await input.CopyToAsync(tmp, ct);
                        await tmp.FlushAsync(ct);
                    }

                    // Swap atômico
                    File.Move(tmpPath, targetPath, overwrite: canOverwrite);

                    return Results.Ok(new
                    {
                        storage,
                        path,
                        overwrite = canOverwrite,
                        savedAs = targetPath,
                        bytes = file.Length
                    });
                }
                catch
                {
                    // Limpeza defensiva
                    if (File.Exists(tmpPath))
                        File.Delete(tmpPath);

                    return Results.StatusCode(StatusCodes.Status500InternalServerError);
                }
            })
            .DisableAntiforgery()
            .WithName("UploadBlobMultipart")
            .WithSummary("Upload genérico de blob via multipart")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError)
            .Accepts<IFormFile>("multipart/form-data");

            // =========================================================
            // UPLOAD VIA RAW BODY (Stream)
            // =========================================================
            //
            // Rota:
            //   POST /api/blobs/raw/{storage}/{**path}?overwrite=true|false
            //
            // IMPORTANTE:
            // - O catch-all {**path} PRECISA ser o último segmento da rota
            //
            group.MapPost("/raw/{storage}/{**path}", async (
                string storage,
                string path,
                bool? overwrite,
                Stream body,                 // corpo cru da requisição
                HttpRequest request,
                IStorageResolver resolver,
                CancellationToken ct) =>
            {
                // Limite de tamanho (defensivo)
                const long maxBytes = 5 * 1024 * 1024;
                if (request.ContentLength is > maxBytes)
                    return Results.BadRequest("Arquivo muito grande.");

                string targetPath;
                try
                {
                    targetPath = resolver.Resolve(storage, path);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }

                var canOverwrite = overwrite ?? true;

                if (!canOverwrite && File.Exists(targetPath))
                    return Results.Conflict("Arquivo já existe e overwrite=false.");

                var tmpPath = targetPath + ".tmp";

                try
                {
                    // Copia o stream cru para arquivo temporário
                    // (stream só pode ser lido uma vez)
                    await using (var tmp = new FileStream(
                        tmpPath,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None,
                        bufferSize: 64 * 1024,
                        useAsync: true))
                    {
                        await body.CopyToAsync(tmp, ct);
                        await tmp.FlushAsync(ct);
                    }

                    File.Move(tmpPath, targetPath, overwrite: canOverwrite);

                    return Results.Ok(new
                    {
                        storage,
                        path,
                        overwrite = canOverwrite,
                        savedAs = targetPath,
                        bytes = request.ContentLength
                    });
                }
                catch
                {
                    if (File.Exists(tmpPath))
                        File.Delete(tmpPath);

                    return Results.StatusCode(StatusCodes.Status500InternalServerError);
                }
            })
            .DisableAntiforgery()
            .WithName("UploadBlobRaw")
            .WithSummary("Upload genérico de blob via raw body")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);

            // =========================================================
            // DOWNLOAD
            // =========================================================
            //
            // Rota:
            //   GET /api/blobs/{storage}/{**path}
            //
            group.MapGet("/{storage}/{**path}", (
                string storage,
                string path,
                IStorageResolver resolver) =>
            {
                string targetPath;
                try
                {
                    targetPath = resolver.Resolve(storage, path);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }

                if (!File.Exists(targetPath))
                    return Results.NotFound();

                // Results.File:
                // - 1º parâmetro: caminho físico do arquivo
                // - fileDownloadName: nome sugerido ao cliente
                return Results.File(
                    targetPath,
                    contentType: "application/octet-stream",
                    fileDownloadName: Path.GetFileName(targetPath));
            })
            .WithName("DownloadBlob")
            .WithSummary("Download genérico de blob")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}
