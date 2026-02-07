using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreFlow.Shared.Logging
{
    public static class ConsoleLoggerExtensions
    {
        public static void LogImportStart(this ILogger logger, string contexto)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[INÍCIO - {contexto}] {DateTime.Now:HH:mm:ss}");
            Console.ResetColor();

            logger.LogInformation("Iniciando processo: {Contexto}", contexto);
        }

        public static void LogImportEnd(this ILogger logger, string contexto)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[FIM - {contexto}] {DateTime.Now:HH:mm:ss}");
            Console.ResetColor();

            logger.LogInformation("Processo finalizado: {Contexto}", contexto);
        }

        public static void LogLoteProcessado(this ILogger logger, int loteIndex, int total, int quantidade)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Lote {loteIndex + 1}/{total} processado. {quantidade} itens.");
            Console.ResetColor();

            logger.LogInformation("Lote {Index}/{Total} processado com {Quantidade} itens", loteIndex + 1, total, quantidade);
        }

        public static void LogErroCritico(this ILogger logger, Exception ex, string contexto)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERRO - {contexto}] {ex.Message}");
            Console.ResetColor();

            logger.LogError(ex, "Erro durante o processo: {Contexto}", contexto);
        }

        public static void LogAviso(this ILogger logger, string mensagem)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[AVISO] {mensagem}");
            Console.ResetColor();

            logger.LogWarning(mensagem);
        }

        public static void LogSucesso(this ILogger logger, string mensagem)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[SUCESSO] {mensagem}");
            Console.ResetColor();

            logger.LogInformation(mensagem);
        }

        public static void LogFalha(this ILogger logger, string mensagem)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[FALHA] {mensagem}");
            Console.ResetColor();

            logger.LogError(mensagem);
        }

    }
}

