# AdventureWorks2022

**A hands-on .NET 10 project exploring Clean Architecture, CQRS, and service-to-service communication using Microsoft's AdventureWorks2022 SQL Server sample database.**

The goal is to build a collection of small, independently developed ASP.NET Core APIs while practicing how to structure application boundaries, persistence, HTTP contracts, and eventually asynchronous integration. The repository is organized as a **monorepo**, with a single Visual Studio solution.


## Tech stack

- **.NET 10 / ASP.NET Core Web API**
- **C#**
- **SQL Server** with the **AdventureWorks2022** sample database
- **Entity Framework Core** (SQL Server provider)
- **MediatR** for CQRS request/handler dispatch
- **Asp.Versioning.Mvc** for header-based API versioning
- **OpenAPI + Scalar** for interactive API documentation
- **xUnit** test project (test coverage is being developed)


## Database

All services will use the **same physical `AdventureWorks2022` SQL Server database** for this learning project. However, **each existing table has exactly one owning API**, and other APIs must communicate with that owner instead of querying or updating its tables directly.

> Shared database ownership is a deliberate learning constraint, **not full database isolation between microservices**. SQL Server foreign keys still apply across service boundaries.

## Getting started

### Prerequisites

1. **.NET 10 SDK** and a compatible development environment (for example, Visual Studio 2026).
2. **SQL Server** with the **AdventureWorks2022** sample database restored.
3. Access to that database from your local machine.

### Configure the connection string

In `/appsettings.json`, keep the placeholder without credentials:

```json
{
  "ConnectionStrings": {
    "AdventureWorks": ""
  }
}
```

For local development, store the actual connection string in **ASP.NET Core User Secrets**, associated with the `Api` projects. In Visual Studio, right-click **Api Project → Manage User Secrets** and enter:

```json
{
  "ConnectionStrings": {
    "AdventureWorks": "Server=YOUR_SERVER;Database=AdventureWorks2022;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Replace `YOUR_SERVER` and the authentication options for your environment. If using SQL Server authentication, keep the username and password in User Secrets, **not in tracked appsettings files**.

User Secrets are intended for local development and are not encrypted secrets storage for production.

### Run the API

Open `AdventureWorks2022.slnx` in Visual Studio, select **BusinessEntity.Api** as the startup project, and run it in the **Development** environment.

Alternatively, from the repository root:

```bash
dotnet restore AdventureWorks2022.slnx
dotnet run --project src/BusinessEntity/BusinessEntity.Api
```

The API exposes its OpenAPI document and Scalar interface in Development:

- **OpenAPI:** `/openapi/v1.json`
- **Scalar:** `/scalar`

Use the HTTPS address and port printed by ASP.NET Core when the application starts.

## API versioning

API versions are selected through the **`x-api-version` HTTP header**, not through the URL. The current version is **`1.0`**.

```http
GET /api/business-entities/1
x-api-version: 1.0
```

The default version is currently assumed when the header is omitted, according to the API's configured versioning settings.


## Error handling

Successful requests return resource data directly, without a custom response envelope. Errors follow **Problem Details** conventions, including appropriate HTTP status codes such as `404 Not Found`, `409 Conflict`, and `500 Internal Server Error`.

The global exception handler centralizes error translation and avoids exposing internal exception details in unexpected server errors. The application layer does not depend on HTTP response types.

## License and sample data

The application source code is licensed under the **MIT License**; see `LICENSE.txt`.

**AdventureWorks2022** is a Microsoft sample database and is subject to its own applicable terms. This repository's MIT license does **not** relicense Microsoft's database, backup files, or sample data.
