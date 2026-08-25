# Sarhne

<p align="center">
  <strong>Anonymous Messaging & Social Interaction Backend</strong>
</p>

<p align="center">
  A production-oriented REST API built with ASP.NET Core, Clean Architecture principles, Entity Framework Core, SQL Server, SignalR, Hangfire, caching, and JWT authentication.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 10">
  <img src="https://img.shields.io/badge/ASP.NET%20Core-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="ASP.NET Core">
  <img src="https://img.shields.io/badge/Entity%20Framework%20Core-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="EF Core">
  <img src="https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server">
  <img src="https://img.shields.io/badge/SignalR-Realtime-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="SignalR">
  <img src="https://img.shields.io/badge/Hangfire-Background%20Jobs-00A98F?style=for-the-badge" alt="Hangfire">
</p>

---

## 📖 Overview

**Sarhne** is a backend API for an anonymous messaging and social interaction platform.

The platform allows users to:

- Receive anonymous or identified messages.
- Control whether received messages are publicly visible.
- Like publicly visible messages.
- Follow other users.
- Receive real-time notifications.
- Receive real-time message updates through SignalR.
- Manage their profile and privacy settings.
- Authenticate using JWT access tokens and refresh tokens.
- Upload and manage message/profile images.
- Administrators to manage users and send system notifications.
- SuperAdmins to manage roles and perform administrative operations.

The project was designed with a strong focus on **separation of concerns, maintainability, security, performance, and extensibility**.

---

## ✨ Features

### 🔐 Authentication & Authorization

- User registration and login.
- JWT access tokens.
- Refresh token mechanism.
- Token revocation.
- Secure HTTP-only cookies where required.
- ASP.NET Core Identity.
- Role-based authorization.
- `User`, `Admin`, and `SuperAdmin` roles.
- Admin policies.
- SuperAdmin policies.
- Current-user abstraction.

### 💬 Messaging

- Send messages to users.
- Anonymous messaging.
- Identified messaging.
- Message content.
- Message images.
- Edit messages.
- Remove message content.
- Remove message images.
- Hide/unhide received messages.
- Public messages endpoint.
- Pagination.
- Message likes.
- Real-time message events through SignalR.

### 👤 User Features

- User profile management.
- Profile image support.
- User settings.
- Privacy controls.
- Notification preferences.
- Follow/unfollow functionality.
- Public profile messages.

### 🔔 Notifications

Supports multiple notification types, including:

- Follow notifications.
- New message notifications.
- System notifications.

Notifications are:

- Stored in SQL Server.
- Filtered according to user notification preferences.
- Delivered in real time using SignalR.

Administrators can send:

- Notifications to a specific user.
- Notifications to all eligible users.

### 🛡️ Admin Features

Admins can:

- Retrieve users.
- Delete users.
- Send notifications to users.
- Send system notifications to all eligible users.

### 👑 SuperAdmin Features

SuperAdmins have all Admin capabilities plus:

- Add roles to users.
- Remove roles from users.
- Retrieve administrators.
- Manage administrative roles.

The authorization model ensures that:

> **SuperAdmin can perform Admin operations, but Admin cannot perform SuperAdmin operations.**

### ⚡ Real-Time Communication

SignalR is used for real-time events.

Current hubs include:

```text
/hubs/notifications
/hubs/messages
```

Examples of client events:

```text
NotificationReceived
NotificationRead
AllNotificationsRead
MessageReceived
```

### 🧹 Background Jobs

Hangfire is used for recurring background tasks.

Current cleanup functionality includes removing:

- Old notifications.
- Expired refresh tokens.

### 🚀 Caching

Application-level caching is implemented using `IMemoryCache`.

Caching is used for frequently accessed data where appropriate, including:

- User settings.
- Public messages.

Cache invalidation is performed whenever the underlying data changes.

### 🧪 Application Behaviors

MediatR pipeline behaviors are used for cross-cutting concerns such as:

- Validation.
- Logging.
- Performance monitoring.

### 🩺 Health Checks

Health checks are included to monitor application availability and dependencies.

### 🌐 CORS

CORS is configured for the frontend applications that communicate with the API.

### 🚦 Rate Limiting

Rate limiting is used to protect API endpoints from excessive requests and abuse.

---

# 🏗️ Architecture

The project follows a **Clean Architecture / N-Tier inspired architecture** with clear separation between business logic, application logic, infrastructure, and the API.

