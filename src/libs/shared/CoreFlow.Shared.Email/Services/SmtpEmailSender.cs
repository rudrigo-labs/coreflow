using CoreFlow.Shared.Email.Configurations;
using CoreFlow.Shared.Email.Interfaces;
using CoreFlow.Shared.Email.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CoreFlow.Shared.Email.Services
{
    /// <summary>
    /// Implementação do serviço de envio de emails usando SMTP através da biblioteca MailKit.
    /// Suporta SSL/TLS direto (porta 465) e STARTTLS (porta 587).
    /// </summary>
    public sealed class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpEmailSettings settings;
        private readonly ILogger? logger;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="SmtpEmailSender"/>.
        /// </summary>
        /// <param name="emailSettings">As configurações de email.</param>
        /// <param name="emailLogger">O logger opcional para registrar eventos.</param>
        /// <exception cref="ArgumentNullException">Lançada quando emailSettings é null.</exception>
        /// <exception cref="InvalidOperationException">Lançada quando as configurações não foram encontradas.</exception>
        public SmtpEmailSender(IOptions<SmtpEmailSettings> emailSettings, ILogger<SmtpEmailSender>? emailLogger = null)
        {
            ArgumentNullException.ThrowIfNull(emailSettings);
            settings = emailSettings.Value ?? throw new InvalidOperationException("SMTP email settings not configured.");
            
            // Validação básica das configurações
            if (string.IsNullOrWhiteSpace(settings.Host))
                throw new InvalidOperationException("SMTP Host is required.");
            if (settings.Port <= 0 || settings.Port > 65535)
                throw new InvalidOperationException("SMTP Port must be between 1 and 65535.");
        }

        /// <summary>
        /// Envia um email de forma assíncrona usando SMTP.
        /// </summary>
        /// <param name="message">A mensagem de email a ser enviada.</param>
        /// <param name="ct">Token de cancelamento para cancelar a operação assíncrona. Padrão: CancellationToken.None.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona de envio do email.</returns>
        /// <exception cref="ArgumentNullException">Lançada quando message é null.</exception>
        /// <exception cref="ArgumentException">Lançada quando a mensagem não possui destinatários válidos.</exception>
        /// <exception cref="Exception">Lançada quando ocorre um erro durante o envio do email. A exceção é registrada no logger antes de ser relançada.</exception>
        public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(message);
            ArgumentNullException.ThrowIfNull(message.From);
            
            if (message.To == null || message.To.Count == 0)
                throw new ArgumentException("At least one recipient is required.", nameof(message));
            
            if (string.IsNullOrWhiteSpace(message.From.Address))
                throw new ArgumentException("From address is required.", nameof(message));
            
            if (string.IsNullOrWhiteSpace(message.TextBody) && string.IsNullOrWhiteSpace(message.HtmlBody))
                throw new ArgumentException("Either TextBody or HtmlBody must be provided.", nameof(message));

            var email = BuildMimeMessage(message);

            using var client = new SmtpClient();

            client.Timeout = (int)settings.Timeout.TotalMilliseconds;

            try
            {
                // Conexão: SSL direto (465) ou STARTTLS (587)
                if (settings.UseSsl)
                {
                    await client.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.SslOnConnect, ct);
                }
                else if (settings.UseStartTls)
                {
                    await client.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.StartTls, ct);
                }
                else
                {
                    await client.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.Auto, ct);
                }

                if (!string.IsNullOrWhiteSpace(settings.Username))
                {
                    await client.AuthenticateAsync(settings.Username, settings.Password, ct);
                }

                await client.SendAsync(email, ct);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Falha ao enviar e-mail para {Recipients}", string.Join(", ", message.To.Select(t => t.Address)));
                throw;
            }
            finally
            {
                try { await client.DisconnectAsync(true, ct); } catch { /* ignore */ }
            }
        }

        /// <summary>
        /// Constrói uma mensagem MIME a partir de um <see cref="EmailMessage"/>.
        /// </summary>
        /// <param name="msg">A mensagem de email a ser convertida.</param>
        /// <returns>Uma instância de <see cref="MimeMessage"/> configurada com os dados da mensagem.</returns>
        /// <exception cref="ArgumentNullException">Lançada quando msg é null.</exception>
        /// <exception cref="ArgumentException">Lançada quando dados inválidos são fornecidos.</exception>
        private static MimeMessage BuildMimeMessage(EmailMessage msg)
        {
            ArgumentNullException.ThrowIfNull(msg);
            ArgumentNullException.ThrowIfNull(msg.From);
            
            if (msg.To == null || msg.To.Count == 0)
                throw new ArgumentException("At least one recipient is required.", nameof(msg));

            var email = new MimeMessage();

            try
            {
                email.From.Add(new MailboxAddress(msg.From.Name, msg.From.Address));
                
                foreach (var to in msg.To)
                {
                    if (to == null || string.IsNullOrWhiteSpace(to.Address))
                        throw new ArgumentException("Invalid recipient address.", nameof(msg));
                    email.To.Add(new MailboxAddress(to.Name, to.Address));
                }
                
                if (msg.Cc != null)
                {
                    foreach (var cc in msg.Cc)
                    {
                        if (cc == null || string.IsNullOrWhiteSpace(cc.Address))
                            continue; // Ignora CCs inválidos em vez de falhar
                        email.Cc.Add(new MailboxAddress(cc.Name, cc.Address));
                    }
                }
                
                if (msg.Bcc != null)
                {
                    foreach (var bcc in msg.Bcc)
                    {
                        if (bcc == null || string.IsNullOrWhiteSpace(bcc.Address))
                            continue; // Ignora BCCs inválidos em vez de falhar
                        email.Bcc.Add(new MailboxAddress(bcc.Name, bcc.Address));
                    }
                }
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentException)
            {
                throw new ArgumentException($"Invalid email address format: {ex.Message}", nameof(msg), ex);
            }

            email.Subject = msg.Subject ?? string.Empty;

            var builder = new BodyBuilder();

            if (!string.IsNullOrEmpty(msg.HtmlBody))
                builder.HtmlBody = msg.HtmlBody;

            if (!string.IsNullOrEmpty(msg.TextBody))
                builder.TextBody = msg.TextBody;

            if (msg.Attachments is { Count: > 0 })
            {
                foreach (var (fileName, contentType, content) in msg.Attachments)
                {
                    if (string.IsNullOrWhiteSpace(fileName))
                        throw new ArgumentException("Attachment file name cannot be null or empty.", nameof(msg));
                    if (content == null || content.Length == 0)
                        throw new ArgumentException("Attachment content cannot be null or empty.", nameof(msg));
                    if (string.IsNullOrWhiteSpace(contentType))
                        throw new ArgumentException("Attachment content type cannot be null or empty.", nameof(msg));
                    
                    try
                    {
                        builder.Attachments.Add(fileName, content, ContentType.Parse(contentType));
                    }
                    catch (Exception ex) when (ex is FormatException || ex is ArgumentException)
                    {
                        throw new ArgumentException($"Invalid attachment format: {ex.Message}", nameof(msg), ex);
                    }
                }
            }

            email.Body = builder.ToMessageBody();
            return email;
        }
    }
}

