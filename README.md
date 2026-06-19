# CRN Assessment - RESTful API

This is a complete .NET 8 RESTful Web API built using Clean Architecture principles, Entity Framework Core, and SQL Server.

## Features & Architecture
- **Clean Architecture:** Divided into `Domain`, `Application`, `Infrastructure`, and `API` layers to ensure strict separation of concerns.
- **Authentication & Authorization:** Secure JWT Bearer authentication with Role-based access control.
- **Data Access:** Entity Framework Core with Repository Pattern and AutoMapper for DTO mapping.
- **Performance:** Implemented AsNoTracking, database pagination (`Skip`/`Take`), Response Compression, and SQL indexing (`ProductName`).
- **Structured Logging:** Serilog integration writing to the Console and local file system.
- **Exception Handling:** Global Exception Middleware providing consistent, standard JSON error responses without leaking stack traces.
- **Testing:** `xUnit` and `Moq` utilized for isolated unit testing of services and controllers.

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (or LocalDB / Docker)
- Docker Desktop (Optional, for containerized deployment)

## Getting Started

### 1. Database Setup
The application uses Entity Framework Core Code-First migrations. By default, it connects to `localhost\SQLEXPRESS`.
To create the database and tables, run:
```bash
dotnet ef database update --project src/Infrastructure --startup-project src/API
```

### 2. Running Locally
Run the API from the root directory:
```bash
dotnet run --project src/API
```
The Swagger UI will be available at `https://localhost:<port>/swagger/index.html`.

### 3. Running with Docker Compose
To spin up both the API and a SQL Server 2022 instance in isolated containers:
```bash
docker-compose up -d --build
```

### 4. Running Tests
To execute the unit tests, simply run:
```bash
dotnet test
```

## JWT Authentication
To interact with secured endpoints, you must obtain a JWT token. 
1. Use the `/api/Auth/login` endpoint with valid credentials.
2. Click the "Authorize" button at the top of Swagger and paste the token directly.

---

## 📝 Reviewer Notes & Testing Guide

To make assessing this project as seamless as possible, please note the following architectural decisions and shortcuts provided for testing:

### 1. Instant Docker Testing (Auto-Seeding)
If you run `docker-compose up --build`, the API will automatically run Entity Framework migrations and **seed the database** with initial data. 
You can immediately test the secured endpoints using the seeded Admin credentials:
- **Username:** `admin`
- **Password:** `Admin@123`

### 2. Pagination is Mandatory (Industry Standard)
The `GET /api/Product/GetAllProducts` endpoint utilizes a `PaginationQuery` (`PageNumber` and `PageSize`). 
*Why?* In a production environment, returning an unconstrained list of database records can cause severe memory and network bottlenecking. Pagination is enforced to ensure scalability. You do not need to manually enter these values in Swagger; the defaults (`PageNumber: 1`, `PageSize: 10`) will apply automatically!

### 3. PUT Requests Require Matching IDs
When updating a product via `PUT /api/Product/UpdateProduct/{id}`, the ID in the route path **must match** the `id` field inside the JSON body. 
*Why?* This is a strict RESTful convention. It prevents accidental data corruption caused by a mismatched route and payload (e.g., inadvertently trying to update Product #5 with Product #1's data). 

### 4. Local Execution (Without Docker)
If you prefer to run the API via Visual Studio instead of Docker, please ensure you update the `ConnectionStrings:DefaultConnection` in `appsettings.json` to match your local SQL Server instance (e.g., changing `localhost\SQLEXPRESS` to `(localdb)\mssqllocaldb` if using LocalDB).
