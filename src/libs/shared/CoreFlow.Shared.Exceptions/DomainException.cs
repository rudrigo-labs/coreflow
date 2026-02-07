using CoreFlow.Shared.Exceptions;
using System;

namespace CoreFlow.Shared.Exceptions
{
    /// <summary>
    /// Exceção lançada quando ocorre uma violação de regra de negócio do domínio.
    /// Representa erros relacionados a lógica de negócio, validações de domínio e regras específicas da aplicação.
    /// </summary>
    [Serializable]
    public class DomainException : AppException
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="DomainException"/> com uma mensagem.
        /// </summary>
        /// <param name="message">A mensagem que descreve a violação da regra de negócio.</param>
        public DomainException(string message)
            : base(message, "domain_error", null, null, null) { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="DomainException"/> com uma mensagem e exceção interna.
        /// </summary>
        /// <param name="message">A mensagem que descreve a violação da regra de negócio.</param>
        /// <param name="inner">A exceção que é a causa da exceção atual.</param>
        public DomainException(string message, Exception inner)
            : base(message, "domain_error", null, null, inner) { }
    }
}

