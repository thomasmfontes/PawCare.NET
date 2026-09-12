# PawCare API

API RESTful desenvolvida para o Challenge da disciplina de Advanced Business Development with .NET.

## Integrantes

- Thomas Fontes - RM: 562254
- Matheus Molina - RM: 563399
- Gabriel Maciel - RM: 562795
- Vitória Rodrigues - RM: 565160
- Augusto Bonomo - RM: 565155

---

## Descrição do Projeto

O **PawCare API** é uma aplicação backend desenvolvida em **ASP.NET Core Web API**, com integração ao banco de dados **Oracle** utilizando **Entity Framework Core**.

O sistema tem como objetivo gerenciar informações relacionadas ao cuidado de pets, permitindo o cadastro e a consulta de tutores, raças, pets, clínicas veterinárias, médicos especialistas, tratamentos, eventos clínicos e históricos clínicos.

A API foi pensada para apoiar o aplicativo PawCare, oferecendo uma base estruturada para o acompanhamento da saúde dos animais, centralizando dados clínicos e relacionamentos importantes entre tutor, pet, clínica e profissional responsável.

---

## Objetivo da Solução

A proposta do PawCare é facilitar o gerenciamento da saúde dos pets por meio de uma API organizada, escalável e integrada ao banco de dados Oracle.

A solução permite:

- Cadastrar tutores.
- Cadastrar pets vinculados a tutores.
- Associar pets a raças.
- Registrar clínicas veterinárias.
- Registrar médicos especialistas.
- Registrar tratamentos por pet.
- Registrar eventos clínicos.
- Gerar histórico clínico a partir dos eventos.
- Consultar informações por parâmetros como CPF do tutor, raça, espécie, cidade, tipo de evento e status.

---

## Tecnologias Utilizadas

- C# (.NET 10)
- ASP.NET Core Web API
- Entity Framework Core
- Oracle Database & Oracle.EntityFrameworkCore
- In-Memory Database (para testes automatizados isolados)
- Swagger / OpenAPI com documentação rica e XML comments
- Serilog (Logging estruturado em Console e Arquivo com Correlation ID)
- OpenTelemetry (Distributed Tracing e Métricas de desempenho)
- Microsoft.Extensions.Diagnostics.HealthChecks (Endpoints de integridade)
- xUnit, Moq e FluentAssertions (Testes Unitários com padrão AAA)
- WebApplicationFactory & Fixtures (Testes de Integração de endpoints)
- Migrations
- Visual Studio / VS Code
- .NET CLI

---

## Arquitetura do Projeto

O projeto adota uma arquitetura modular em camadas, com separação clara de responsabilidades, incluindo observabilidade, diagnósticos e projetos de testes automatizados dedicados:

```txt
PawCareApi/
│
├── Controllers/                         # Controladores REST com anotações e XML Docs Swagger
│   ├── TutoresController.cs
│   ├── RacasController.cs
│   ├── PetsController.cs
│   ├── ClinicasController.cs
│   ├── MedicosEspecialistasController.cs
│   ├── TratamentosController.cs
│   ├── EventosController.cs
│   └── HistoricosClinicosController.cs
│
├── Middlewares/                         # Middlewares da aplicação
│   └── CorrelationIdMiddleware.cs       # Rastreamento de requisições com X-Correlation-ID
│
├── HealthChecks/                        # Verificação de saúde da aplicação
│   ├── ExternalServicesHealthCheck.cs   # Health check de serviços externos/internet
│   └── HealthCheckResponseWriter.cs     # Serializador JSON formatado para diagnósticos
│
├── Diagnostics/                         # Instrumentação de telemetria
│   └── PawCareMetrics.cs                # Contadores e histogramas OpenTelemetry
│
├── Data/
│   └── PawCareContext.cs                # DbContext da aplicação
│
├── Models/                              # Modelos de domínio
│   ├── Tutor.cs
│   ├── Raca.cs
│   ├── Pet.cs
│   ├── Clinica.cs
│   ├── MedicoEspecialista.cs
│   ├── Tratamento.cs
│   ├── Evento.cs
│   └── HistoricoClinico.cs
│
├── Mappings/                            # Fluent API mappings para Oracle
│   ├── TutorMapping.cs
│   ├── RacaMapping.cs
│   ├── PetMapping.cs
│   ├── ClinicaMapping.cs
│   ├── MedicoEspecialistaMapping.cs
│   ├── TratamentoMapping.cs
│   ├── EventoMapping.cs
│   └── HistoricoClinicoMapping.cs
│
├── Migrations/                          # Migrações do banco de dados
├── tests/                               # Projetos de Testes Automatizados (Padrão AAA)
│   ├── PawCareApi.Tests.Unit/           # Testes Unitários (xUnit + Moq)
│   │   ├── Domain/                      # Testes de regras de negócio das entidades
│   │   ├── Controllers/                 # Testes unitários dos controladores
│   │   └── Fixtures/                    # TestDbContextFixture & UnitTestCollection
│   │
│   └── PawCareApi.Tests.Integration/    # Testes de Integração (WebApplicationFactory)
│       ├── Endpoints/                   # Testes de fluxo HTTP completo (Pets, Tutores, Health)
│       └── Fixtures/                    # CustomWebApplicationFactory & IntegrationTestCollection
│
├── logs/                                # Arquivos de log estruturados (gerados em runtime)
├── Program.cs                           # Inicialização, Serilog, OpenTelemetry e HealthChecks
├── appsettings.json
└── README.md
```
---

## Entidades Implementadas

A API implementa as seguintes entidades principais:

- Tutor
- Raça
- Pet
- Clínica
- Médico Especialista
- Tratamento
- Evento
- Histórico Clínico

---

## Relacionamentos Principais

O banco de dados foi modelado com os seguintes relacionamentos:

- Um **Tutor** pode possuir vários **Pets**.
- Uma **Raça** pode estar associada a vários **Pets**.
- Um **Pet** pertence a um **Tutor**.
- Um **Pet** pertence a uma **Raça**.
- Um **Pet** pode possuir vários **Tratamentos**.
- Um **Pet** pode participar de vários **Eventos**.
- Um **Evento** é vinculado a um **Pet**.
- Um **Evento** é vinculado a um **Tutor**.
- Um **Evento** é vinculado a um **Médico Especialista**.
- Um **Evento** é vinculado a uma **Clínica**.
- Um **Evento** pode gerar um **Histórico Clínico**.
- Uma **Clínica** pode sediar vários **Eventos**.
- Um **Médico Especialista** pode realizar vários **Eventos**.

---

## Banco de Dados

O banco de dados utilizado foi o **Oracle**.

As tabelas foram criadas utilizando **Entity Framework Core Migrations**.

Principais tabelas criadas:

```txt
TUTOR
RACA
PET
CLINICA
MEDICO_ESPECIALISTA
TRATAMENTO
EVENTO
HISTORICO_CLINICO
__EFMigrationsHistory
```

A tabela `__EFMigrationsHistory` é criada automaticamente pelo Entity Framework Core para controlar as migrations aplicadas no banco.

---

## Configuração da Conexão Oracle

A conexão com o banco Oracle é configurada no arquivo `appsettings.json`.

