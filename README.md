# Product Management - Full Stack

Aplicacao full stack para gestao de catalogo de produtos com foco em boas praticas de arquitetura, validacao, testes e experiencia de uso.

## Stack
- Backend: .NET 8, ASP.NET Core Web API, EF Core, FluentValidation, Serilog
- Frontend: Angular 17 (Standalone Components + Signals + Reactive Forms + RxJS + Angular Material)
- Banco: SQLite
- DevOps: Docker, Docker Compose, GitHub Actions

## Pre-requisitos
- .NET SDK 8.0+
- Node.js 20+ e npm 10+
- Docker Desktop (opcional, para execucao em containers)

## Estrutura da Solucao
### Backend
- ProductManagement.Domain
- ProductManagement.Application
- ProductManagement.Infrastructure
- ProductManagement.Api
- ProductManagement.Application.Tests
- ProductManagement.Api.IntegrationTests

### Frontend
- product-management-web

## Funcionalidades
- Cadastro, edicao e consulta de produtos
- Ciclo de vida de status do produto (Active/Inactive) com acoes de ativar/inativar
- Listagem com paginacao server-side, filtros e ordenacao
- Home inicial e navegacao para catalogo
- Criacao e edicao de produto em modal
- Swagger com exemplos de payload
- Tratamento global de erros em Problem Details (RFC 7807)

## Como Rodar
## Opcao 1: Docker (API + Frontend)
```bash
docker compose up --build
```

Acessos:
- API Swagger: http://localhost:8080/swagger
- Frontend: http://localhost:4200

## Opcao 2: Local (Backend + Frontend separados)
### Backend
```bash
dotnet restore ProductManagement.slnx --configfile NuGet.Config
dotnet build ProductManagement.slnx
dotnet test ProductManagement.slnx --no-build
dotnet run --project ProductManagement.Api
```

Por padrao (perfil https), a API sobe em:
- https://localhost:7190
- http://localhost:5072

### Frontend
```bash
cd product-management-web
npm install
npm run build
npm test -- --runInBand
npm start
```

O frontend local roda em:
- http://localhost:4200

## Configuracao de Ambiente
- Frontend local aponta para `https://localhost:7190/api` em `product-management-web/src/environments/environment.ts`.
- Em Docker, a API e publicada em `http://localhost:8080` e o frontend servido em `http://localhost:4200`.
- Para testar frontend local contra API em Docker, ajuste `apiBaseUrl` para `http://localhost:8080/api`.

## Banco SQLite e Migrations
- A API aplica migrations automaticamente no startup (`Database.MigrateAsync`) quando o provider e relacional.
- Em ambiente de teste com provider in-memory, a inicializacao usa `EnsureCreated`.

Comandos uteis para evolucao de schema:

```bash
dotnet ef migrations add NomeDaMigration --project ProductManagement.Infrastructure/ProductManagement.Infrastructure.csproj --startup-project ProductManagement.Api/ProductManagement.Api.csproj --output-dir Persistence/Migrations
dotnet ef database update --project ProductManagement.Infrastructure/ProductManagement.Infrastructure.csproj --startup-project ProductManagement.Api/ProductManagement.Api.csproj
dotnet ef migrations list --project ProductManagement.Infrastructure/ProductManagement.Infrastructure.csproj --startup-project ProductManagement.Api/ProductManagement.Api.csproj
```

## Endpoints Principais
- `GET /api/products`: lista produtos com filtros, ordenacao e paginacao
- `GET /api/products/{id}`: consulta por id
- `POST /api/products`: cria produto
- `PUT /api/products/{id}`: atualiza produto
- `DELETE /api/products/{id}/inactivate`: inativa produto
- `PATCH /api/products/{id}/activate`: ativa produto

Exemplo de consulta com filtros:

```http
GET /api/products?page=1&pageSize=10&name=notebook&category=eletronicos&status=Active&sortBy=createdAt&sortDirection=desc
```

## Testes
### Backend
```bash
dotnet test ProductManagement.slnx
```

### Frontend
```bash
cd product-management-web
npm test -- --runInBand
```

## Decisoes Tecnicas e Trade-offs
- Clean Architecture em camadas: melhora separacao de responsabilidades e testabilidade, com custo de maior quantidade de projetos/abstracoes.
- SQLite: simplifica setup e reproducao local, mas nao e a melhor opcao para cenarios de alta concorrencia.
- EF Core com migrations automaticas no startup: acelera bootstrap, com trade-off de exigir cuidado extra em ambientes produtivos com controle de rollout.
- Enum serializado como string na API: melhora legibilidade e interoperabilidade de contratos, com necessidade de validacao consistente entre backend e frontend.
- Filtros e paginacao server-side: reduz carga no frontend e melhora escalabilidade da listagem, com impacto de maior complexidade no repositorio/queries.
- Inativacao/ativacao explicita: preserva historico e evita exclusao fisica, exigindo clareza de UX para status visivel na grade.

## Troubleshooting
- Porta 4200 ocupada:
	- Execute `npm start -- --port 4201` ou finalize o processo que usa a porta.
- Frontend nao conecta na API:
	- Verifique se a API esta em execucao na URL esperada (`https://localhost:7190` local ou `http://localhost:8080` Docker).
	- Ajuste `apiBaseUrl` conforme o modo de execucao.
- Falha de build/test por arquivo DLL bloqueado no backend:
	- Encerre processo em execucao da API e rode novamente os testes.
- Erro de certificado HTTPS no navegador:
	- Confie no certificado local do .NET (`dotnet dev-certs https --trust`) e reinicie o navegador.

## O que Faria Diferente com Mais Tempo
- Adicionar testes end-to-end cobrindo fluxo completo (home, listagem, criar, editar, ativar/inativar).
- Evoluir observabilidade com metricas, tracing distribuido e dashboards.
- Implementar estrategia de cache para consultas de listagem com invalidacao por escrita.
- Refinar versionamento e documentacao da API (ex.: OpenAPI examples mais completos por caso de uso).
- Expandir politicas de resiliencia e hardening para ambiente de producao (rate limit, retries e health checks avancados).