```text
┌─────────────────────────────┐
│          Sarhne.API         │
│ Controllers / Hubs /        │
│ Middleware / Authorization  │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│     Sarhne.Application      │
│ Features / CQRS / Behaviors │
│ Contracts / DTOs / Errors   │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│       Sarhne.Domain         │
│ Entities / Enums / Constants│
│ Business Rules              │
└─────────────────────────────┘
               ▲
               │
┌──────────────┴──────────────┐
│     Sarhne.Infrastructure   │
│ EF Core / Identity /        │
│ Persistence / Services /    │
│ Email / Storage / Caching   │
└─────────────────────────────┘
```

### Projects

| Project | Responsibility |
|---|---|
| `Sarhne.API` | HTTP API, Controllers, SignalR Hubs, Middleware, Authorization |
| `Sarhne.Application` | Business use cases, CQRS, handlers, DTOs, contracts, behaviors |
| `Sarhne.Domain` | Core entities, enums, constants, and domain rules |
| `Sarhne.Infrastructure` | Database, EF Core, Identity, external services, caching, email, storage, background jobs |

---

# 🛠️ Tech Stack

| Technology | Purpose |
|---|---|
| **.NET 10** | Application runtime |
| **ASP.NET Core 10** | REST API |
| **Entity Framework Core 10** | ORM / data access |
| **SQL Server** | Relational database |
| **ASP.NET Core Identity** | User and role management |
| **JWT Bearer Authentication** | Authentication |
| **MediatR** | CQRS and request handling |
| **FluentValidation** | Request validation |
| **Mapster** | Object mapping |
| **SignalR** | Real-time communication |
| **Hangfire** | Background and recurring jobs |
| **Serilog** | Structured logging |
| **IMemoryCache** | Application caching |
| **Swagger / OpenAPI** | API documentation |
| **Health Checks** | Application monitoring |

---

# 📂 Project Structure

```text
Sarhne/
│
├── Sarhne.API/
│   ├── Controllers/
│   ├── Extensions/
│   ├── Hubs/
│   ├── Middlewares/
│   ├── Authorization/
│   ├── Properties/
│   ├── Program.cs
│   ├── DependencyInjection.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── Sarhne.Application/
│   ├── Behaviors/
│   ├── Common/
│   ├── Contracts/
│   ├── Features/
│   │   ├── Authentication/
│   │   ├── User/
│   │   ├── Admin/
│   │   └── SuperAdmin/
│   └── ...
│
├── Sarhne.Domain/
│   ├── Constants/
│   ├── Entities/
│   ├── Enums/
│   └── ...
│
├── Sarhne.Infrastructure/
│   ├── Identity/
│   ├── Persistence/
│   ├── Services/
│   │   ├── Authentication/
│   │   ├── BackgroundJobs/
│   │   ├── Caching/
│   │   ├── Email/
│   │   ├── Notifications/
│   │   └── Storage/
│   └── Settings/
│
├── .gitignore
├── README.md
└── Sarhne.sln
```

---

# 🔑 Authentication

Authentication uses:

```text
JWT Access Token
        +
Refresh Token
```

Access tokens are short-lived, while refresh tokens are stored and managed server-side.

Refresh tokens support:

- Expiration.
- Revocation.
- Replacement.
- IP tracking.
- Revocation reason.

---

# 👥 Roles & Authorization

The application currently defines:

```csharp
public static class Roles
{
    public const string Admin = nameof(Admin);
    public const string User = nameof(User);
    public const string SuperAdmin = nameof(SuperAdmin);
}
```

Authorization policies include:

```text
Admin
SuperAdmin
```

The Admin policy allows:

```text
Admin
SuperAdmin
```

while the SuperAdmin policy allows only:

```text
SuperAdmin
```

This creates the hierarchy:

```text
SuperAdmin
    │
    └── can perform Admin operations

Admin
    │
    └── cannot perform SuperAdmin operations
```

---

# 📡 SignalR

SignalR provides real-time communication between the API and frontend.

### Notification Hub

```text
/hubs/notifications
```

Used for events such as:

```text
NotificationReceived
NotificationRead
AllNotificationsRead
```

### Message Hub

```text
/hubs/messages
```

Used for real-time message events.

Authentication for SignalR uses the JWT access token supplied through the connection.

---

# 🗃️ Database