Exemplo:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=oracle.fiap.com.br:1521/ORCL"
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "AllowedHosts": "*"
}
```

> Por segurança, a string de conexão real não deve ser exposta publicamente em repositórios públicos.

---

## Migrations

Foram utilizadas migrations para criação e atualização das tabelas no Oracle.

Comandos utilizados:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Depois, para adicionar as entidades clínicas:

```bash
dotnet ef migrations add AddClinicalEntities
dotnet ef database update
```

---

## Endpoints da API

A API possui endpoints REST para operações de cadastro, consulta, atualização e remoção.

---

## Tutores

| Método | Endpoint | Descrição | Retornos |
|---|---|---|---|
| GET | `/api/Tutores` | Lista todos os tutores | 200 |
| GET | `/api/Tutores/{cpf}` | Busca tutor por CPF | 200, 404 |
| GET | `/api/Tutores/email/{email}` | Busca tutor por e-mail | 200, 404 |
| POST | `/api/Tutores` | Cadastra um tutor | 201, 400 |
| PUT | `/api/Tutores/{cpf}` | Atualiza um tutor | 204, 400, 404 |
| DELETE | `/api/Tutores/{cpf}` | Remove um tutor | 204, 400, 404 |

### Exemplo de cadastro de tutor

```json
{
  "cpf": "12345678900",
  "nome": "Maria Oliveira",
  "telefone": "11999999999",
  "email": "maria@email.com",
  "qtdPets": 0,
  "pets": []
}
```

---

## Raças

| Método | Endpoint | Descrição | Retornos |
|---|---|---|---|
| GET | `/api/Racas` | Lista todas as raças | 200 |
| GET | `/api/Racas/{id}` | Busca raça por ID | 200, 404 |
| GET | `/api/Racas/especie/{especie}` | Lista raças por espécie | 200 |
| POST | `/api/Racas` | Cadastra uma raça | 201 |
| PUT | `/api/Racas/{id}` | Atualiza uma raça | 204, 400, 404 |
| DELETE | `/api/Racas/{id}` | Remove uma raça | 204, 400, 404 |

### Exemplo de cadastro de raça

```json
{
  "nome": "Golden Retriever",
  "especie": "Cachorro",
  "pets": []
}
```

---

## Pets

| Método | Endpoint | Descrição | Retornos |
|---|---|---|---|
| GET | `/api/Pets` | Lista todos os pets | 200 |
| GET | `/api/Pets/{id}` | Busca pet por ID | 200, 404 |
| GET | `/api/Pets/tutor/{cpf}` | Lista pets por tutor | 200, 404 |
| GET | `/api/Pets/raca/{racaId}` | Lista pets por raça | 200, 404 |
| GET | `/api/Pets/status/{status}` | Lista pets por status de longevidade | 200 |
| POST | `/api/Pets` | Cadastra um pet | 201, 400 |
| PUT | `/api/Pets/{id}` | Atualiza um pet | 204, 400, 404 |
| DELETE | `/api/Pets/{id}` | Remove um pet | 204, 404 |

### Exemplo de cadastro de pet

```json
{
  "nome": "Luna",
  "dataNascimento": "2021-05-10T00:00:00",
  "peso": 18.5,
  "statusLongevidade": "Adulto",
  "racaId": 1,
  "tutorCpf": "12345678900"
}
```

---

## Clínicas

| Método | Endpoint | Descrição | Retornos |
|---|---|---|---|
| GET | `/api/Clinicas` | Lista todas as clínicas | 200 |
| GET | `/api/Clinicas/{id}` | Busca clínica por ID | 200, 404 |
| GET | `/api/Clinicas/cidade/{cidade}` | Lista clínicas por cidade | 200 |
| GET | `/api/Clinicas/atendimento-24h` | Lista clínicas com atendimento 24h | 200 |
| POST | `/api/Clinicas` | Cadastra uma clínica | 201 |
| PUT | `/api/Clinicas/{id}` | Atualiza uma clínica | 204, 400, 404 |
| DELETE | `/api/Clinicas/{id}` | Remove uma clínica | 204, 400, 404 |

### Exemplo de cadastro de clínica

```json
{
  "nomeCnpj": "Clínica Pet Vida - 12345678000199",
  "telefone": "1133334444",
  "latitude": -23.55052,
  "longitude": -46.633308,
  "bairro": "Centro",
  "cidade": "São Paulo",
  "estado": "SP",
  "atendimento24h": true
}
```

---

## Médicos Especialistas

| Método | Endpoint | Descrição | Retornos |
|---|---|---|---|
| GET | `/api/MedicosEspecialistas` | Lista todos os médicos | 200 |
| GET | `/api/MedicosEspecialistas/{id}` | Busca médico por ID | 200, 404 |
| GET | `/api/MedicosEspecialistas/especialidade/{especialidade}` | Lista médicos por especialidade | 200 |
| POST | `/api/MedicosEspecialistas` | Cadastra um médico | 201 |
| PUT | `/api/MedicosEspecialistas/{id}` | Atualiza um médico | 204, 400, 404 |
| DELETE | `/api/MedicosEspecialistas/{id}` | Remove um médico | 204, 400, 404 |

### Exemplo de cadastro de médico especialista

```json
{
  "nome": "Dra. Ana Souza",
  "especialidade": "Dermatologia Veterinária"
}
```

---

## Tratamentos

| Método | Endpoint | Descrição | Retornos |
|---|---|---|---|
| GET | `/api/Tratamentos` | Lista todos os tratamentos | 200 |
| GET | `/api/Tratamentos/{id}` | Busca tratamento por ID | 200, 404 |
| GET | `/api/Tratamentos/pet/{idPet}` | Lista tratamentos por pet | 200, 404 |
| POST | `/api/Tratamentos` | Cadastra um tratamento | 201, 400 |
| PUT | `/api/Tratamentos/{id}` | Atualiza um tratamento | 204, 400, 404 |
| DELETE | `/api/Tratamentos/{id}` | Remove um tratamento | 204, 404 |

### Exemplo de cadastro de tratamento

```json
{
  "idPet": 1,
  "nomeMedicamento": "Antibiótico Veterinário",
  "frequencia": "A cada 12 horas",
  "dataInicio": "2026-05-20T00:00:00",
  "dataFinal": "2026-05-27T00:00:00"
}
```

---

## Eventos

| Método | Endpoint | Descrição | Retornos |
|---|---|---|---|
| GET | `/api/Eventos` | Lista todos os eventos | 200 |
| GET | `/api/Eventos/{id}` | Busca evento por ID | 200, 404 |
| GET | `/api/Eventos/pet/{idPet}` | Lista eventos por pet | 200, 404 |
| GET | `/api/Eventos/tutor/{cpf}` | Lista eventos por tutor | 200, 404 |
| GET | `/api/Eventos/tipo/{tipo}` | Lista eventos por tipo | 200 |
| POST | `/api/Eventos` | Cadastra um evento | 201, 400 |
| PUT | `/api/Eventos/{id}` | Atualiza um evento | 204, 400, 404 |
| DELETE | `/api/Eventos/{id}` | Remove um evento | 204, 400, 404 |

### Exemplo de cadastro de evento

```json
{
  "tipo": "Consulta",
  "idPet": 1,
  "idTutor": "12345678900",
  "idMedico": 1,
  "idClinica": 1
}
```

---

## Históricos Clínicos

| Método | Endpoint | Descrição | Retornos |
|---|---|---|---|
| GET | `/api/HistoricosClinicos` | Lista todos os históricos | 200 |
| GET | `/api/HistoricosClinicos/{id}` | Busca histórico por ID | 200, 404 |
| GET | `/api/HistoricosClinicos/evento/{idEvento}` | Busca histórico por evento | 200, 404 |
| GET | `/api/HistoricosClinicos/status/{status}` | Lista históricos por status | 200 |
| POST | `/api/HistoricosClinicos` | Cadastra um histórico clínico | 201, 400 |
| PUT | `/api/HistoricosClinicos/{id}` | Atualiza um histórico clínico | 204, 400, 404 |
| DELETE | `/api/HistoricosClinicos/{id}` | Remove um histórico clínico | 204, 404 |

### Exemplo de cadastro de histórico clínico

```json
{
  "idEvento": 1,
  "dataEvento": "2026-05-20T00:00:00",
  "dataVencimento": "2026-06-20T00:00:00",
  "status": "Concluído",
  "observacoesIa": "Consulta registrada com recomendação de acompanhamento."
}
```

---

## Retornos HTTP Utilizados

A API utiliza os seguintes códigos HTTP:

| Código | Retorno | Uso |
|---|---|---|
| 200 | OK | Consultas realizadas com sucesso |
| 201 | Created | Registro criado com sucesso |
| 204 | No Content | Atualização ou exclusão realizada com sucesso |
| 400 | Bad Request | Dados inválidos, inconsistentes ou violação de regra de negócio |
| 404 | Not Found | Registro não encontrado |

---

## Validações Implementadas

Foram implementadas validações para evitar inconsistências no banco de dados.

Exemplos:

- Não permite cadastrar pet com tutor inexistente.
- Não permite cadastrar pet com raça inexistente.
- Não permite cadastrar tratamento com pet inexistente.
- Não permite cadastrar evento com pet inexistente.
- Não permite cadastrar evento com tutor inexistente.
- Não permite cadastrar evento com médico inexistente.
- Não permite cadastrar evento com clínica inexistente.
- Não permite cadastrar evento se o pet não pertencer ao tutor informado.
- Não permite cadastrar mais de um histórico clínico para o mesmo evento.
- Não permite remover tutor que possui pets cadastrados.
- Não permite remover raça vinculada a pets.
- Não permite remover clínica vinculada a eventos.
- Não permite remover médico vinculado a eventos.
- Não permite remover evento que possui histórico clínico.

---

## Exemplos de Testes Realizados

| Teste | Endpoint | Resultado |
|---|---|---|
| Cadastro de tutor | `POST /api/Tutores` | 201 Created |
| Consulta de pets | `GET /api/Pets` | 200 OK |
| Consulta de pet inexistente | `GET /api/Pets/9999` | 404 Not Found |
| Cadastro de pet com raça inexistente | `POST /api/Pets` | 400 Bad Request |
| Atualização de pet | `PUT /api/Pets/1` | 204 No Content |
| Cadastro de evento | `POST /api/Eventos` | 201 Created |
| Consulta de eventos por tutor | `GET /api/Eventos/tutor/12345678900` | 200 OK |

---

## Como Executar o Projeto

### 1. Clonar o repositório

```bash
git clone https://github.com/thomasmfontes/PawCare.NET.git
```

### 2. Acessar a pasta do projeto

```bash
cd PawCareApi
```

### 3. Restaurar os pacotes

```bash
dotnet restore
```

### 4. Configurar a string de conexão

No arquivo `appsettings.json`, configurar a conexão Oracle:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=SECRETO;Password=SECRETO;Data Source=oracle.fiap.com.br:1521/ORCL"
  }
}
```

