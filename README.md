# AVMLabs — Lab Client Management System (Technical Assessment)

Full Stack Developer assessment submission — .NET + SQL Server + ASP.NET Core MVC (Razor).

## Structure

```
AVM_Assignment/
├── Module1_SQL/
│   └── schema_and_queries.sql      # Schema (Module 1.1) + Queries 1-4 (Module 1.2)
├── AVMLabs.Api/                    # ASP.NET Core Web API (Module 2)
│   ├── Models/                     # EF Core entities
│   ├── Data/AppDbContext.cs        # DbContext + seed data (3 clients, 5 tests, 3 work orders)
│   ├── DTOs/                       # Request/response DTOs
│   ├── Controllers/                # ClientsController, WorkOrdersController, ReportsController, TestsController
│   ├── Program.cs
│   └── appsettings.json            # Connection string (not hardcoded in code)
├── AVMLabs.Mvc/                    # ASP.NET Core MVC front-end (Module 3)
│   ├── Controllers/                # ClientsController, WorkOrdersController
│   ├── Views/Clients/Index.cshtml       # View 1 - Client List Page
│   ├── Views/Clients/Details.cshtml     # View 2 - Client Details Page
│   ├── Views/WorkOrders/Create.cshtml   # View 3 - Work Order Entry Form
│   ├── Services/ApiClient.cs       # Talks to AVMLabs.Api over HTTP
│   └── Program.cs
└── AVMLabs.sln
```

## How to run locally

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB is fine — ships with Visual Studio) or a SQL Server instance

### 1. Run the SQL script (optional — for manual query testing)
Open `Module1_SQL/schema_and_queries.sql` in SSMS / Azure Data Studio against a fresh
database and execute. This creates the schema, seeds sample data, and runs Queries 1–4.

### 2. Run the Web API
```bash
cd AVMLabs.Api
dotnet restore
dotnet ef migrations add InitialCreate     # first time only
dotnet run
```
- The app auto-applies migrations on startup and seeds data via `HasData`.
- Swagger UI: `https://localhost:5001/swagger`
- Update the connection string in `appsettings.json` if not using LocalDB.

### 3. Run the MVC front-end
```bash
cd AVMLabs.Mvc
dotnet restore
dotnet run
```
- Confirm `ApiBaseUrl` in `AVMLabs.Mvc/appsettings.json` matches the API's URL.
- Browse to the MVC app's URL (e.g. `https://localhost:5003`) — it opens on the Client List page.

## Notes on scope / assumptions
- Kept queries to basic `SELECT` / `JOIN` / `GROUP BY` / `WHERE` per the brief — no CTEs or window functions.
- Controllers use DTOs throughout; EF entities are never returned directly.
- Client List search is a server-side postback (`GET /Clients?search=...`), as permitted by the brief.
- Work Order Entry Form uses plain JS for the dynamic rows/running total; submission posts
  form data to the MVC controller, which forwards a JSON payload to `POST /api/workorders`.
- A small `GET /api/tests` endpoint was added (not in the marked spec's 6 core routes) purely
  to populate the Test dropdown in View 3 — it's not separately scored but is needed for the UI to work.
- `/api/clients/{id}` doesn't include work orders inline, so the MVC `ClientsController.Details`
  action calls `/api/workorders` and filters client-side; a real production API would add a
  dedicated `/api/clients/{id}/workorders` endpoint instead.
