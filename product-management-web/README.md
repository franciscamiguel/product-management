# Product Management Web

Frontend Angular do sistema de gestao de produtos.

## Stack
- Angular 17 (Standalone Components)
- Angular Material
- Reactive Forms + RxJS
- Jest + Testing Library

## Pre-requisitos
- Node.js 20+
- npm 10+

## Como Rodar
No diretorio `product-management-web`:

```bash
npm install
npm start
```

Aplicacao em:
- http://localhost:4200

## Build
```bash
npm run build
```

## Testes
```bash
npm test -- --runInBand
```

## Scripts Disponiveis
- `npm start`: sobe o servidor de desenvolvimento
- `npm run build`: gera build de producao
- `npm test -- --runInBand`: executa testes unitarios com Jest
- `npm run test:watch`: executa testes em watch mode
- `npm run lint`: validacao rapida de build em configuracao de desenvolvimento

## Configuracao da API
- Ambiente local: `src/environments/environment.ts`
- Valor padrao atual: `https://localhost:7190/api`

Se a API estiver no Docker (`http://localhost:8080`), altere temporariamente para:

```ts
apiBaseUrl: 'http://localhost:8080/api'
```

## Estrutura de Features
- `src/app/features/home`: pagina inicial
- `src/app/features/catalog`: listagem, filtros e acoes de CRUD/status
- `src/app/features/catalog/product-dialog`: modal de criacao/edicao
- `src/app/core/services`: servicos HTTP para integracao com API
- `src/app/core/models`: contratos de dados

## Comportamento Funcional Importante
- A listagem usa paginacao e filtros server-side.
- Os filtros sao aplicados ao clicar em Pesquisar.
- Limpar filtros restaura criterios e recarrega a listagem.
- Acoes de status alternam entre Ativar e Inativar conforme estado do item.