### 5. Aplicar as migrations

```bash
dotnet ef database update
```

### 6. Executar a aplicação

```bash
dotnet run
```

### 7. Acessar o Swagger

Acessar no navegador:

```txt
http://localhost:{porta}/swagger
```

Exemplo:

```txt
http://localhost:5115/swagger
```

---

## Documentação Detalhada da API (Swagger / OpenAPI)

A API conta com documentação interativa avançada gerada pelo **Swashbuckle / OpenAPI**, enriquecida por comentários XML compilados a partir do código-fonte (`PawCareApi.xml`):

- **Descrições Completas**: Todos os controladores e endpoints possuem descrições de propósito, parâmetros de rota/query e corpo da requisição.
- **Códigos de Resposta HTTP Documentados**: Cada método implementa anotações explícitas `[ProducesResponseType]` e tags XML `<response code="...">`, cobrindo:
  - `200 OK`: Sucesso em consultas.
  - `201 Created`: Criação de recursos com retorno da rota no header `Location`.
  - `204 NoContent`: Atualizações e exclusões bem-sucedidas.
  - `400 BadRequest`: Validações de payload, campos obrigatórios, inconsistências de IDs ou chaves duplicadas.
  - `404 NotFound`: Entidades ou parâmetros não localizados na base.
- **Modelos e DTOs Tipados**: Esquemas de dados exibidos com tipos primitivos, restrições e relacionamentos claros na interface gráfica do Swagger.

