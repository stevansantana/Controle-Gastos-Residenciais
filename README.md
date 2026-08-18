# Controle de Gastos Residenciais

Sistema de controle de gastos residenciais, com cadastro de pessoas, cadastro
de transações (receitas/despesas) e consulta de totais.

Desenvolvido com:

- **Back-end:** .NET 8 (ASP.NET Core Web API) + Entity Framework Core + SQLite
- **Front-end:** React 18 + TypeScript (Vite)

## Arquitetura e decisões técnicas

- **Persistência:** o back-end usa SQLite (arquivo `gastos.db`, gerado
  automaticamente na primeira execução, dentro de `backend/ControleGastos.Api`).
  Por ser um arquivo em disco, os dados continuam existindo mesmo depois de
  fechar a API ou o navegador — não é um banco em memória.
- **Camadas do back-end:**
  - `Models`: entidades do banco (`Pessoa`, `Transacao`).
  - `Data`: `AppDbContext`, onde é configurado o relacionamento 1:N entre
    Pessoa e Transacao com **delete em cascata** (ao remover uma pessoa, o
    próprio banco apaga as transações dela).
  - `Dtos`: objetos de entrada/saída da API, para não expor as entidades do
    banco diretamente (evita, por exemplo, referências circulares no JSON).
  - `Services`: regras de negócio (validações, cálculo de totais), separadas
    dos Controllers.
  - `Controllers`: endpoints HTTP, apenas orquestram a chamada aos Services.
- **Regra do menor de idade:** centralizada em `TransacaoService.CriarAsync`,
  que impede a criação de uma transação do tipo `Receita` quando a pessoa
  associada tem menos de 18 anos (`Pessoa.EhMenorDeIdade`). O front-end também
  reforça essa regra na interface (desabilitando a opção "Receita"), mas a
  validação que realmente garante a regra é sempre a do back-end.
- **Identificadores:** `Guid` gerado automaticamente na criação de cada
  Pessoa/Transação, evitando dependência da ordem de inserção no banco.

## Estrutura de pastas

```
ControleGastos/
├── backend/
│   └── ControleGastos.Api/
│       ├── Controllers/     (endpoints HTTP)
│       ├── Services/        (regras de negócio)
│       ├── Models/          (entidades do banco)
│       ├── Dtos/            (contratos de entrada/saída da API)
│       ├── Data/            (DbContext / EF Core)
│       └── Program.cs       (configuração da aplicação)
└── frontend/
    └── controle-gastos-frontend/
        └── src/
            ├── components/  (telas: Pessoas, Transações, Totais)
            ├── services/    (chamadas à API)
            └── types/       (tipos TypeScript espelhando os DTOs)
```

## Como rodar o projeto localmente

Pré-requisitos: [.NET 8 SDK](https://dotnet.microsoft.com/download) e
[Node.js 18+](https://nodejs.org/).

### 1. Back-end

```bash
cd backend/ControleGastos.Api
dotnet restore
dotnet run
```

A API sobe em `http://localhost:5197` (a porta está fixada em
`Properties/launchSettings.json`). Ao rodar pela primeira vez, o arquivo
`gastos.db` é criado automaticamente. Você pode conferir os endpoints
interativamente em `http://localhost:5197/swagger`.

### 2. Front-end

Em outro terminal:

```bash
cd frontend/controle-gastos-frontend
npm install
npm run dev
```

A aplicação abre em `http://localhost:5173`.

> Importante: o back-end precisa estar rodando para o front-end funcionar,
> pois todos os dados vêm da API.

## Endpoints da API

| Método | Rota                | Descrição                                                            |
| ------ | ------------------- | -------------------------------------------------------------------- |
| GET    | `/api/pessoas`      | Lista todas as pessoas                                               |
| POST   | `/api/pessoas`      | Cria uma pessoa (`{ nome, idade }`)                                  |
| DELETE | `/api/pessoas/{id}` | Remove uma pessoa e todas as suas transações                         |
| GET    | `/api/transacoes`   | Lista todas as transações                                            |
| POST   | `/api/transacoes`   | Cria uma transação (`{ descricao, valor, tipo, pessoaId }`)          |
| GET    | `/api/totais`       | Retorna totais de receitas/despesas/saldo por pessoa e o total geral |

## Possíveis melhorias futuras

- Edição de pessoas e transações.
- Filtros na consulta de totais (por período, por tipo).
- Autenticação/autorização.
- Testes automatizados (xUnit no back-end, Vitest no front-end).
