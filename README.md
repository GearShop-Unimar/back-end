# ⚙️ GearShop API (Backend)

Bem-vindo ao repositório do *backend* da GearShop, uma aplicação de comércio eletrónico especializada em peças automotivas. Esta API é construída em **ASP.NET Core** e é responsável pela lógica de negócio, autenticação e persistência de dados.

## 🛠️ Tecnologias Utilizadas

* **Linguagem:** C# 
* **Framework:** ASP.NET Core Web API
* **Banco de Dados:** MySQL
* **ORM:** Entity Framework Core (EF Core) com provedor Pomelo.EntityFrameworkCore.MySql
* **Autenticação:** JWT (JSON Web Tokens) Bearer
* **Documentação:** Swagger/OpenAPI
* **Estrutura:** Controladores e Padrão Repository/Service.

## 🚀 Como Rodar o Projeto

Siga os passos abaixo para configurar e iniciar o servidor da API.

### Pré-requisitos

Certifique-se de ter instalado:
1.  [.NET](https://dotnet.microsoft.com/download)
2.  Um servidor MySQL (ou Docker)
3.  Um editor de código (Visual Studio Code ou Visual Studio)

### 1. Configuração do Banco de Dados

1.  Crie um banco de dados vazio no seu servidor MySQL (ex: `gearshop`).
2.  Atualize a *string* de conexão no arquivo `appsettings.Development.json` (ou `appsettings.json`):

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Port=3306;Database=gearshop;Uid=seu_usuario;Pwd=sua_senha;"
    }
    ```

3.  Aplique as migrações do Entity Framework Core para criar o esquema do banco de dados:

    ```bash
    dotnet ef database update
    ```

### 2. Configuração do JWT

1.  Adicione a chave secreta JWT (usada para assinar e validar tokens) no arquivo de configuração (`appsettings.json` ou `appsettings.Development.json`):

    ```json
    "Jwt": {
      "Key": "SUA_CHAVE_SECRETA_MUITO_LONGA_E_SEGURA"
    }
    ```

    > **Importante:** A chave deve ser longa e mantida em segredo.

### 3. Execução

1.  Navegue até o diretório raiz do projeto (`/GearShop`).
2.  Rode a aplicação:

    ```bash
    dotnet run
    ```

O servidor estará disponível por padrão em `http://localhost:5282` (verifique as configurações no console).

## 🧭 Endpoints Principais

A documentação completa dos endpoints está disponível através do Swagger.

| Método | Endpoint | Descrição | Requer Autenticação |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Auth/login` | Autentica um usuário e retorna o token JWT. | Não |
| `POST` | `/api/users` | Registra um novo usuário/cliente. | Não |
| `POST` | `/api/product` | Cria um novo anúncio de produto (requer `FormData` para imagem). | Sim (Vendedor) |
| `GET` | `/api/product` | Retorna a lista de todos os produtos. | Não |
| `GET` | `/api/product/{id}` | Retorna detalhes de um produto específico. | Não |

## 📦 Estrutura do Projeto

* **Controllers/**: Lógica de recebimento de requisições HTTP e roteamento.
* **Data/**: `AppDbContext` e configuração do Entity Framework Core.
* **Dtos/**: Objetos de Transferência de Dados (DTOs) usados para entrada e saída.
* **Models/**: Modelos que representam as entidades do banco de dados (ex: `Product.cs`, `User.cs`).
* **Repositories/**: Camada de acesso direto ao banco de dados (EF Core).
* **Services/**: Camada de lógica de negócio e regras (implementa interfaces `IProductService`, etc.).
* **Middleware/**: Componentes de pipeline (ex: `GlobalExceptionMiddleware`).
* **wwwroot/**: Pasta de arquivos estáticos, incluindo uploads de imagens.

## ✅ Testes Automatizados

O repositório agora inclui o projeto `GearShop.Tests`, baseado em **xUnit**, **FluentAssertions** e **NSubstitute**, cobrindo cenários de serviço como autenticação, catálogo de produtos e carrinho.

### Estrutura

```
GearShop.Tests/
├── Services/
│   ├── Auth/AuthServiceTests.cs
│   ├── Cart/CartServiceTests.cs
│   ├── Premium/PremiumAccountServiceTests.cs
│   ├── Product/ProductServiceTests.cs
│   ├── Review/ReviewServiceTests.cs
│   └── User/UserServiceTests.cs
└── GearShop.Tests.csproj
```

### Executando os testes

```bash
dotnet test
# ou
dotnet test GearShop.Tests/GearShop.Tests.csproj
```

> **Nota:** durante a execução pode surgir o aviso `MSB3277` devido a dependências do Entity Framework com versões distintas (9.0.10 vs 9.0.0 trazida pelo provedor Pomelo). É apenas um *warning* conhecido e não impede a execução dos testes ou dos 22 cenários unitários atuais.