---

## Monitoramento e Observabilidade

A aplicação implementa uma camada abrangente de observabilidade, integrando monitoramento de integridade (Health Checks), registro de eventos estruturados (Logging) e telemetria distribuída (Tracing e Métricas).

### 1. Health Checks (`Microsoft.Extensions.Diagnostics.HealthChecks`)

Permite a sondagem em tempo real da saúde da aplicação e de suas dependências críticas:

| Endpoint | Finalidade | Verificações Inclusas |
| :--- | :--- | :--- |
| `/health` | Diagnóstico geral em formato JSON | Todas as sondas registradas (DB + Serviços Externos) |
| `/health/ready` | Sonda de Prontidão (Readiness Probe) | Conectividade com o banco Oracle e dependências externas |
| `/health/live` | Sonda de Vitalidade (Liveness Probe) | Verifica se o processo da aplicação ASP.NET Core está ativo |

#### Exemplo de Resposta JSON (`/health` e `/health/ready`):
```json
{
  "status": "Healthy",
  "totalDuration": "12.45ms",
  "timestamp": "2026-09-12T01:50:00.000Z",
  "entries": [
    {
      "component": "oracle-database",
      "status": "Healthy",
      "description": null,
      "duration": "8.12ms",
      "exception": null,
      "data": {}
    },
    {
      "component": "external-services",
      "status": "Healthy",
      "description": "Serviço externo de notificações/integração respondendo com sucesso.",
      "duration": "4.33ms",
      "exception": null,
      "data": {}
    }
  ]
}
```

