# HR Management System API

A multi-tenant Human Resources Management System built with **ASP.NET Core**, following **Clean Architecture** principles.  
The system provides end-to-end HR workflows: employees, departments, requests (leave, financial, resignation, data change), recruitment, AI CV ranking, notifications, multi-tenancy, and more.

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Technology Stack](#technology-stack)
4. [Solution Structure](#solution-structure)
5. [Core Features](#core-features)
6. [Modules & Workflows](#modules--workflows)
7. [Getting Started](#getting-started)
8. [Configuration](#configuration)
9. [Multi-Tenancy](#multi-tenancy)
10. [AI CV Ranking](#ai-cv-ranking)
11. [Background Jobs & Scheduling](#background-jobs--scheduling)
12. [Health Checks & Observability](#health-checks--observability)
13. [Error Handling & Validation](#error-handling--validation)
14. [Testing](#testing)
15. [Future Enhancements](#future-enhancements)

---

## Project Overview

The **HR Management System** is a back-end API that centralizes HR operations for multiple companies (tenants) in a single platform.

**Key capabilities:**

- Multi-tenant HR platform (one database, tenant isolation with filters)
- Employee & department management
- HR requests (leave, financial, resignation, employee data change)
- Recruitment module (job positions, job applications, CV upload)
- AI-powered CV ranking using **OpenAI Responses API**
- Authentication & authorization with JWT and role-based access
- Email notifications (welcome email, reminders)
- Quartz scheduled jobs + Hangfire dashboard
- Centralized logging with Serilog
- Health checks & rate limiting

---

## Architecture

The project follows **Clean Architecture / Onion Architecture**:

- **API Layer**  
  - ASP.NET Core Web API, controllers, middleware, health checks, rate-limiting, authentication wiring, OpenAI & localization configuration.

- **Application Layer**  
  - Business logic, use cases, services, DTO mapping, interfaces for external dependencies (repositories, file storage, email, OpenAI).

- **Infrastructure Layer**  
  - EF Core DbContext, repositories, multi-tenant context, data seeding, infrastructure implementations (CurrentUser, CurrentTenant).

- **Domain Layer**  
  - Pure domain entities (Employee, Department, GenericRequest, JobPosition, Tenant, etc.), base entities, tenant entities, role constants.

- **Shared Layer**  
  - Enums, shared resource for localization (Arabic & English), cross-layer types.

- **Testing Layer**  
  - Unit tests for core services (e.g., financial requests, validations & messages).

This separation makes the solution:

- **Testable** – all business logic is behind interfaces in the Application layer  
- **Maintainable** – UI/API can change without touching domain rules  
- **Replaceable** – infrastructure (DB, OpenAI provider, email provider, file storage) can be swapped with minimal changes

---

## Technology Stack

**Core**

- .NET **9.0**
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server

**Architecture & Patterns**

- Clean Architecture / Onion Architecture
- Generic Repository
- Multi-Tenancy with `CurrentTenant`
- DTO + AutoMapper profiles
- Dependency Injection via extension methods

**Security**

- JWT Authentication
- Role-based Authorization
- Custom roles:
  - `SystemAdmin`, `HrAdmin`, `Manager`, `Employee`, `Recruiter`

**AI & Integrations**

- OpenAI Responses API for CV ranking (`gpt-4o-mini`)
- SMTP email sender
- File storage for CV PDFs

**Background Jobs & Observability**

- Quartz.NET (leave accrual, reminders)
- Hangfire (optional dashboard)
- Health Checks
- Serilog logging
- ASP.NET Core rate limiting

---

## Solution Structure

```text
Hr_Managment_System/
│
├── HrManagmentSystem_API/                 # API Layer (Presentation)
│   ├── Controllers/
│   │   ├── AiCvRankingController.cs       # Manual CV ranking endpoint (OpenAI)
│   │   ├── AuthController.cs              # Login, token, HR onboarding
│   │   ├── DepartmentsController.cs       # CRUD + hierarchy + manager assignment
│   │   ├── DocumentCvController.cs        # CV upload, metadata, download
│   │   ├── EmployeeDataChangeRequestsController.cs # Change personal data workflow
│   │   ├── EmployeesController.cs         # Employees CRUD + manager assignment + roles
│   │   ├── FinancialRequestsController.cs # Loans / financial requests
│   │   ├── JobPositionController.cs       # Job positions + applications + status
│   │   ├── LeaveRequestsController.cs     # Leave requests, balance, approvals
│   │   ├── RequestsController.cs          # Generic requests listing / history
│   │   ├── ResignationRequestsController.cs # Resignation process
│   │   └── TenantsController.cs           # Tenants management (SystemAdmin only)
│   │
│   ├── ExtensionMethod/
│   │   ├── AuthenticationExtensions.cs    # JWT, Identity, current user
│   │   ├── HangfireExtensions.cs          # Hangfire server setup
│   │   ├── HealthChecksExtensions.cs      # Custom health checks registration
│   │   ├── LocalizationExtensions.cs      # Arabic/English localization
│   │   ├── OpenAiExtensions.cs            # OpenAI options & HTTP client
│   │   ├── RateLimitingExtensions.cs      # ASP.NET rate limiting policies
│   │   └── SerilogExtensions.cs           # Logging configuration
│   │
│   ├── HealthCheck/
│   │   └── SystemDataHealthCheck.cs       # Checks seeded data & system consistency
│   │
│   ├── Middleware/
│   │   └── CurrentTenantMiddleware.cs     # Resolves tenant per request
│   │
│   ├── Logs/                              # Serilog text logs
│   ├── Uploads/
│   │   └── Cvs/                           # Stored CV PDFs (FileStorageService)
│   │
│   ├── appsettings.json                   # Base configuration
│   ├── HrManagmentSystem_API.http         # HTTP test file for Visual Studio
│   └── Program.cs                         # Application entry point & pipeline
│
├── HrMangmentSystem_Application/          # Application Layer (Use Cases)
│   ├── Common/
│   │   └── Paged/
│   │       └── Paged.cs                   # PagedResult & PagedRequest
│   │
│   ├── Responses/
│   │   └── ApiResponse.cs                 # Unified API response wrapper
│   │
│   ├── Security/
│   │   └── PasswordGenerator.cs           # Temporary password generator
│   │
│   ├── Config/                            # Options bound from appsettings
│   │   ├── FileStorageOptions.cs
│   │   ├── LeaveAccrualOptions.cs
│   │   ├── LeaveBalanceOptions.cs
│   │   ├── OpenAiOptions.cs
│   │   └── SmtpOptions.cs
│   │
│   ├── ExtensionMethod/
│   │   ├── ApplicationServicesExtensions.cs # Registers all services & interfaces
│   │   ├── AutoMapperExtensions.cs        # Adds AutoMapper profiles
│   │   └── QuartzExtensions.cs            # Registers Quartz jobs (leave accrual)
│   │
│   ├── Implementation/
│   │   ├── Auth/
│   │   │   ├── AuthenticationService.cs   # Login, JWT, onboarding
│   │   │   └── JwtSecurityToken.cs        # Token generation wrapper
│   │   │
│   │   ├── FileStorage/
│   │   │   └── FileStorageService.cs      # Save CVs to disk with safe naming
│   │   │
│   │   ├── Notifications/
│   │   │   └── EmailSender.cs             # SMTP email notifications
│   │   │
│   │   ├── OpenAi/
│   │   │   └── OpenAiCvScoringClient.cs   # Calls OpenAI Responses API for CV score
│   │   │
│   │   ├── Requests/
│   │   │   ├── EmployeeDataChangeRequestService.cs
│   │   │   ├── FinancialRequestService.cs
│   │   │   ├── LeaveAccrualService.cs
│   │   │   ├── LeaveBalanceService.cs
│   │   │   ├── LeaveRequestService.cs
│   │   │   ├── PendingRequestsReminderService.cs
│   │   │   ├── RequestService.cs          # Shared request status workflow
│   │   │   └── ResignationRequestService.cs
│   │   │
│   │   ├── Services/
│   │   │   ├── CvRankingService.cs        # Bridges job application + OpenAI score
│   │   │   ├── DepartmentService.cs
│   │   │   ├── DocumentCvService.cs
│   │   │   ├── EmployeeRoleService.cs
│   │   │   ├── EmployeeService.cs
│   │   │   ├── JobApplicationService.cs
│   │   │   ├── JobPositionService.cs
│   │   │   └── TenantService.cs
│   │   │
│   │   ├── Tenant/
│   │   │   └── TenantService.cs           # Tenant CRUD & status control
│   │   │
│   │   └── Job/
│   │       └── LeaveAccrualJob.cs         # Scheduled monthly leave accrual
│   │
│   ├── Interfaces/
│   │   ├── Auth/
│   │   ├── FileStorage/
│   │   ├── Notifications/
│   │   ├── OpenAi/
│   │   ├── Requests/
│   │   ├── Services/
│   │   └── Tenant/
│   │
│   └── Mapper/                            # AutoMapper profiles
│       ├── DepartmentProfile.cs
│       ├── DocumentCvProfile.cs
│       ├── EmployeeProfile.cs
│       ├── JobApplicationProfile.cs
│       ├── JobPositionProfile.cs
│       ├── RequestProfile.cs
│       └── TenantProfile.cs
│
├── HrMangmentSystem_Infrastructure/       # Infrastructure Layer
│   ├── ExtensionMethod/
│   │   └── ConfigureDatabasesExtension.cs # DbContext + connection string wiring
│   │
│   ├── Implementations/
│   │   ├── Identity/
│   │   │   └── HttpCurrentUser.cs         # Reads user & tenant from HttpContext
│   │   │
│   │   └── Repositories/
│   │       ├── GenericRepository.cs
│   │       ├── SoftDeleteRepository.cs
│   │       └── TenantRepository.cs        # Special repo for TenantEntity
│   │
│   ├── Interfaces/Repository
│   │   ├── ICurrentTenant.cs
│   │   ├── ICurrentUser.cs
│   │   ├── IGenericRepository.cs
│   │   └── ITenantRepository.cs
│   │
│   ├── Migrations/                        # EF Core migrations
│   ├── Models/
│   │   └── AppDbContext.cs                # DbSets, modelBuilder, tenant filters
│   │
│   ├── SeedingData/
│   │   └── DataSeeding.cs                 # Seed Tenants, Departments, Roles, Employees
│   │
│   └── Tenant/
│       └── CurrentTenant.cs               # Holds TenantId for current scope
│
├── HrMangmentSystem_Domain/               # Domain Layer
│   ├── Common/
│   │   ├── BaseEntity.cs
│   │   ├── IAuditableEntity.cs
│   │   ├── ITenantEntity.cs
│   │   └── SoftDeletable.cs
│   │
│   ├── Constants/
│   │   └── RoleName.cs                    # Static role name constants
│   │
│   ├── Entities/
│   │   ├── Employees/                     # Employee, LeaveBalance, etc.
│   │   ├── Recruitment/                   # JobPosition, JobApplication, DocumentCv
│   │   ├── Requests/                      # GenericRequest, LeaveRequest, FinancialRequest, EmployeeDataChange, RequestHistory
│   │   └── Roles/                         # Role, EmployeeRole
│   │
│   └── Tenants/
│       └── Tenants.cs                     # TenantEntity definition
│
├── HrMangmentSystem_Shared/               # Shared Layer
│   ├── Enum/
│   │   ├── Employee/
│   │   │   ├── DocumentType.cs
│   │   │   ├── EmployeeStatus.cs
│   │   │   └── Gender.cs
│   │   ├── Recruitment/
│   │   │   └── JobApplicationStatus.cs
│   │   ├── Request/
│   │   │   ├── FinancialType.cs
│   │   │   ├── LeaveType.cs
│   │   │   ├── RequestAction.cs
│   │   │   ├── RequestStatus.cs
│   │   │   └── RequestType.cs
│   │   └── Roles/
│   │       └── RoleType.cs
│   │
│   └── Resources/
│       └── SharedResource.cs              # Localization anchor (Resx)
│
└── Hr_ManagmentSystem_Test/               # Testing Layer
    └── FinancialRequestServiceTests.cs    # Unit tests for FinancialRequestService
