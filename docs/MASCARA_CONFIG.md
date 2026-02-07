# Mascara de dados de configuracao

Objetivo: compartilhar exemplos de configuracao sem expor segredos (credenciais, chaves, caminhos internos).

**Regras de mascara**
- Mantenha as mesmas chaves e a mesma estrutura do JSON original.
- Substitua valores sensiveis por placeholders entre <>.
- Preserve valores nao sensiveis (ex.: flags, portas, timeouts).
- Em connection strings, mantenha o formato e troque apenas host, usuario e senha.
- Em caminhos locais, remova o caminho real e use um placeholder.

**Exemplo mascarado (appsettings.json)**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=<DB_HOST>;Initial Catalog=CoreFlow;User ID=<DB_USER>;Password=<DB_PASSWORD>;Encrypt=True;TrustServerCertificate=True;"
  },
  "AllowedHosts": "*",
  "Smtp": {
    "Host": "smtp.example.com",
    "Port": 465,
    "UseStartTls": true,
    "UseSsl": true,
    "Username": "<SMTP_USER>",
    "Password": "<SMTP_PASSWORD>",
    "FromName": "Connect Solutions - Core Services",
    "Timeout": "00:00:30"
  },
  "AuthorizedClients": [
    {
      "ClientId": "<CLIENT_ID>",
      "ClientSecret": "<CLIENT_SECRET>"
    }
  ],
  "Cors": {
    "AllowedOrigins": []
  },
  "Storages": {
    "nada-mais-importa": "<STORAGE_PATH>"
  }
}
```

**Como aplicar**
1. Copie o JSON real para um arquivo de documentacao ou bloco de exemplo.
2. Substitua dados sensiveis pelos placeholders acima.
3. Valide o JSON (sem quebrar estrutura ou tipos).
4. Compartilhe apenas a versao mascarada.

**Checklist rapido**
- Senhas e tokens removidos.
- Usuarios e ClientId substituidos.
- Hosts internos e caminhos locais trocados por placeholders.
- Estrutura e tipos originais preservados.
