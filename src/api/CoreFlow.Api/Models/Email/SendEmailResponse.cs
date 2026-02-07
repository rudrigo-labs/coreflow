namespace CoreFlow.Api.Models.Email;

/// <summary>
/// DTO para resposta de envio de e-mail.
/// </summary>
public sealed class SendEmailResponse
{
    /// <summary>
    /// Indica se o e-mail foi enviado com sucesso.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Mensagem de resposta.
    /// </summary>
    public required string Message { get; init; }
}

