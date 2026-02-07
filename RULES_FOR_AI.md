# RULES_FOR_AI – CoreFlow

Este documento define as regras **mandatórias** para qualquer IA que vá trabalhar neste repositório. Se uma instrução sua conflitar com o código do template, **o código vence**.

---

**Princípios inegociáveis**

- CoreFlow é **arquitetura**, não boilerplate. Não “adapte” camadas.
- A estrutura de projetos e heranças base **não pode ser alterada**.
- O fluxo `Customer` é o **modelo estrutural canônico**.
- O conteúdo de negócio pode mudar, a **estrutura não**.

---

**Dependências entre camadas (regra dura)**

- `CoreFlow.Core.Domain` não depende de nenhuma outra camada interna.
- `CoreFlow.Core.Application` depende apenas de `CoreFlow.Core.Domain` e `CoreFlow.Shared.Results`.
- `CoreFlow.Core.Contracts` é isolado, sem dependências de Application ou Infrastructure.
- `CoreFlow.Infrastructure` depende de `CoreFlow.Core.Application`, `CoreFlow.Core.Domain` e `CoreFlow.Infrastructure.Auditing`.
- `CoreFlow.Api` depende de `CoreFlow.Core.Application`, `CoreFlow.Infrastructure`, `CoreFlow.Core.Contracts` e projetos `CoreFlow.Shared.*`.
- `CoreFlow.Shared.*` não depende de Core/Infrastructure/Api.

Se uma alteração quebrar essa regra, **pare e peça confirmação**.

---

**Estrutura obrigatória de um fluxo (use `Customer` como template)**

- Domain
- Aggregate em `src/libs/core/CoreFlow.Core.Domain/Entities/<Entidade>Aggregate/`.
- Eventos de domínio no mesmo folder.

- Application
- Command em `src/libs/core/CoreFlow.Core.Application/Commands/<Entidade>/`.
- Query em `src/libs/core/CoreFlow.Core.Application/Queries/<Entidade>/`.
- ReadModel em `src/libs/core/CoreFlow.Core.Application/ReadModels/<Entidade>/`.
- Interface de repositório em `src/libs/core/CoreFlow.Core.Application/Interfaces/Repositories/Domain/`.
- Mapeamento em `src/libs/core/CoreFlow.Core.Application/Mappings/ApplicationReadModelsProfile.cs`.
- Registro de handlers em `src/libs/core/CoreFlow.Core.Application/Extensions/ServiceCollectionExtensions.cs`.

- Infrastructure
- Data entity em `src/libs/infrastructure/CoreFlow.Infrastructure/Models/` herdando `DataEntityBase<TId>`.
- Configuração EF em `src/libs/infrastructure/CoreFlow.Infrastructure/EntityTypeConfigurations/`.
- Mapeamento Domain ⇄ Infra em `src/libs/infrastructure/CoreFlow.Infrastructure/Mappings/DomainToInfrastructureProfile.cs`.
- Repositório concreto em `src/libs/infrastructure/CoreFlow.Infrastructure/Repositories/`.
- Registro de repositório em `src/libs/infrastructure/CoreFlow.Infrastructure/Extensions/ServiceCollectionExtensions.cs`.
- `DbContext` com `ApplyConfiguration` no `ApplicationDbContext.OnModelCreating`.

- Contracts
- DTOs em `src/libs/core/CoreFlow.Core.Contracts/<Entidade>/`.

- Api
- Endpoints em `src/api/CoreFlow.Api/Endpoints/<Entidade>Endpoints.cs`.
- Mapeamentos de resposta em `src/api/CoreFlow.Api/Extensions/`.
- Registro em `src/api/CoreFlow.Api/Extensions/EndpointsExtensions.cs`.

---

**Ordem de implementação (evita erro de compilação)**

1. Domain
2. Application
3. Contracts
4. Infrastructure
5. Api
6. Ajustes finais de DI, mapeamentos e `DbContext`

Se você pular passos, o build pode quebrar. **Não faça.**

---

**Regras para handlers e mensagens**

- Toda mensagem de Application deve implementar `IRequest<TResponse>`.
- Todo handler deve herdar `BaseMessageHandler<TRequest, TResponse>`.
- O método obrigatório é `HandleAsync(TRequest, CancellationToken)`.
- Handlers devem retornar `Result`/`Result<T>` e usar `ResultExtensions`.
- A Api **sempre** usa `IServiceBus` para enviar comandos/queries.
- A Api nunca chama repositórios diretamente.

---

**Eventos de domínio e Outbox**

- Aggregate deve emitir eventos via `RaiseEvent`.
- Em comandos que geram eventos, persistir na Outbox e limpar a fila local, seguindo o padrão do `CreateCustomerCommandHandler`.

---

**Não faça**

- Não crie novas camadas, novos projetos base ou pastas paralelas.
- Não mova arquivos para fora da estrutura canônica.
- Não coloque DTOs dentro de Domain ou Application.
- Não acesse banco, EF ou Dapper dentro de Application ou Domain.
- Não altere heranças base (`AggregateRootBase`, `DomainEventBase`, `BaseMessageHandler`).

---

**Quando ficar em dúvida**

- Compare com o fluxo `Customer` e replique a estrutura.
- Se ainda estiver incerto, **pare e pergunte**.
