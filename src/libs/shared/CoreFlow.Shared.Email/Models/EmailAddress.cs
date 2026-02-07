using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreFlow.Shared.Email.Models
{
    /// <summary>
    /// Representa um endereço de email com nome e endereço.
    /// </summary>
    /// <param name="Name">O nome do destinatário ou remetente.</param>
    /// <param name="Address">O endereço de email (ex: usuario@exemplo.com).</param>
    public sealed record EmailAddress(string Name, string Address)
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase,
            TimeSpan.FromMilliseconds(250));

        /// <summary>
        /// Valida se o endereço de email possui um formato válido.
        /// </summary>
        /// <param name="address">O endereço de email a ser validado.</param>
        /// <returns><c>true</c> se o formato é válido; caso contrário, <c>false</c>.</returns>
        public static bool IsValidEmail(string? address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return false;

            if (address.Length > 254) // RFC 5321
                return false;

            try
            {
                return EmailRegex.IsMatch(address);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Valida se esta instância possui um endereço de email válido.
        /// </summary>
        /// <returns><c>true</c> se o endereço é válido; caso contrário, <c>false</c>.</returns>
        public bool IsValid()
        {
            return IsValidEmail(Address);
        }

        /// <summary>
        /// Cria uma nova instância de <see cref="EmailAddress"/> validando o formato do email.
        /// </summary>
        /// <param name="name">O nome do destinatário ou remetente.</param>
        /// <param name="address">O endereço de email.</param>
        /// <returns>Uma nova instância de <see cref="EmailAddress"/>.</returns>
        /// <exception cref="ArgumentException">Lançada quando o endereço de email é inválido.</exception>
        public static EmailAddress Create(string name, string address)
        {
            if (!IsValidEmail(address))
                throw new ArgumentException($"Invalid email address format: {address}", nameof(address));
            
            return new EmailAddress(name, address);
        }
    }
}