### 2. Logging Estruturado com Serilog

Configurado para geração de logs em formato estruturado com contextualização em múltiplos destinos:

- **Correlation ID Middleware**: Cada requisição recebe um `X-Correlation-ID` único (gerado ou propagado pelo cabeçalho HTTP), permitindo rastrear todo o ciclo de vida da requisição entre diferentes camadas e microsserviços.
- **Destinos (Sinks)**:
  - **Console**: Saída legível e colorida para facilitar o desenvolvimento local.
  - **Arquivo Rotativo**: Armazenamento diário em `logs/pawcare-.log` com política de retenção de 14 dias.
- **Níveis de Severidade**:
  - `Information`: Ciclo de vida da aplicação e requisições HTTP concluídas.
  - `Warning`: Validações de negócio frustradas ou degradações controladas.
  - `Error`: Exceções não tratadas e falhas críticas de infraestrutura.

### 3. Distributed Tracing e Métricas com OpenTelemetry

A aplicação expõe métricas de telemetria e rastreamento distribuído através do padrão **OpenTelemetry**:

- **Distributed Tracing**:
  - Instrumentação automática do pipeline ASP.NET Core e de chamadas `HttpClient`.
  - Captura e propagação de contexto de traces entre chamadas de rede e camadas da aplicação.
- **Métricas de Desempenho (`PawCareMetrics`)**:
  - Contadores de requisições: `pawcare_requests_total`.
  - Contadores de taxa de erros: `pawcare_errors_total`.
  - Histogramas de duração de operações: `pawcare_operation_duration_seconds`.
  - Métricas de runtime do .NET e roteamento ASP.NET Core (`http.request.duration`, `http.client.request.duration`, `dns.lookup.duration`).

---

## Testes Automatizados – Padrão AAA

A solução possui cobertura completa de testes automatizados organizados em projetos dedicados por camada, garantindo que novas funcionalidades possam ser validadas com segurança e isolamento.

### Organização dos Testes