The application uses:

```text
SQL Server
     ↓
Entity Framework Core
     ↓
SarhneDbContext
```

The project also uses EF Core interceptors for cross-cutting persistence concerns such as auditing and soft-delete behavior.

---

# 🔔 Notification Flow

A typical notification flow looks like:

```text
Application Feature
       │
       ▼
NotificationService
       │
       ├── Check user notification settings
       │
       ▼
Create Notification Entity
       │
       ▼
SQL Server
       │
       ▼
NotificationRealtimeService
       │
       ▼
SignalR
       │
       ▼
Frontend
```

This provides both:

- Persistent notification history.
- Real-time notification delivery.

---

# ⚡ Caching Strategy

Caching is implemented through an abstraction:

```csharp
ICacheService
```

rather than directly coupling application features to `IMemoryCache`.

This allows the caching implementation to be changed later without changing business logic.

Examples of cached data include:

```text
User Settings
Public Messages
```

Cache invalidation is performed when the underlying data changes to avoid serving stale data.

---

# 🧹 Background Cleanup

Hangfire manages recurring cleanup jobs.

The cleanup process removes data that no longer needs to remain in the database, including:

```text
Notifications older than the configured retention period
Expired refresh tokens
```

This keeps the database cleaner and prevents unnecessary growth.

---

# 🧩 CQRS & MediatR

Application features follow a request/handler structure.

Example:

```text
Feature
│
├── Command / Query
├── Handler
├── Validator
└── DTO / Response
```

This keeps each use case isolated and makes the application easier to maintain and test.

---

# 🔍 Validation

FluentValidation is integrated into the MediatR pipeline.

The general request flow is:

```text
HTTP Request
     │
     ▼
Controller
     │
     ▼
MediatR
     │
     ▼
ValidationBehavior
     │
     ▼
Handler
```

Invalid requests are rejected before reaching the business logic.

---

# 📊 Logging & Performance

Cross-cutting logging and performance monitoring are implemented through MediatR behaviors.

This allows the application to monitor:

- Executed requests.
- Execution duration.
- Potentially slow application operations.
- Application errors.

Serilog is used for structured application logging.

---

# 🩺 Health Checks

Health checks provide a simple way to verify application availability and infrastructure dependencies.

They are intended to be used by:

- Monitoring systems.
- Deployment environments.
- Container orchestration.
- Load balancers.

---

# 🌐 CORS

The API supports configurable frontend origins.

Example:

```json
"Cors": {
  "AllowedOrigins": [
    "https://localhost:3000",
    "http://localhost:5500"
  ]
}
```

Production origins should be configured according to the deployed frontend.

---

# 🚦 Rate Limiting

Rate limiting is used to reduce abuse and protect sensitive endpoints from excessive requests.

This is particularly important for endpoints related to:

- Authentication.
- Messaging.
- Public APIs.

---

# ⚙️ Configuration & Secrets

The project separates **application configuration** from **sensitive credentials**.

## `appsettings.json`

The repository contains only non-sensitive configuration and safe defaults.

Examples include:

- Logging configuration.
- JWT issuer and audience.
- Token expiration settings.
- Cookie configuration.
- CORS configuration.
- Application URLs.
- Email server settings that are not credentials.

Sensitive values are intentionally left empty or omitted.

---

## 🔐 ASP.NET Core User Secrets

Sensitive development values are stored using **ASP.NET Core User Secrets**.

User Secrets are associated with the `Sarhne.API` project and are stored outside the project directory.

Typical secrets include:

```text
ConnectionStrings:DefaultConnection
SeedAdmin:Email
SeedAdmin:Password
Jwt:SecretKey
Email:SenderEmail
Email:Password
```

Initialize User Secrets if necessary:

```bash
dotnet user-secrets init --project Sarhne.API
```

Then configure the required values:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING" --project Sarhne.API

dotnet user-secrets set "SeedAdmin:Email" "YOUR_ADMIN_EMAIL" --project Sarhne.API
dotnet user-secrets set "SeedAdmin:Password" "YOUR_ADMIN_PASSWORD" --project Sarhne.API
dotnet user-secrets set "SeedAdmin:FullName" "System Admin" --project Sarhne.API
dotnet user-secrets set "SeedAdmin:UserName" "Admin" --project Sarhne.API
dotnet user-secrets set "SeedAdmin:Gender" "Male" --project Sarhne.API

