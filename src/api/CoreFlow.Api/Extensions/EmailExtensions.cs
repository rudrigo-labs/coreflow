using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CoreFlow.Shared.Email.Extensions;

namespace CoreFlow.Api.Extensions;

/// <summary>
/// Extensões para configuração do serviço de e-mail.
/// </summary>
public static class EmailExtensions
{
    /// <summary>
    /// Adiciona a configuração do serviço de e-mail aos serviços.
    /// </summary>
    /// <param name="services">A coleção de serviços.</param>
    /// <param name="configuration">A configuração da aplicação.</param>
    public static IServiceCollection AddEmailConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSmtpEmailSender(configuration);
        return services;
    }
}

