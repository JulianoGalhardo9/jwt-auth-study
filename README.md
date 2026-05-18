# 🔐 Identity Microservice

> Microsserviço de identidade e autenticação em **.NET 9**, construído com **Clean Architecture**, **DDD** e segurança de nível profissional — incluindo Refresh Token com Rotação completa.

---

## 📐 Arquitetura

O projeto segue a regra de dependência estrita da Clean Architecture: camadas externas conhecem as internas, mas o **Domínio nunca depende de ninguém**.

```
┌────────────────────────────────────────────────┐
│                      API                       │  Controllers · Middlewares · Program.cs
├────────────────────────────────────────────────┤
│                  Application                   │  UseCases · Commands · Handlers · DTOs
├────────────────────────────────────────────────┤
│                    Domain                      │  Entities · Interfaces · Result Pattern
├────────────────────────────────────────────────┤
│               Infrastructure                   │  EF Core · BCrypt · JWT · Repositories
└────────────────────────────────────────────────┘
```

| Camada | Responsabilidade |
|---|---|
| **Domain** | Entidades de negócio (`User`), erros de domínio (`DomainErrors`) e contratos de serviços/repositórios. 100% isolado de frameworks. |
| **Application** | Casos de Uso via padrão Command/Handler. DTOs de entrada/saída e validações com FluentValidation. |
| **Infrastructure** | Implementações concretas: persistência (EF Core InMemory), hash de senhas (BCrypt) e emissão de tokens JWT. |
| **API** | Porta de entrada HTTP. Pipeline, middlewares de autenticação e exposição dos endpoints. |

---

## ✨ Funcionalidades e Padrões

### Result Pattern
Substituição completa de `Exceptions` para controle de fluxo de negócio. O sistema responde com objetos tipados de sucesso ou falha, aumentando performance e previsibilidade do código.

### Modelos de Domínio Ricos
A entidade `User` protege sua própria integridade com construtores e modificadores privados, expondo apenas métodos de negócio (`Create`, `UpdateRefreshToken`).

### Refresh Token Rotation (Fluxo Profundo)
Cada Refresh Token é válido para **uso único**. Ao renovar o Access Token, um novo par de tokens é gerado e o token anterior é completamente invalidado no banco de dados — mitigando riscos de roubo de sessão.

### Outras Práticas
- **FluentValidation** — validação desacoplada de payloads antes de chegarem à camada de negócio
- **BCrypt** — hash adaptativo para senhas de usuários
- **JWT Estrito** — proteção de endpoints via `[Authorize]`

---

## 🛠️ Stack Tecnológica

| Tecnologia | Uso |
|---|---|
| .NET 9 + ASP.NET Core Web API | Framework principal |
| EF Core InMemory | Persistência em memória |
| Microsoft.AspNetCore.Authentication.JwtBearer | Autenticação JWT |
| FluentValidation.DependencyInjectionExtensions | Validação de entrada |
| BCrypt.Net-Next | Hash de senhas |

---

## 🚀 Como Executar

### Pré-requisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) instalado
- Um cliente HTTP — Postman, Insomnia, Bruno ou REST Client (VS Code)

### Passos

```bash
# 1. Verifique se a solution compila sem erros
dotnet build

# 2. Navegue até o projeto da API
cd IdentityMicroservice.API

# 3. Inicie o microsserviço
dotnet run
```

A aplicação estará disponível nas URLs exibidas no console (tipicamente `http://localhost:5000`).

---

## 📬 Endpoints da API

### `POST /api/auth/register` — Registro de Usuário

```json
{
  "name": "Juliano Galhardo",
  "email": "juliano@email.com",
  "password": "senhaSuperSegura123"
}
```

---

### `POST /api/auth/login` — Login

```json
{
  "email": "juliano@email.com",
  "password": "senhaSuperSegura123"
}
```

**Resposta `200 OK`:**
```json
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "d4f8a2c1..."
}
```

> O `accessToken` tem curta duração. O `refreshToken` tem longa duração e é usado para renovar a sessão.

---

### `POST /api/auth/refresh` — Renovação de Sessão

```json
{
  "refreshToken": "STRING_DO_REFRESH_TOKEN_RECEBIDO_NO_LOGIN"
}
```

**Resposta `200 OK`:** Retorna um novo par de tokens. O token enviado é imediatamente invalidado.

---

### `GET /api/users/me` — Rota Protegida *(requer autenticação)*

```http
Authorization: Bearer <SEU_ACCESS_TOKEN>
```

---

## 🔄 Fluxo de Autenticação

```
┌─────────┐         ┌─────────────────┐         ┌──────────┐
│ Cliente │         │  Auth Endpoints │         │    DB    │
└────┬────┘         └────────┬────────┘         └────┬─────┘
     │   POST /login         │                       │
     │──────────────────────▶│                       │
     │                       │  Valida credenciais   │
     │                       │──────────────────────▶│
     │   accessToken         │                       │
     │   + refreshToken      │   Salva refreshToken  │
     │◀──────────────────────│──────────────────────▶│
     │                       │                       │
     │   POST /refresh        │                       │
     │──────────────────────▶│                       │
     │                       │  Valida + invalida    │
     │                       │  token anterior       │
     │                       │──────────────────────▶│
     │   Novo par de tokens  │                       │
     │◀──────────────────────│                       │
```

---

## 📁 Estrutura de Pastas

```
IdentityMicroservice/
├── IdentityMicroservice.API/
│   ├── Controllers/
│   ├── Middlewares/
│   └── Program.cs
├── IdentityMicroservice.Application/
│   ├── UseCases/
│   ├── Commands/
│   ├── Handlers/
│   └── DTOs/
├── IdentityMicroservice.Domain/
│   ├── Entities/
│   ├── Errors/
│   └── Interfaces/
└── IdentityMicroservice.Infrastructure/
    ├── Persistence/
    ├── Cryptography/
    └── Repositories/
```

---

## 📄 Licença

Distribuído sob a licença MIT. Consulte o arquivo `LICENSE` para mais detalhes.