dotnet user-secrets set "Jwt:SecretKey" "YOUR_JWT_SECRET" --project Sarhne.API

dotnet user-secrets set "Email:SenderEmail" "YOUR_EMAIL" --project Sarhne.API
dotnet user-secrets set "Email:Password" "YOUR_EMAIL_PASSWORD" --project Sarhne.API
```

You can verify configured secrets with:

```bash
dotnet user-secrets list --project Sarhne.API
```

> **Never commit User Secrets or real credentials to source control.**

For production environments, use environment variables or a dedicated secret-management solution instead of User Secrets.

---

# 🚀 Getting Started

## Prerequisites

Make sure you have:

- .NET 10 SDK
- SQL Server
- Git
- An IDE such as Visual Studio or JetBrains Rider

---

## 1. Clone the repository

```bash
git clone https://github.com/mo-musa/Sarhne.git
cd Sarhne
```

---

## 2. Configure User Secrets

Initialize User Secrets:

```bash
dotnet user-secrets init --project Sarhne.API
```

Configure the required sensitive values as described in the **Configuration & Secrets** section.

---

## 3. Configure the database

Make sure your SQL Server instance is running and configure:

```text
ConnectionStrings:DefaultConnection
```

through User Secrets.

Then apply the migrations:

```bash
dotnet ef database update --project Sarhne.Infrastructure --startup-project Sarhne.API
```

---

## 4. Run the API

From the solution root:

```bash
dotnet run --project Sarhne.API
```

---

# 📚 API Documentation

When running in Development, Swagger is available through the configured Swagger UI endpoint.

Swagger provides interactive documentation for:

- Authentication.
- User features.
- Messaging.
- Notifications.
- Admin operations.
- SuperAdmin operations.

---

# 🔄 Development Workflow

A typical feature follows this structure:

```text
1. Define requirement
        ↓
2. Create Command / Query
        ↓
3. Create Validator
        ↓
4. Create Handler
        ↓
5. Create DTO / Response
        ↓
6. Add Controller endpoint
        ↓
7. Add required infrastructure/service logic
        ↓
8. Add authorization if required
        ↓
9. Test through Swagger / frontend
        ↓
10. Add caching / invalidation when appropriate
```

---

# 🧠 Design Principles

The project follows several important principles:

### Separation of Concerns

Each layer has a clear responsibility.

### Dependency Inversion

Application logic depends on abstractions rather than infrastructure implementations.

### CQRS

Commands and queries are separated to keep use cases focused.

### Thin Controllers

Controllers are responsible mainly for:

- Receiving HTTP requests.
- Sending requests through MediatR.
- Returning standardized results.

### Centralized Error Handling

Exceptions are handled through a global exception handler.

### Standardized Results

Application operations use a consistent result model instead of scattering error-handling logic across controllers.

### Reusable Services

Cross-cutting functionality such as:

- Notifications.
- Caching.
- Email.
- Authentication.
- Storage.

is abstracted behind interfaces.

---

# 🔐 Security Notes

Never commit:

```text
JWT secrets
Database passwords
Email passwords
API keys
Refresh tokens
Production credentials
User Secrets
```

Use:

- **ASP.NET Core User Secrets** for local development.
- **Environment variables or a dedicated secret manager** in production.

If a secret is accidentally committed, **rotate the secret immediately**.

Removing a secret from the latest commit is not sufficient if it has already existed in Git history.

---

# 🗺️ Future Improvements

Possible future improvements include:

- Automated unit and integration tests.
- Distributed caching such as Redis when horizontal scaling is required.
- Centralized observability.
- CI/CD pipeline.
- Docker containerization.
- Production deployment configuration.
- More granular rate-limiting policies.
- Advanced notification delivery strategies.
- Automated API integration testing.

---

# 🤝 Contributing

Contributions are welcome.

Before submitting changes:

1. Keep the existing architecture and conventions.
2. Follow the established feature structure.
3. Avoid placing business logic inside controllers.
4. Add validation for user input.
5. Consider authorization requirements.
6. Consider caching and cache invalidation when modifying frequently accessed data.
7. Never commit secrets or credentials.

---

# 📄 License

This project is currently intended for educational and development purposes.

Add an appropriate open-source license here if the project is later released under one.

---

<p align="center">
  Built with ASP.NET Core and ❤️
</p>
