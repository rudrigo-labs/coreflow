using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CoreFlow.Api.Models.Email;
using CoreFlow.Shared.Email.Interfaces;
using CoreFlow.Shared.Email.Configurations;
using CoreFlow.Shared.Email.Models;

namespace CoreFlow.Api.Endpoints;

public static class EmailEndpoints
{
    /// <summary>
    /// Registra os endpoints relacionados ao serviço de e-mail.
    /// </summary>
    public static void MapEmailEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/email")
            .WithTags("Email");

        group.MapPost("/send-simple", SendSimpleEmailAsync)
            .WithName("SendSimpleEmail")
            .WithSummary("Envia um e-mail simples")
            .WithDescription("Envia um e-mail simples com destinatário, assunto e mensagem. Ideal para uso em formulários.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/send-complete", SendCompleteEmailAsync)
            .WithName("SendCompleteEmail")
            .WithSummary("Envia um e-mail completo")
            .WithDescription("Envia um e-mail completo com suporte a CC, BCC, HTML e anexos")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Endpoint para envio de e-mail simples (usado em formulários).
    /// </summary>
    private static async Task<IResult> SendSimpleEmailAsync(
        [FromBody] SendSimpleEmailRequest request,
        IEmailSender emailSender,
        IOptions<SmtpEmailSettings> smtpSettings,
        CancellationToken cancellationToken)
    {
        // Validação básica
        if (request == null)
            return Results.BadRequest(new { error = "Request body is required" });

        if (string.IsNullOrWhiteSpace(request.FromName))
            return Results.BadRequest(new { error = "FromName is required" });

        if (string.IsNullOrWhiteSpace(request.To))
            return Results.BadRequest(new { error = "To address is required" });

        if (string.IsNullOrWhiteSpace(request.Subject))
            return Results.BadRequest(new { error = "Subject is required" });

        if (string.IsNullOrWhiteSpace(request.Message))
            return Results.BadRequest(new { error = "Message is required" });

        try
        {
            // Validar endereço de e-mail do destinatário
            if (!EmailAddress.IsValidEmail(request.To))
                return Results.BadRequest(new { error = $"Invalid email address: {request.To}" });

            var settings = smtpSettings.Value;
            var fromAddress = new EmailAddress(request.FromName, settings.Username);
            var toAddress = new EmailAddress(string.Empty, request.To);

            // Criar mensagem de e-mail
            var emailMessage = new EmailMessage
            {
                From = fromAddress,
                To = new List<EmailAddress> { toAddress },
                Subject = request.Subject,
                TextBody = request.Message
            };

            // Enviar e-mail
            await emailSender.SendAsync(emailMessage, cancellationToken);

            return Results.Ok(new SendEmailResponse
            {
                Success = true,
                Message = "E-mail enviado com sucesso"
            });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Erro ao enviar e-mail"
            );
        }
    }

    /// <summary>
    /// Endpoint para envio de e-mail completo.
    /// </summary>
    private static async Task<IResult> SendCompleteEmailAsync(
        [FromBody] SendEmailRequest request,
        IEmailSender emailSender,
        CancellationToken cancellationToken)
    {
        // Validação básica
        if (request == null)
            return Results.BadRequest(new { error = "Request body is required" });

        if (request.To == null || request.To.Count == 0)
            return Results.BadRequest(new { error = "At least one recipient (To) is required" });

        if (string.IsNullOrWhiteSpace(request.Subject))
            return Results.BadRequest(new { error = "Subject is required" });

        if (string.IsNullOrWhiteSpace(request.TextBody) && string.IsNullOrWhiteSpace(request.HtmlBody))
            return Results.BadRequest(new { error = "At least one body (TextBody or HtmlBody) is required" });

        try
        {
            // Validar endereços de e-mail
            var toAddresses = new List<EmailAddress>();
            foreach (var to in request.To)
            {
                if (!EmailAddress.IsValidEmail(to.Address))
                    return Results.BadRequest(new { error = $"Invalid email address: {to.Address}" });
                
                toAddresses.Add(new EmailAddress(to.Name ?? string.Empty, to.Address));
            }

            // Validar remetente
            if (request.From == null || string.IsNullOrWhiteSpace(request.From.Address))
                return Results.BadRequest(new { error = "From address is required" });

            if (string.IsNullOrWhiteSpace(request.From.Name))
                return Results.BadRequest(new { error = "FromName is required" });

            if (!EmailAddress.IsValidEmail(request.From.Address))
                return Results.BadRequest(new { error = $"Invalid from email address: {request.From.Address}" });

            var fromAddress = new EmailAddress(request.From.Name, request.From.Address);

            // Validar CC se fornecido
            var ccAddresses = request.Cc?.Select(cc =>
            {
                if (!EmailAddress.IsValidEmail(cc.Address))
                    throw new ArgumentException($"Invalid CC email address: {cc.Address}");
                return new EmailAddress(cc.Name ?? string.Empty, cc.Address);
            }).ToList();

            // Validar BCC se fornecido
            var bccAddresses = request.Bcc?.Select(bcc =>
            {
                if (!EmailAddress.IsValidEmail(bcc.Address))
                    throw new ArgumentException($"Invalid BCC email address: {bcc.Address}");
                return new EmailAddress(bcc.Name ?? string.Empty, bcc.Address);
            }).ToList();

            // Processar anexos se fornecidos
            IReadOnlyList<(string FileName, string ContentType, byte[] Content)>? attachments = null;
            if (request.Attachments != null && request.Attachments.Count > 0)
            {
                var attachmentList = new List<(string FileName, string ContentType, byte[] Content)>();
                foreach (var attachment in request.Attachments)
                {
                    try
                    {
                        var contentBytes = Convert.FromBase64String(attachment.ContentBase64);
                        attachmentList.Add((attachment.FileName, attachment.ContentType, contentBytes));
                    }
                    catch (FormatException)
                    {
                        return Results.BadRequest(new { error = $"Invalid base64 content for attachment: {attachment.FileName}" });
                    }
                }
                attachments = attachmentList;
            }

            // Criar mensagem de e-mail
            var emailMessage = new EmailMessage
            {
                From = fromAddress,
                To = toAddresses,
                Cc = ccAddresses,
                Bcc = bccAddresses,
                Subject = request.Subject,
                TextBody = request.TextBody,
                HtmlBody = request.HtmlBody,
                Attachments = attachments
            };

            // Enviar e-mail
            await emailSender.SendAsync(emailMessage, cancellationToken);

            return Results.Ok(new SendEmailResponse
            {
                Success = true,
                Message = "E-mail enviado com sucesso"
            });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Erro ao enviar e-mail"
            );
        }
    }
}

