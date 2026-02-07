using CoreFlow.Api.Extensions;
using CoreFlow.Api.Infrastructure.Storage;
using CoreFlow.Api.Middleware;
using CoreFlow.Core.Application.Extensions;
using CoreFlow.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Log do ambiente detectado (para debug)
var environmentName = builder.Environment.EnvironmentName;
Console.WriteLine($"[DEBUG] Ambiente detectado: {environmentName}");
Console.WriteLine($"[DEBUG] ASPNETCORE_ENVIRONMENT: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "não definido"}");

// Configurar serviços
builder.Services.AddSwaggerConfiguration(environmentName);
builder.Services.AddEmailConfiguration(builder.Configuration);
builder.Services.AddCorsConfiguration(builder.Configuration);
builder.Services.AddSingleton<IStorageResolver, StorageResolver>();
builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddInfrastructureLayer(builder.Configuration);

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
});

var app = builder.Build();

// Configurar pipeline HTTP
app.UseSwaggerConfiguration(environmentName);
app.UseHttpsRedirection();

// CORS deve vir antes do middleware de autenticação
app.UseCors();

// Middleware de autenticação ClientID/ClientSecret (com bcrypt)
app.UseMiddleware<ClientAuthenticationMiddleware>();

// Registrar endpoints
app.MapApplicationEndpoints();

app.Run();
