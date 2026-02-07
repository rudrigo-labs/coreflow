# AuthCenter.CoreFlow.Shared.Email

Uma biblioteca comum para envio de emails usando SMTP com MailKit no .NET 9.

## ?? Caracter sticas

- ? **Suporte completo a SMTP** (SSL/TLS, STARTTLS)
- ? **Anexos** (arquivos)
- ? **HTML e texto** no corpo do email
- ? **CC e BCC**
- ? **Timeout configur vel**
- ? **Logging integrado**
- ? **Inje  o de depend ncia** pronta
- ? **Baseado em MailKit** (biblioteca moderna e robusta)

## ?? Instala  o

### 1. Adicionar refer ncia ao projeto

```xml
<ProjectReference Include="..\..\common\AuthCenter.CoreFlow.Shared.Email\AuthCenter.CoreFlow.Shared.Email.csproj" />
```

### 2. Configurar no `Program.cs` ou `Startup.cs`

```csharp
using AuthCenter.CoreFlow.Shared.Email.Extensions;

// Registro do servi o de email
builder.Services.AddSmtpEmailSender(builder.Configuration);
```

## ?? Configura  o

### appsettings.json

```json
{
  "Smtp": {
    "Host": "smtp.zoho.com",
    "Port": 465,
    "UseStartTls": true,
    "UseSsl": true,
    "Username": "noreply@connectsolutions.com.br",
    "Password": "6@dslqXq",
    "FromName": "Identity Hub - Connect Solutions",
    "Timeout": "00:00:30"
  }
}
```

### Configura  es Dispon veis

| Propriedade | Tipo | Padr o | Descri  o |
|-------------|------|--------|-----------|
| `Host` | string | - | Servidor SMTP |
| `Port` | int | 587 | Porta do servidor |
| `UseStartTls` | bool | true | Usar STARTTLS (porta 587) |
| `UseSsl` | bool | false | Usar SSL direto (porta 465) |
| `Username` | string | - | Usu rio para autentica  o |
| `Password` | string | - | Senha para autentica  o |
| `FromName` | string | - | Nome do remetente |
| `Timeout` | TimeSpan | 30s | Timeout da conex o |

## ?? Uso

### Inje  o de Depend ncia

```csharp
public class EmailService
{
    private readonly IEmailSender _emailSender;

    public EmailService(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }
}
```

### Envio B sico

```csharp
var sender = services.GetRequiredService<IEmailSender>();

var message = new EmailMessage
{
    From = new EmailAddress("Minha App", "no-reply@minhaapp.com"),
    To = new[] { new EmailAddress("Destinat rio", "alguem@exemplo.com") },
    Subject = "Teste MailKit + .NET 9",
    TextBody = "Vers o texto puro.",
    HtmlBody = "<strong>Vers o HTML</strong> com MailKit e .NET 9.",
    Attachments = new[]
    {
        ("exemplo.txt", "text/plain", System.Text.Encoding.UTF8.GetBytes("ol !"))
    }
};

await sender.SendAsync(message);
```

### Exemplo Completo com CC e BCC

```csharp
var message = new EmailMessage
{
    From = new EmailAddress("Sistema", "noreply@authcenter.com"),
    To = new[] 
    { 
        new EmailAddress("Usu rio Principal", "user@email.com") 
    },
    Cc = new[] 
    { 
        new EmailAddress("C pia", "cc@email.com") 
    },
    Bcc = new[] 
    { 
        new EmailAddress("C pia Oculta", "bcc@email.com") 
    },
    Subject = "Confirma  o de Email",
    HtmlBody = @"
        <h1>Bem-vindo ao AuthCenter!</h1>
        <p>Clique no link abaixo para confirmar seu email:</p>
        <a href='https://authcenter.com/confirm?token=abc123'>Confirmar Email</a>
    ",
    TextBody = "Bem-vindo ao AuthCenter! Acesse: https://authcenter.com/confirm?token=abc123"
};

await _emailSender.SendAsync(message);
```

## ?? Provedores SMTP Suportados

### Gmail
```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "UseStartTls": true,
    "UseSsl": false,
    "Username": "seu-email@gmail.com",
    "Password": "sua-senha-app"
  }
}
```

### Outlook/Hotmail
```json
{
  "Smtp": {
    "Host": "smtp-mail.outlook.com",
    "Port": 587,
    "UseStartTls": true,
    "UseSsl": false,
    "Username": "seu-email@outlook.com",
    "Password": "sua-senha"
  }
}
```

### Zoho (Configura  o Atual)
```json
{
  "Smtp": {
    "Host": "smtp.zoho.com",
    "Port": 465,
    "UseStartTls": true,
    "UseSsl": true,
    "Username": "noreply@connectsolutions.com.br",
    "Password": "6@dslqXq",
    "FromName": "Identity Hub - Connect Solutions"
  }
}
```

## ??? Desenvolvimento

### Estrutura do Projeto

```
AuthCenter.CoreFlow.Shared.Email/
+-- Configurations/
    +-- SmtpEmailSettings.cs
+-- Extensions/
    +-- ServiceCollectionExtensions.cs
+-- Interfaces/
    +-- IEmailSender.cs
+-- Models/
    +-- EmailAddress.cs
    +-- EmailMessage.cs
+-- Services/
    +-- SmtpEmailSender.cs
+-- README.md
```

### Depend ncias

- **MailKit** (4.14.1) - Biblioteca principal para SMTP
- **Microsoft.Extensions.Logging.Abstractions** (9.0.10)
- **Microsoft.Extensions.Options.ConfigurationExtensions** (9.0.10)

## ?? Tratamento de Erros

A biblioteca inclui logging autom tico de erros:

```csharp
try
{
    await _emailSender.SendAsync(message);
}
catch (Exception ex)
{
    // O erro j    logado automaticamente pela biblioteca
    // Voc  pode adicionar tratamento adicional aqui
    _logger.LogError("Falha ao enviar email: {Error}", ex.Message);
}
```

## ?? Logs

A biblioteca registra automaticamente:
- ? Tentativas de envio
- ? Falhas de conex o
- ? Erros de autentica  o
- ? Timeouts

## ?? Seguran a

- ? **Senhas de aplicativo** recomendadas para Gmail/Outlook
- ? **SSL/TLS** obrigat rio
- ? **Timeout** configur vel para evitar travamentos
- ? **Valida  o** de endere os de email

## ?? Exemplos de Uso no AuthCenter

### Confirma  o de Email
```csharp
public async Task<Result> SendEmailConfirmationAsync(string email, string token)
{
    var message = new EmailMessage
    {
        From = new EmailAddress("AuthCenter", "noreply@authcenter.com"),
        To = new[] { new EmailAddress("Usu rio", email) },
        Subject = "Confirme seu email - AuthCenter",
        HtmlBody = GenerateConfirmationEmailHtml(token)
    };

    await _emailSender.SendAsync(message);
    return Result.Successful();
}
```

### Reset de Senha
```csharp
public async Task<Result> SendPasswordResetAsync(string email, string resetToken)
{
    var message = new EmailMessage
    {
        From = new EmailAddress("AuthCenter", "noreply@authcenter.com"),
        To = new[] { new EmailAddress("Usu rio", email) },
        Subject = "Redefinir senha - AuthCenter",
        HtmlBody = GeneratePasswordResetEmailHtml(resetToken)
    };

    await _emailSender.SendAsync(message);
    return Result.Successful();
}
```

---

**Desenvolvido para AuthCenter - Connect Solutions** ??
