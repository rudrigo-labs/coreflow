using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreFlow.Shared.Email.Configurations
{
    /// <summary>
    /// Configurações para conexão SMTP e envio de emails.
    /// Usado para configurar o serviço de envio de emails via SMTP.
    /// </summary>
    public sealed class SmtpEmailSettings
    {
        /// <summary>
        /// Obtém ou define o endereço do servidor SMTP.
        /// </summary>
        /// <value>O hostname ou endereço IP do servidor SMTP. Campo obrigatório.</value>
        public string Host { get; set; } = default!;

        /// <summary>
        /// Obtém ou define a porta do servidor SMTP.
        /// </summary>
        /// <value>A porta do servidor SMTP. Padrão: 587.</value>
        public int Port { get; set; } = 587;

        /// <summary>
        /// Obtém ou define um valor que indica se deve usar STARTTLS para conexão segura.
        /// </summary>
        /// <value><c>true</c> para usar STARTTLS (porta 587, padrão); caso contrário, <c>false</c>.</value>
        public bool UseStartTls { get; set; } = true;

        /// <summary>
        /// Obtém ou define um valor que indica se deve usar SSL/TLS direto para conexão segura.
        /// </summary>
        /// <value><c>true</c> para usar SSL/TLS direto (porta 465); caso contrário, <c>false</c>.</value>
        public bool UseSsl { get; set; } = false;

        /// <summary>
        /// Obtém ou define o nome de usuário para autenticação SMTP.
        /// </summary>
        /// <value>O nome de usuário para autenticação. Campo obrigatório.</value>
        public string Username { get; set; } = default!;

        /// <summary>
        /// Obtém ou define a senha para autenticação SMTP.
        /// </summary>
        /// <value>A senha para autenticação. Campo obrigatório.</value>
        public string Password { get; set; } = default!;

        /// <summary>
        /// Obtém ou define o nome do remetente padrão para emails.
        /// </summary>
        /// <value>O nome que aparecerá como remetente nos emails. Campo obrigatório.</value>
        public string FromName { get; set; } = default!;

        /// <summary>
        /// Obtém ou define o timeout para operações SMTP.
        /// </summary>
        /// <value>O tempo limite para conexões e operações SMTP. Padrão: 30 segundos.</value>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}

