using CoreFlow.Shared.Email.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreFlow.Shared.Email.Interfaces
{
    /// <summary>
    /// Interface para envio de emails de forma assíncrona.
    /// Define o contrato para implementações de serviços de envio de email.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Envia um email de forma assíncrona.
        /// </summary>
        /// <param name="message">A mensagem de email a ser enviada.</param>
        /// <param name="ct">Token de cancelamento para cancelar a operação assíncrona. Padrão: CancellationToken.None.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona de envio do email.</returns>
        Task SendAsync(EmailMessage message, CancellationToken ct = default);
    }
}

