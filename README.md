# CoreFlow – Arquitetura de Referência (.NET)

> **Leia antes de usar.** Este repositório **não é um projeto de produto**. Ele é o **template arquitetural obrigatório** da CoreFlow.

CoreFlow foi criada para ser **replicada**, não reinterpretada. A estrutura e as dependências **são parte da regra**. Se mudar a estrutura, **deixa de ser CoreFlow**.

---

**O que você encontra aqui**

- Um template completo de arquitetura com camadas, contratos, padrões de resultado e integração.
- Um fluxo canônico (`Customer`) que **define como todo domínio deve ser implementado**.
- Regras de dependência e organização que **não podem ser quebradas**.

---

**O que é obrigatório**

- Todos os projetos em `src/libs/shared`.
- O projeto `src/libs/infrastructure/CoreFlow.Infrastructure.Auditing`.
- A estrutura de camadas e as heranças base.

A única parte de negócio que é apenas um exemplo é o **conteúdo do fluxo `Customer`**. A **estrutura** do fluxo **não é opcional**.

---

**Visão geral da solução**

```
src/
  CoreFlow.slnx
  api/
    CoreFlow.Api/
  libs/
    core/
      CoreFlow.Core.Domain/
      CoreFlow.Core.Application/
      CoreFlow.Core.Contracts/
    infrastructure/
      CoreFlow.Infrastructure/
      CoreFlow.Infrastructure.Auditing/
    shared/
      CoreFlow.Shared.Results/
      CoreFlow.Shared.Extensions/
      CoreFlow.Shared.Logging/
      CoreFlow.Shared.Exceptions/
      CoreFlow.Shared.Email/
```

---

**Camadas e responsabilidades**

Api (`*.Api`)
Responsável por endpoints, DI e tradução HTTP ⇄ Application.
Não implementa regras de negócio.

Core.Domain (`*.Core.Domain`)
Aggregates, entidades, value objects, regras puras.
Não conhece Application, Infrastructure ou Api.

Core.Application (`*.Core.Application`)
Commands/Queries, Handlers, interfaces de repositório, orquestração.
Usa `Result` (`CoreFlow.Shared.Results`).

Core.Contracts (`*.Core.Contracts`)
DTOs e contratos externos.
Não depende de Application nem Infrastructure.

Infrastructure (`*.Infrastructure`)
Persistência, integrações, implementações de repositório, outbox.
Depende de Application e Domain.

Infrastructure.Auditing (`*.Infrastructure.Auditing`)
Auditoria obrigatória para toda persistência.

Shared (`*.Shared.*`)
Projetos transversais obrigatórios, usados pela solução inteira.

---

**Regras de dependência (não quebrar)**

```
Domain -> (nenhuma dependência de camadas internas)
Application -> Domain + Shared.Results
Contracts -> (isolado)
Infrastructure -> Application + Domain + Infrastructure.Auditing
Api -> Application + Infrastructure + Contracts + Shared.*
Shared.* -> (isolado, sem depender de Core/Infrastructure/Api)
```

---

**Customer é o fluxo canônico**

O módulo `Customer` **não é descartável** e **não é opcional**. Ele é **o modelo estrutural** da CoreFlow. Ao criar outro domínio (ex.: Lead, Ticket, Content), você deve **repetir exatamente a mesma estrutura**, trocando apenas o negócio.

Se não parecer com `Customer` estruturalmente, **não é CoreFlow**.

---

**Fluxo completo (Customer) – visão prática**

Domain
- Aggregate: `src/libs/core/CoreFlow.Core.Domain/Entities/CustomerAggregate/Customer.cs`
- Evento: `src/libs/core/CoreFlow.Core.Domain/Entities/CustomerAggregate/CustomerCreatedEvent.cs`

Application
- Command: `src/libs/core/CoreFlow.Core.Application/Commands/Customers/CreateCustomerCommand.cs`
- Query: `src/libs/core/CoreFlow.Core.Application/Queries/Customers/GetCustomerByIdQuery.cs`
- ReadModel: `src/libs/core/CoreFlow.Core.Application/ReadModels/Customer/CustomerReadModel.cs`
- Mapeamento: `src/libs/core/CoreFlow.Core.Application/Mappings/ApplicationReadModelsProfile.cs`
- Interface de repositório: `src/libs/core/CoreFlow.Core.Application/Interfaces/Repositories/Domain/ICustomerRepository.cs`
- Registro dos handlers: `src/libs/core/CoreFlow.Core.Application/Extensions/ServiceCollectionExtensions.cs`

