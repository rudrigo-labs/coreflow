using CoreFlow.Api.Models.Email;

namespace CoreFlow.Api.Models.Email;

/// <summary>
/// DTO para requisição de envio de e-mail.
/// </summary>
public sealed class SendEmailRequest
{
    /// <summary>
    /// Endereço do remetente.
    /// </summary>
    public required EmailAddressDto From { get; init; }

    /// <summary>
    /// Lista de destinatários principais.
    /// </summary>
    public required IList<EmailAddressDto> To { get; init; }

    /// <summary>
    /// Lista de destinatários em cópia (CC). Opcional.
    /// </summary>
    public IList<EmailAddressDto>? Cc { get; init; }

    /// <summary>
    /// Lista de destinatários em cópia oculta (BCC). Opcional.
    /// </summary>
    public IList<EmailAddressDto>? Bcc { get; init; }

    /// <summary>
    /// Assunto do e-mail. Obrigatório. Usado para identificar o formulário que está sendo enviado.
    /// </summary>
    public required string Subject { get; init; }

    /// <summary>
    /// Corpo do e-mail em texto simples. Opcional, mas pelo menos TextBody ou HtmlBody deve ser fornecido.
    /// </summary>
    public string? TextBody { get; init; }

    /// <summary>
    /// Corpo do e-mail em HTML. Opcional, mas pelo menos TextBody ou HtmlBody deve ser fornecido.
    /// </summary>
    public string? HtmlBody { get; init; }

    /// <summary>
    /// Lista de anexos. Opcional.
    /// Cada anexo contém: Nome do arquivo, Tipo de conteúdo MIME, Conteúdo em base64.
    /// </summary>
    public IList<EmailAttachmentDto>? Attachments { get; init; }
}

/// <summary>
/// DTO para representar um endereço de e-mail.
/// </summary>
public sealed class EmailAddressDto
{
    /// <summary>
    /// Nome do destinatário ou remetente. Opcional.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Endereço de e-mail (ex: usuario@exemplo.com).
    /// </summary>
    public required string Address { get; init; }
}

/// <summary>
/// DTO para representar um anexo de e-mail.
/// </summary>
public sealed class EmailAttachmentDto
{
    /// <summary>
    /// Nome do arquivo.
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Tipo de conteúdo MIME (ex: application/pdf, image/png).
    /// </summary>
    public required string ContentType { get; init; }

    /// <summary>
    /// Conteúdo do arquivo em base64.
    /// </summary>
    public required string ContentBase64 { get; init; }
}

