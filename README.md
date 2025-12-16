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
Core Features
1. Multi-Tenant HR Platform

Each record implements ITenantEntity and is automatically filtered by TenantId.

CurrentTenantMiddleware sets the tenant for each request.

SystemAdmin can manage tenants via TenantsController.

2. Authentication & Authorization

JWT-based authentication via AuthenticationExtensions.

AuthController:

Login / token generation

Create employee account + send welcome email with temporary password

Role-based access using constants from RoleName and RoleType.

Examples:

Only SystemAdmin can access /api/tenants.

HrAdmin + Recruiter manage job positions.

Employee creates leave / financial / data change / resignation requests.

3. Employees & Departments

EmployeesController

CRUD operations on employees

Assign / update manager

Update employee role (EmployeeRoleService)

DepartmentsController

CRUD operations on departments

Set department manager

Set parent department with cycle prevention (no infinite hierarchy loops)

Tenant-safe: no cross-tenant links between employees/departments

4. Requests Module (Generic Workflow)

All request types share a common GenericRequest entity with history and status:

Leave requests

Financial requests

Resignation

Employee data change

RequestService centralizes status transitions and RequestHistory logging.

5. Leave Management

Leave request creation with:

Date range validation

Overlapping requests detection

Leave balance check (LeaveBalanceService)

On approval:

Balance is consumed (TryConsumeLeaveAsync)

On cancellation:

Remaining days are refunded (if future or partially used)

LeaveAccrualJob + LeaveAccrualService automatically increases yearly leave balances.

6. Financial Requests

Employees create financial requests (loan, salary advance…).

Validation:

Positive amount

Comment / reason not empty

Unit tests in FinancialRequestServiceTests cover:

Invalid amount

Missing employee

Success path

7. Employee Data Change Requests

User sends list of field changes (phone, address, etc.) as JSON.

Stored in EmployeeDataChange.RequestedDataJson.

HR can review and apply approved changes later.

Full audit history with RequestHistory.

8. Resignation Requests

Flow:

Employee submits resignation request.

HR / Manager decides status (approve/reject).

Reason & last working day captured.

Uses the same generic request engine.

9. Recruitment & Job Applications

JobPositionController

Create / update / delete job positions

Activate/deactivate jobs

List all positions (with pagination & filtering)

Public Apply Flow

Anonymous candidate uploads CV via POST /api/jobposition/{jobPositionId}/cv/upload

CV saved physically using FileStorageService

DocumentCv metadata stored in DB

JobApplicationService links job + CV + candidate data

Multi-Tenancy

TenantEntity base + ITenantEntity interface

CurrentTenant service holds the current TenantId.

AppDbContext uses global query filters:

modelBuilder.Entity<Employee>()
    .HasQueryFilter(e => !_currentTenant.IsSet || e.TenantId == _currentTenant.TenantId);


TenantsController allows SystemAdmin to:

Create tenants

Update name & status (IsActive)

List tenants

Multi-tenancy rules are enforced in:

EmployeeService.AssignManagerAsync

DepartmentService.SetDepartmentManagerAsync

DepartmentService.SetParentDepartmentAsync

TenantService operations

No cross-tenant manager or department relationships are allowed.

AI CV Ranking

Implemented in Application Layer via:

OpenAiCvScoringClient (infrastructure-independent client)

CvRankingService (business logic around JobApplication + MatchScore)

Flow:

Candidate uploads CV → DocumentCv stored with physical path.

HR creates JobPosition and receives JobApplications.

CvRankingService.CalculateMatchScoreAsync(jobApplicationId):

Resolves job description (Title + Description + Requirements).

Resolves CV physical path from FilePath.

Calls OpenAiCvScoringClient.GetScoreAsync(jobText, pdfPath).

Stores numeric MatchScore (0–100) on JobApplication.

AiCvRankingController provides a manual ranking endpoint for testing:

POST /api/aicvranking/rank/{jobApplicationId}


OpenAI integration uses:

Responses API (/v1/responses)

JSON Schema strict formatting to guarantee { "score": number } output

Robust JSON parsing with multiple fallbacks + regex

Background Jobs & Scheduling

Quartz.NET

Registered via QuartzExtensions.cs.

LeaveAccrualJob runs periodically using LeaveAccrualOptions.

PendingRequestsReminderService

Background service that scans for pending requests and can send reminders.

HangfireExtensions

Optional Hangfire server for visual dashboard / background processing (Future-ready).

Health Checks & Observability

AddCustomHealthChecks in HealthChecksExtensions.cs:

SystemDataHealthCheck → verifies seeded tenants, roles, etc.

SQL Server health check using SELECT 1;.

Exposed via:

GET /health


Returns JSON status + detailed checks.

Logging

Serilog configured in SerilogExtensions.cs.

Logs written to text files under HrManagmentSystem_API/Logs/.

Error Handling & Validation

All service methods return ApiResponse<T>:

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public object? Errors { get; set; }
    public string? TraceId { get; set; }
}


Controllers:

Validate ModelState for DTOs.

Return BadRequest with validation errors when invalid.

Use NotFound for missing entities.

Localization:

All messages come from SharedResource (Arabic + English).

Example keys:

LeaveRequest_InvalidDateRange

LeaveBalance_Insufficient

Employee_ManagerCannotBeSelf

Tenant_CrossTenantNotAllowed

Getting Started

Clone the Repository

git clone https://github.com/baselabbasi/Hr_Managment_System.git
cd Hr_Managment_System


Update Connection String

In HrManagmentSystem_API/appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=HrManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
}


Configure OpenAI & SMTP (optional)

"OpenAI": {
  "Model": "gpt-4o-mini",
  "BaseUrl": "https://api.openai.com/v1/responses",
  "ApiKey": ""            // keep empty here
},
"Smtp": {
  "Host": "smtp.yourhost.com",
  "Port": 587,
  "User": "no-reply@yourcompany.com",
  "Password": "",
  "FromName": "HR Management System"
}


Put real secrets in User Secrets or environment variables.

Run EF Core Migrations

From the Infrastructure project folder:

dotnet ef database update --project HrMangmentSystem_Infrastructure --startup-project HrManagmentSystem_API


Run the API

cd HrManagmentSystem_API
dotnet run


Open Swagger

Browse to:

https://localhost:{PORT}/swagger

Testing

Unit tests live in Hr_ManagmentSystem_Test:

FinancialRequestServiceTests tests:

Invalid amount

Missing employee

Not enough balance

Success case

To run tests:

dotnet test

Future Enhancements

Full UI (Angular/React/Blazor) for HR portal & employee self-service

More unit & integration tests for all request types

Notification templates & in-app notifications

Audit logs UI

SSO / external identity provider integration

Export reports (Excel/PDF) for HR dashboards

Webhooks for external payroll or ERP integration

Note
This README describes the back-end architecture and can be used directly as your GitHub README.md.
You can still customize sections (like connection strings, ports, OpenAI model) to match your environment.

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
