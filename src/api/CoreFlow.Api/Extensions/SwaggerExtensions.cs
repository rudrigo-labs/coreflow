using Microsoft.OpenApi;

namespace CoreFlow.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerConfiguration(
        this IServiceCollection services,
        string environmentName)
    {
        var isDevelopmentOrStaging =
            environmentName.Equals("Development", StringComparison.OrdinalIgnoreCase) ||
            environmentName.Equals("Staging", StringComparison.OrdinalIgnoreCase);

        if (!isDevelopmentOrStaging)
            return services;

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Connect Solutions - Core Services API",
                Description =
                    "API de serviços core da Connect Solutions. " +
                    "Requer X-Client-Id + X-Client-Secret e Authorization Bearer token.",
                Contact = new OpenApiContact
                {
                    Name = "Connect Solutions",
                    Email = "rodrigo.oliveira@connectsolutions.com.br"
                }
            });

            options.CustomSchemaIds(t => t.FullName?.Replace('+', '.') ?? t.Name);

            // XML docs (se existir)
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // -----------------------------
            // Security Definitions (3 AND)
            // -----------------------------

            // Bearer (JWT)
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Bearer token. Exemplo: \"Bearer {token}\""
            });

            // ClientId
            options.AddSecurityDefinition("ClientId", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = "X-Client-Id",
                Description = "Client Id do integrador"
            });

            // ClientSecret
            options.AddSecurityDefinition("ClientSecret", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = "X-Client-Secret",
                Description = "Client Secret do integrador"
            });

            // Requirement global (AND)
            // (No OpenApi v2.x: usa OpenApiSecuritySchemeReference)
            options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
            {
                { new OpenApiSecuritySchemeReference("ClientId"), new List<string>() },
                { new OpenApiSecuritySchemeReference("ClientSecret"), new List<string>() },
                { new OpenApiSecuritySchemeReference("Bearer"), new List<string>() }
            });
        });

        return services;
    }

    public static WebApplication UseSwaggerConfiguration(
        this WebApplication app,
        string environmentName)
    {
        var isDevelopmentOrStaging =
            environmentName.Equals("Development", StringComparison.OrdinalIgnoreCase) ||
            environmentName.Equals("Staging", StringComparison.OrdinalIgnoreCase);

        if (!isDevelopmentOrStaging)
            return app;

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Core Services API v1");
            options.RoutePrefix = string.Empty;
            options.DisplayRequestDuration();
            options.EnableTryItOutByDefault();
            options.DefaultModelsExpandDepth(-1);
        });

        return app;
    }
}
