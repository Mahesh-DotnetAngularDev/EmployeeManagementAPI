# Employee Management API

A production-style Employee Management REST API built with **ASP.NET Core 10**, **Entity Framework Core**, and **PostgreSQL**.

This project demonstrates practical backend development concepts including RESTful APIs, DTOs, validation, service-layer architecture, exception handling, logging, pagination, filtering, searching, sorting, and database optimization.

---

## 🚀 Technologies

- .NET 10
- ASP.NET Core Web API
- C#
- Entity Framework Core
- PostgreSQL
- Npgsql
- Swagger / OpenAPI
- Dependency Injection
- Git / GitHub

---

## ✨ Features

- Employee CRUD operations
- DTO-based API requests
- Request validation
- Service layer architecture
- Dependency Injection
- PostgreSQL database integration
- Entity Framework Core migrations
- Unique email constraint
- Global exception handling
- Structured logging
- Pagination
- Employee search
- Department filtering
- Sorting
- Swagger / OpenAPI documentation
- Database indexes for optimized filtering

---

## 🏗️ Architecture

The application follows a layered architecture:

```text
Client
  ↓
Controller
  ↓
DTO / Validation
  ↓
Service Layer
  ↓
Entity Framework Core
  ↓
PostgreSQL