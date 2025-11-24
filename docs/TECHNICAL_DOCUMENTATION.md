**GearShop — Documentação Técnica (Básica)**

- **Projeto**: GearShop (API .NET)
- **Descrição**: Backend RESTful para uma loja (produtos, carrinho, pedidos, pagamentos, posts sociais, mensagens, contas premium e avaliações).
- **Autor / Repositório**: (ver repositório local)

**Tecnologias**
- **Runtime / SDK**: .NET 9 (conforme migrations/nomes)
- **ORM**: Entity Framework Core
- **Banco de Dados**: MySQL (configurado via `UseMySql` em `Program.cs`)
- **Autenticação**: JWT (configuração em `appsettings.json` + `Configuration/JwtOptions.cs`)
- **Documentação da API**: Swagger (OpenAPI)

**Pré-requisitos (desenvolvimento)**
- **.NET SDK**: 9.x instalado
- **MySQL**: servidor acessível com uma connection string configurada
- **Ferramentas opcionais**: `dotnet-ef` (para migrations), VS Code / Visual Studio

**Estrutura principal do repositório**
- **`Program.cs`**: bootstrap da aplicação, DI, autenticação, CORS, compressão, Swagger e configuração do `DbContext`.
- **`Data/AppDbContext.cs`**: modelos DbSet e configuração do modelo via Fluent API.
- **`Controllers/`**: controladores REST (ex.: `AuthController`, `ProductController`, `CartController`, `OrderController`, `UserController`, `PostController`, `MessageController`, `ReviewController`, `PaymentController`, `ImageController`, `PremiumAccountController`).
- **`Models/`**: entidades do domínio (User, Product, Order, Post, Message, etc.).
- **`Dtos/`**: objetos de transferência (requests/responses).
- **`Repositories/`**: repositórios EF (acesso a dados)
- **`Services/`**: regras de negócio e orquestração
- **`Configuration/`**: classes de configuração (ex.: `JwtOptions.cs`).
- **`Migrations/`**: migrations geradas pelo EF Core.
- **`wwwroot/`**: arquivos estáticos (imagens, etc.).
- **`Middleware/GlobalExceptionMiddleware.cs`**: tratamento global de exceções.

**Configuração (variáveis / appsettings)**
- **Connection Strings**: `ConnectionStrings:DefaultConnection` em `appsettings.json` / `appsettings.Development.json`. Exemplo (MySQL):

```powershell
# PowerShell (exemplo local)
$env:ConnectionStrings__DefaultConnection = "Server=localhost;Database=gearshop;User=root;Password=senha;"
$env:ASPNETCORE_ENVIRONMENT = 'Development'
```

- **JWT**: as chaves em `Jwt:Key`, `Jwt:Issuer` (definidas em `appsettings` ou em variáveis de ambiente). O projeto usa `builder.Configuration["Jwt:Key"]` para validar tokens.

**Como executar (desenvolvimento)**
- Build:

```powershell
dotnet build "GearShop.csproj"
```

- Run (modo normal):

```powershell
dotnet run --project "GearShop.csproj"
```

- Run com hot-reload/watch (recomendado para dev):

```powershell
dotnet watch run --project "GearShop.csproj"
```

- Publish (produção):

```powershell
dotnet publish "GearShop.csproj" -c Release -o ./publish
```

Observação: o workspace também contém tasks de VS Code para `build`, `publish` e `watch` que podem ser executadas via `Terminal -> Run Task`.

**Migrations / Banco de dados**
- Gerar migration (necessário `dotnet-ef`):

```powershell
# a partir da pasta do projeto
dotnet ef migrations add MyMigrationName --project .
```

- Aplicar migrations ao banco:

```powershell
dotnet ef database update --project .
```

**Autenticação / Segurança**
- A API usa JWT Bearer. O `Jwt:Key` deve ser mantido secreto em produção.
- Swagger está configurado para suportar autorização via header `Authorization: Bearer <token>`.

**Principais endpoints (visão rápida)**
- **Auth**: `/api/auth` — registrar, login, refresh token (ver `AuthController`).
- **Users**: `/api/users` — CRUD / perfil (ver `UserController`).
- **Products**: `/api/products` — listagem, detalhes, CRUD (ver `ProductController`).
- **Cart**: `/api/cart` — gerenciar itens do carrinho (ver `CartController`).
- **Orders / Payments**: `/api/orders`, `/api/payments` — checkout e pagamentos
- **Posts / Social**: `/api/posts`, comentários, likes (ver `PostController`).
- **Messaging**: `/api/messages` — conversas e mensagens (ver `MessageController`).
- **Reviews**: `/api/reviews` — avaliações de produtos (ver `ReviewController`).
- **Premium**: `/api/premium` — compra e gerência de contas premium (ver `PremiumAccountController`).

(Consulte cada controller para rotas e DTOs específicos.)

**Observações de arquitetura**
- Projeto organizado em camadas: Controllers → Services → Repositories → Data
- Uso do padrão Repository para abstrair acesso ao EF
- Configuração centralizada no `Program.cs` para DI e middlewares

**Logs / Tratamento de erros**
- Existe `GlobalExceptionMiddleware` para capturar exceções e formatar respostas JSON consistentes.
- Recomenda-se integrar um provider de logs (ex.: Application Insights, Serilog) para produção.

**Testes**
- Não há uma pasta `tests` no repositório atual. Recomenda-se adicionar testes unitários e/ou de integração (xUnit + test host) antes de produção.

**Deploy (sugestão básica)**
- Build e `dotnet publish` para gerar artefatos.
- Configurar `ConnectionStrings` e `Jwt` via variáveis de ambiente no ambiente de produção.
- Executar em container (Docker) ou em host .NET (IIS / Kestrel + proxy reverso).