Infrastructure
- Entidade de dados: `src/libs/infrastructure/CoreFlow.Infrastructure/Models/CustomerEntity.cs`
- Mapping Domain ⇄ Infra: `src/libs/infrastructure/CoreFlow.Infrastructure/Mappings/DomainToInfrastructureProfile.cs`
- Configuração EF: `src/libs/infrastructure/CoreFlow.Infrastructure/EntityTypeConfigurations/CustomerConfiguration.cs`
- Repositório: `src/libs/infrastructure/CoreFlow.Infrastructure/Repositories/CustomerRepository.cs`
- DbContext: `src/libs/infrastructure/CoreFlow.Infrastructure/DbContexts/ApplicationDbContext.cs`
- DI Infra: `src/libs/infrastructure/CoreFlow.Infrastructure/Extensions/ServiceCollectionExtensions.cs`

Contracts
- Request: `src/libs/core/CoreFlow.Core.Contracts/Customers/CustomerCreateRequest.cs`
- Response: `src/libs/core/CoreFlow.Core.Contracts/Customers/CustomerResponse.cs`

Api
- Endpoints: `src/api/CoreFlow.Api/Endpoints/CustomerEndpoints.cs`
- Mapper de resposta: `src/api/CoreFlow.Api/Extensions/CustomerExtensions.cs`
- Registro de endpoints: `src/api/CoreFlow.Api/Extensions/EndpointsExtensions.cs`

---

**Como criar um novo fluxo (obrigatório seguir a ordem)**

Passo 1 — Domain
- Criar Aggregate e eventos em `src/libs/core/CoreFlow.Core.Domain/Entities/<Entidade>Aggregate/`.
- Implementar eventos com `DomainEventBase<TId>` e `IDomainEventHandler<TEvent>`.

Passo 2 — Application
- Criar `Command` e `Query` em `src/libs/core/CoreFlow.Core.Application/Commands/<Entidade>/` e `.../Queries/<Entidade>/`.
- Todos os handlers devem herdar `BaseMessageHandler<,>`.
- Criar `ReadModel` e mapear no `ApplicationReadModelsProfile`.
- Criar interface de repositório em `Interfaces/Repositories/Domain`.
- Registrar handlers no `AddApplicationLayer`.

Passo 3 — Contracts
- Criar DTOs em `src/libs/core/CoreFlow.Core.Contracts/<Entidade>/`.

Passo 4 — Infrastructure
- Criar entidade de dados em `Models` herdando `DataEntityBase<TId>`.
- Criar `EntityTypeConfiguration`.
- Mapear Domain ⇄ Infra em `DomainToInfrastructureProfile`.
- Criar repositório concreto e registrar em `AddRepositories`.
- Registrar a configuração no `ApplicationDbContext.OnModelCreating`.

Passo 5 — Api
- Criar endpoints em `src/api/CoreFlow.Api/Endpoints/<Entidade>Endpoints.cs`.
- Criar extensões de mapeamento para `Result` e `Response`.
- Registrar o endpoint em `MapApplicationEndpoints`.

Passo 6 — Resultado e Outbox
- Use `Result` e `Result<T>` com `ResultExtensions`.
- Para comandos que geram eventos, persistir na Outbox e limpar a fila local.

---

**Regras obrigatórias (resumo)**

- A arquitetura e heranças base são imutáveis sem decisão explícita.
- Shared e Auditing são obrigatórios.
- Api não fala com Domain nem Infrastructure diretamente.
- Application não conhece Infrastructure.
- Domain não conhece mais ninguém.
- `Customer` é o template estrutural obrigatório.

---

**Documentos importantes**

- `README.md` → guia humano
- `RULES_FOR_AI.md` → regras mandatórias para qualquer IA

Em caso de conflito entre documentação e código, **o código vence**.

---

**Aviso final**

CoreFlow existe para **reduzir ambiguidade**, **forçar consistência** e **evitar desvios de arquitetura**. Se você busca liberdade total de estrutura, **não use CoreFlow**.
