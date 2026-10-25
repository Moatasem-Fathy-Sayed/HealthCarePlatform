# 🏥 Healthcare Platform (Enterprise Distributed Microservices System)

An enterprise-grade, production-ready healthcare management and telemetry platform built with modern **.NET 10** and **Clean Architecture / Domain-Driven Design (DDD)** principles. The system follows a distributed microservices architecture designed for high scalability, fault tolerance, and secure real-time operations.

---

## 🚀 Key Features & Implemented Modules

### 1. Patient & Appointment Management (Core Microservice)
* **Domain-Driven Design (DDD):** Built with strict domain invariants, encapsulated entities (`Appointment`), and robust business logic (e.g., status validation, preventing invalid state transitions).
* **CQRS Pattern:** Complete decoupling of write operations (Commands) and read operations (Queries) using **MediatR**.
* **Validation Pipeline:** Automated request validation leveraging **FluentValidation** integrated directly into the MediatR pipeline behaviors.
* **Global Exception Handling:** Centralized middleware (`GlobalExceptionHandlerMiddleware`) capturing unhandled exceptions and standardizing API error responses (`404 Not Found`, `400 Bad Request`, `500 Internal Server Error`) with clean JSON structures.

### 2. Architecture & Tech Stack
* **Backend Framework:** .NET 10 / C# 13, ASP.NET Core Web API.
* **Architecture:** Clean Architecture, Vertical Slice principles.
* **Data Access:** Entity Framework Core with SQL Server 2025.
* **Messaging & Resilience:** Designed for asynchronous event-driven communication (RabbitMQ, Wolverine, Transactional Outbox Pattern).
* **API Gateway & Querying:** YARP API Gateway and GraphQL (Hot Chocolate engine) for advanced filtering, sorting, and pagination.
* **Real-Time & Security:** SignalR for live communications, JWT Authentication, Role-Based Access Control (RBAC), and Google OAuth 2.0.

---

## 🛠️ Project Structure

```text
HealthcarePlatform/
│
├── src/
│   ├── Services/
│   │   ├── Patient/
│   │   │   ├── Patient.Domain           # Entities, Enums, and Domain Invariants
│   │   │   ├── Patient.Application      # CQRS Handlers, DTOs, Validators, Interfaces
│   │   │   ├── Patient.Infrastructure   # EF Core DbContext, Configurations, Persistence
│   │   │   └── Patient.API              # Controllers, Middlewares, Program.cs
│   │   │
│   │   └── [Future Microservices...]    # Billing, EHR, Identity, etc.
│   │
└── 📁 Healthcare Platform Execution Roadmap.pdf  # System Architecture Blueprint