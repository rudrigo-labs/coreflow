using CoreFlow.Shared.Email.Configurations;
using CoreFlow.Shared.Email.Interfaces;
using CoreFlow.Shared.Email.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreFlow.Shared.Email.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSmtpEmailSender(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = "Smtp")
        {
            services.Configure<SmtpEmailSettings>(configuration.GetRequiredSection(sectionName));
            services.AddSingleton<IEmailSender, SmtpEmailSender>();
            return services;
        }
    }
}

