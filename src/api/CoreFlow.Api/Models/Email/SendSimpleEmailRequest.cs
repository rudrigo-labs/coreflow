namespace CoreFlow.Api.Models.Email;

/// <summary>
/// DTO para requisição de envio de e-mail simples (usado em formulários).
/// </summary>
public sealed class SendSimpleEmailRequest
{
    /// <summary>
    /// Nome do remetente.
    /// </summary>
    public required string FromName { get; init; }

    /// <summary>
    /// Endereço de e-mail do destinatário.
    /// </summary>
    public required string To { get; init; }

    /// <summary>
    /// Assunto do e-mail. Usado para identificar o formulário que está sendo enviado.
    /// </summary>
    public required string Subject { get; init; }

    /// <summary>
    /// Mensagem do e-mail (corpo).
    /// </summary>
    public required string Message { get; init; }
}

