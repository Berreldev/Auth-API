# 🔐 AuthAPI – Sistema de Autenticação Criptografada

- 👨‍💻 **Desenvolvedor:** *Gabriel Almeida*  
- 🗓 **Período de desenvolvimento / última atualização:** 07/2025
  
---

## 1 – Sobre o projeto

- API REST criada em **ASP.NET Core 8** para fornecer cadastro, login e verificação de sessão.  
- **Arquitetura limpa** (Controllers → Services → Data) com **Injeção de Dependência** nativa.  
- **Segurança**: senha protegida com **BCrypt** + autenticação via **JWT** com claims de usuário.  
- **Persistência** em **PostgreSQL** usando **Entity Framework Core** e migrações versionadas.  
- Documentação automática em **Swagger/OpenAPI**.  
- Testes de integração automatizados em **Postman**.  
- Conteinerização pronta com **Docker + Docker Compose**.
  
- ---

## 2 – Stack utilizada

| Camada           | Tecnologia | Observações                                   |
|------------------|------------|----------------------------------------------|
| Linguagem        | C# 12 / .NET 8 | Projeto Web API template                    |
| ORM / Banco      | Entity Framework Core 8<br>PostgreSQL 16 | Migrações (`dotnet ef`)                     |
| Segurança        | BCrypt.Net-Next 4 | Hash de senha                              |
| Autenticação     | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`) | Expiração configurável                      |
| Docs & Testes    | Swashbuckle (Swagger)<br>Postman | Coleção automatizada                        |
| DevOps           | Docker file + docker-compose | Sobe API + banco com um comando             |

---
## 3 – Passos para clonar, configurar e executar a API 📜

| | Comando |
|---|---|
| **1)** Clone o repositório | ```bash<br>git clone -b main https://github.com/Berreldev/Auth-API<br>cd AuthAPI``` |
| **2)** Crie o banco PostgreSQL local (opcional se usar Docker) | ```bash<br>psql -U postgres -c "CREATE DATABASE authdb;"``` |
| **3)** Aplique as migrações | ```bash<br>dotnet ef database update``` |
| **4)** Rode o projeto | ```bash<br>dotnet run``` |
| **➡  Alternativa com Docker Compose** | ```bash<br>docker compose up --build``` |

> A API será exposta (por padrão) em **https://localhost:7242** e **http://localhost:5242**.
> 
---


## 4 – Documentação (Swagger) 📚

Depois que a aplicação estiver rodando, acesse:
https://localhost:7242/swagger


Na interface Swagger UI é possível testar **/register**, **/login** e **/check** sem precisar de Postman.

---

## 5 – Endpoints principais ⚡

| Método | Rota                        | Descrição                               |
|--------|-----------------------------|-----------------------------------------|
| POST   | `/api/auth/register`        | Cria novo usuário (FullName, Email, Password) |
| POST   | `/api/auth/login`           | Autentica usuário e devolve `authToken` |
| POST   | `/api/auth/check`           | Valida `authToken` e retorna `isLogged` |

---

## 6 – Rodando os testes no Postman 🧪

1. Importe o arquivo **AuthAPI.postman_collection.json** (na pasta _tests_)  
2. Clique em **Run Collection**.  
3. O fluxo **Register → Login → Check** será executado automaticamente e deverá ficar verde ✅.

---

## 7 – Migrações e versionamento de banco 🗄

- Criar nova migração:  
  ```bash
  dotnet ef migrations add AddNewField

  Aplicar no banco:
  dotnet ef database update
