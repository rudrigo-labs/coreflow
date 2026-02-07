using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreFlow.Shared.Email.Models
{
    /// <summary>
    /// Representa uma mensagem de email completa com destinatários, assunto, corpo e anexos.
    /// </summary>
    public sealed class EmailMessage
    {
        /// <summary>
        /// Obtém o endereço do remetente do email.
        /// </summary>
        /// <value>O endereço de email do remetente. Campo obrigatório.</value>
        public required EmailAddress From { get; init; }

        /// <summary>
        /// Obtém a lista de destinatários principais do email.
        /// </summary>
        /// <value>Lista de endereços de email dos destinatários. Campo obrigatório e não pode estar vazio.</value>
        public required IReadOnlyList<EmailAddress> To { get; init; }

        /// <summary>
        /// Obtém a lista de destinatários em cópia (CC).
        /// </summary>
        /// <value>Lista de endereços de email em cópia. Opcional.</value>
        public IReadOnlyList<EmailAddress>? Cc { get; init; }

        /// <summary>
        /// Obtém a lista de destinatários em cópia oculta (BCC).
        /// </summary>
        /// <value>Lista de endereços de email em cópia oculta. Opcional.</value>
        public IReadOnlyList<EmailAddress>? Bcc { get; init; }

        /// <summary>
        /// Obtém o assunto do email.
        /// </summary>
        /// <value>O texto do assunto do email. Opcional.</value>
        public string? Subject { get; init; }

        /// <summary>
        /// Obtém o corpo do email em formato texto simples.
        /// </summary>
        /// <value>O conteúdo do email em texto puro. Opcional, mas pelo menos TextBody ou HtmlBody deve ser fornecido.</value>
        public string? TextBody { get; init; }

        /// <summary>
        /// Obtém o corpo do email em formato HTML.
        /// </summary>
        /// <value>O conteúdo do email em HTML. Opcional, mas pelo menos TextBody ou HtmlBody deve ser fornecido.</value>
        public string? HtmlBody { get; init; }

        /// <summary>
        /// Obtém a lista de anexos do email.
        /// </summary>
        /// <value>Lista de anexos contendo (Nome do arquivo, Tipo de conteúdo MIME, Conteúdo em bytes). Opcional.</value>
        public IReadOnlyList<(string FileName, string ContentType, byte[] Content)>? Attachments { get; init; }
    }
}