1. **`tests/PawCareApi.Tests.Unit` (Testes Unitários)**:
   - Framework: **xUnit** com asserções fluidas via **FluentAssertions** e mocks via **Moq**.
   - Foco: Regras de negócio de domínio (`PetDomainTests`, `TutorDomainTests`) e controladores em isolamento (`PetsControllerTests`, `TutoresControllerTests`, `ClinicasControllerTests`).
   - Utilização de `TestDbContextFixture` com banco de dados em memória isolado por execução.
2. **`tests/PawCareApi.Tests.Integration` (Testes de Integração)**:
   - Framework: **xUnit** + `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory<Program>`).
   - Foco: Testar o pipeline HTTP completo dos endpoints (`PetsEndpointTests`, `TutoresEndpointTests`, `HealthCheckEndpointTests`), validando cabeçalhos, códigos de status (200, 201, 204, 400, 404) e serialização JSON.
   - Utilização de **Class Fixtures** e **Collection Fixtures** (`IntegrationTestCollection`) para inicialização eficiente do servidor de testes e compartilhamento de contexto sem overhead de re-instanciação.

### Padrão AAA (Arrange, Act, Assert) e Nomenclatura

Todos os métodos de teste seguem rigorosamente a convenção:
`MetodoTestado_Cenario_ResultadoEsperado`

E a estrutura de blocos explícita:
```csharp
[Fact]
public async Task Create_QuandoDadosValidos_DeveRetornarCreatedEIncrementarQtdPetsDoTutor()
{
    // Arrange (Preparação do cenário e dos dados)
    using var context = _fixture.CreateContext();
    var tutor = new Tutor { Cpf = "33333333333", Nome = "Roberto", ... };
    ...

    // Act (Execução da ação a ser testada)
    var result = await controller.Create(novoPet);

    // Assert (Validação dos resultados esperados)
    var createdResult = result.Result as CreatedAtActionResult;
    createdResult.Should().NotBeNull();
    createdResult!.StatusCode.Should().Be(201);
}
```

### Como Executar os Testes

Execute na raiz da solução:

```bash
# Executar toda a suíte de testes (Unit + Integration)
dotnet test

# Executar com saída detalhada no console
dotnet test --logger "console;verbosity=detailed"

# Executar apenas os testes unitários
dotnet test tests/PawCareApi.Tests.Unit

# Executar apenas os testes de integração
dotnet test tests/PawCareApi.Tests.Integration
```

---

## Comandos Úteis

### Compilar o projeto

```bash
dotnet build
```

### Rodar o projeto

```bash
dotnet run
```

### Criar uma migration

```bash
dotnet ef migrations add NomeDaMigration
```

### Aplicar migrations no banco

```bash
dotnet ef database update
```

### Listar migrations

```bash
dotnet ef migrations list
```

---

## Observações Técnicas

Durante o desenvolvimento, foi necessário configurar a serialização JSON para evitar ciclos de referência entre entidades relacionadas.

Exemplo de ciclo evitado:

```txt
Pet -> Raca -> Pets -> Raca -> Pets
```

A configuração foi aplicada no `Program.cs` utilizando:

```csharp
using System.Text.Json.Serialization;

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
```

Também foi necessário mapear campos booleanos para `NUMBER(1)` no Oracle, pois o Oracle não trabalha da mesma forma que o C# com propriedades booleanas em tabelas relacionais.

Exemplo:

```csharp
builder.Property(c => c.Atendimento24h)
    .HasColumnName("ATENDIMENTO_24H")
    .HasConversion<int>()
    .HasColumnType("NUMBER(1)");
```

---

## Conclusão

O projeto PawCare API entrega uma solução backend funcional, estruturada e integrada ao banco de dados Oracle.

A API implementa operações CRUD, rotas RESTful parametrizadas, relacionamentos entre entidades, validações de consistência, migrations com Entity Framework Core e documentação/testes via Swagger.

Com isso, a aplicação atende aos principais requisitos do Challenge, demonstrando domínio de ASP.NET Core, Entity Framework Core, Oracle Database, arquitetura de API REST e boas práticas básicas de desenvolvimento backend.
