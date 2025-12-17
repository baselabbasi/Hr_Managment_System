# HR Management System API

A multi-tenant Human Resources Management System built with **ASP.NET Core**, following **Clean Architecture** principles.  The system provides end-to-end HR workflows:  employees, departments, requests (leave, financial, resignation, data change), recruitment, AI CV ranking, notifications, multi-tenancy, and more.

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Technology Stack](#technology-stack)
4. [Solution Structure](#solution-structure)
5. [Core Features](#core-features)
6. [Getting Started](#getting-started)
7. [Configuration](#configuration)
8. [Multi-Tenancy](#multi-tenancy)
9. [AI CV Ranking](#ai-cv-ranking)
10. [Background Jobs & Scheduling](#background-jobs--scheduling)
11. [Health Checks & Observability](#health-checks--observability)
12. [Error Handling & Validation](#error-handling--validation)
13. [Testing](#testing)
14. [Future Enhancements](#future-enhancements)

---

## Project Overview

The **HR Management System** is a back-end API that centralizes HR operations for multiple companies (tenants) in a single platform. 

**Key capabilities:**

- ✅ Multi-tenant HR platform (one database, tenant isolation with filters)
- ✅ Employee & department management
- ✅ HR requests (leave, financial, resignation, employee data change)
- ✅ Recruitment module (job positions, job applications, CV upload)
- ✅ AI-powered CV ranking using **OpenAI Responses API**
- ✅ Authentication & authorization with JWT and role-based access
- ✅ Email notifications (welcome email, reminders)
- ✅ Quartz scheduled jobs + Hangfire dashboard
- ✅ Centralized logging with Serilog
- ✅ Health checks & rate limiting

---

## Architecture

The project follows **Clean Architecture / Onion Architecture**:

### **API Layer**
- ASP.NET Core Web API controllers
- Middleware, health checks, rate-limiting
- Authentication wiring
- OpenAI & localization configuration

### **Application Layer**
- Business logic, use cases, services
- DTO mapping, interfaces for external dependencies
- Repository patterns, file storage, email, OpenAI integrations

### **Infrastructure Layer**
- EF Core DbContext, repositories
- Multi-tenant context, data seeding
- Infrastructure implementations (CurrentUser, CurrentTenant)

### **Domain Layer**
- Pure domain entities (Employee, Department, GenericRequest, JobPosition, Tenant, etc.)
- Base entities, tenant entities
- Role constants

### **Shared Layer**
- Enums, localization resources (Arabic & English)
- Cross-layer types

### **Testing Layer**
- Unit tests for core services
- Financial requests, validations & messages

This separation makes the solution: 

- **Testable** – all business logic is behind interfaces
- **Maintainable** – UI/API can change without touching domain rules
- **Replaceable** – infrastructure can be swapped with minimal changes

---

## Technology Stack

**Core**
- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server

**Architecture & Patterns**
- Clean Architecture / Onion Architecture
- Generic Repository Pattern
- Multi-Tenancy with `CurrentTenant`
- DTO + AutoMapper profiles
- Dependency Injection via extension methods

**Security**
- JWT Authentication
- Role-based Authorization
- Custom roles:  `SystemAdmin`, `HrAdmin`, `Manager`, `Employee`, `Recruiter`

**AI & Integrations**
- OpenAI Responses API for CV ranking (`gpt-4o-mini`)
- SMTP email sender
- File storage for CV PDFs

**Background Jobs & Observability**
- Quartz. NET (leave accrual, reminders)
- Hangfire (optional dashboard)
- Health Checks
- Serilog logging
- ASP.NET Core rate limiting

---

## Solution Structure

```
Hr_Managment_System/
│
├── HrManagmentSystem_API/                 # API Layer (Presentation)
│   ├── Controllers/
│   │   ├── AiCvRankingController.cs       # Manual CV ranking endpoint (OpenAI)
│   │   ├── AuthController.cs              # Login, token, HR onboarding
│   │   ├── DepartmentsController.cs       # CRUD + hierarchy + manager assignment
│   │   ├── DocumentCvController.cs        # CV upload, metadata, download
│   │   ├── EmployeeDataChangeRequestsController.cs
│   │   ├── EmployeesController.cs         # CRUD + manager assignment + roles
│   │   ├── FinancialRequestsController.cs # Loans / financial requests
│   │   ├── JobPositionController.cs       # Job positions + applications + status
│   │   ├── LeaveRequestsController.cs     # Leave requests, balance, approvals
│   │   ├── RequestsController.cs          # Generic requests listing / history
│   │   ├── ResignationRequestsController.cs
│   │   └── TenantsController.cs           # Tenants management (SystemAdmin only)
│   │
│   ├── ExtensionMethod/
│   │   ├── AuthenticationExtensions.cs
│   │   ├── HangfireExtensions.cs
│   │   ├── HealthChecksExtensions.cs
│   │   ├── LocalizationExtensions.cs
│   │   ├── OpenAiExtensions.cs
│   │   ├── RateLimitingExtensions.cs
│   │   └── SerilogExtensions.cs
│   │
│   ├── HealthCheck/
│   ├── Middleware/
│   ├── Logs/
│   ├── Uploads/Cvs/
│   ├── appsettings.json
│   └── Program.cs
│
├── HrMangmentSystem_Application/          # Application Layer (Use Cases)
│   ├── Common/
│   ├── Responses/
│   ├── Security/
│   ├── Config/
│   ├── ExtensionMethod/
│   ├── Implementation/
│   │   ├── Auth/
│   │   ├── FileStorage/
│   │   ├── Notifications/
│   │   ├── OpenAi/
│   │   ├── Requests/
│   │   ├── Services/
│   │   ├── Tenant/
│   │   └── Job/
│   ├── Interfaces/
│   └── Mapper/
│
├── HrMangmentSystem_Infrastructure/       # Infrastructure Layer
│   ├── ExtensionMethod/
│   ├── Implementations/
│   ├── Interfaces/
│   ├── Migrations/
│   ├── Models/
│   ├── SeedingData/
│   └── Tenant/
│
├── HrMangmentSystem_Domain/               # Domain Layer
│   ├── Common/
│   ├── Constants/
│   ├── Entities/
│   └── Tenants/
│
├── HrMangmentSystem_Shared/               # Shared Layer
│   ├── Enum/
│   └── Resources/
│
└── Hr_ManagmentSystem_Test/               # Testing Layer
    └── FinancialRequestServiceTests.cs
```

---

## Core Features

### 1. **Multi-Tenant HR Platform**
- Each record implements `ITenantEntity` and is automatically filtered by `TenantId`
- `CurrentTenantMiddleware` sets the tenant for each request
- `SystemAdmin` can manage tenants via `TenantsController`

### 2. **Authentication & Authorization**
- JWT-based authentication via `AuthenticationExtensions`
- Login & token generation
- Create employee account + welcome email with temporary password
- Role-based access using constants from `RoleName` and `RoleType`
- Roles: SystemAdmin, HrAdmin, Manager, Employee, Recruiter

### 3. **Employees & Departments**
- **EmployeesController**: CRUD operations, assign/update manager, update employee role
- **DepartmentsController**: CRUD operations, set department manager, set parent department
- Cycle prevention for department hierarchy
- Tenant-safe operations (no cross-tenant links)

### 4. **Requests Module (Generic Workflow)**
- **Leave requests** with date validation, overlap detection, balance checks
- **Financial requests** for loans and salary advances
- **Resignation requests** with HR approval workflow
- **Employee data change requests** for updating personal information
- Centralized `RequestService` for status transitions and history logging

### 5. **Leave Management**
- Leave request creation with validation
- Overlapping requests detection
- Leave balance checks
- Automatic balance consumption on approval
- Balance refund on cancellation
- `LeaveAccrualJob` automatically increases yearly leave balances

### 6. **Financial Requests**
- Employees create financial requests with amount and reason
- Validation for positive amounts and non-empty comments
- Unit tests for validation scenarios

### 7. **Recruitment & Job Applications**
- Create/update/delete job positions
- Activate/deactivate jobs
- Anonymous candidate CV upload
- CV metadata storage in database
- Job application linking

### 8. **AI CV Ranking**
- Implemented via `OpenAiCvScoringClient` (infrastructure-independent)
- Business logic via `CvRankingService`
- Workflow: Upload CV → Create JobPosition → Get JobApplications → Score with OpenAI
- Stores numeric MatchScore (0–100) on JobApplication
- Uses OpenAI Responses API with JSON Schema strict formatting

---

## Getting Started

### **Clone the Repository**
```bash
git clone https://github.com/baselabbasi/Hr_Managment_System.git
cd Hr_Managment_System
```

### **Update Connection String**
In `HrManagmentSystem_API/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=. ;Database=HrManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### **Configure OpenAI & SMTP (optional)**
```json
"OpenAI": {
  "Model": "gpt-4o-mini",
  "BaseUrl": "https://api.openai.com/v1/responses",
  "ApiKey": ""
},
"Smtp": {
  "Host":  "smtp.yourhost.com",
  "Port":  587,
  "User": "no-reply@yourcompany.com",
  "Password": "",
  "FromName": "HR Management System"
}
```

**Note:** Keep secrets empty in `appsettings.json` and use User Secrets or environment variables. 

### **Run EF Core Migrations**
```bash
dotnet ef database update --project HrMangmentSystem_Infrastructure --startup-project HrManagmentSystem_API
```

### **Run the API**
```bash
cd HrManagmentSystem_API
dotnet run
```

### **Open Swagger**
Browse to:  `https://localhost:{PORT}/swagger`

---

## Configuration

### **Localization**
- Arabic & English support via `SharedResource`
- All messages come from localization resources

### **Health Checks**
- `GET /health` endpoint
- Returns JSON status + detailed checks
- SystemDataHealthCheck verifies seeded data & system consistency
- SQL Server health check included

### **Logging**
- Serilog configured in `SerilogExtensions.cs`
- Logs written to text files under `HrManagmentSystem_API/Logs/`

### **Rate Limiting**
- ASP.NET Core rate limiting policies configured
- Prevents API abuse

---

## Multi-Tenancy

- `TenantEntity` base class + `ITenantEntity` interface
- `CurrentTenant` service holds the current `TenantId`
- `AppDbContext` uses global query filters for automatic tenant isolation
- Multi-tenancy enforcement in: 
  - `EmployeeService. AssignManagerAsync`
  - `DepartmentService.SetDepartmentManagerAsync`
  - `DepartmentService.SetParentDepartmentAsync`
  - `TenantService` operations
- No cross-tenant manager or department relationships allowed

---

## AI CV Ranking

**Flow:**
1. Candidate uploads CV → `DocumentCv` stored with physical path
2. HR creates `JobPosition` and receives `JobApplications`
3. `CvRankingService. CalculateMatchScoreAsync(jobApplicationId)`:
   - Resolves job description (Title + Description + Requirements)
   - Resolves CV physical path from `FilePath`
   - Calls `OpenAiCvScoringClient.GetScoreAsync(jobText, pdfPath)`
   - Stores numeric `MatchScore` (0–100) on `JobApplication`
4. Manual ranking endpoint:  `POST /api/aicvranking/rank/{jobApplicationId}`

**OpenAI Integration:**
- Uses Responses API (`/v1/responses`)
- JSON Schema strict formatting for guaranteed output
- Robust JSON parsing with multiple fallbacks + regex

---

## Background Jobs & Scheduling

### **Quartz.NET**
- Registered via `QuartzExtensions.cs`
- `LeaveAccrualJob` runs periodically using `LeaveAccrualOptions`
- Configurable schedule for leave accrual calculations

### **Hangfire** (Optional)
- Optional Hangfire server for visual dashboard
- Background processing future-ready

### **Pending Requests Reminder**
- `PendingRequestsReminderService` scans for pending requests
- Sends reminders to managers/HR

---

## Health Checks & Observability

### **Health Check Endpoints**
- `GET /health` returns comprehensive system status
- Checks database connectivity
- Verifies seeded data & system consistency

### **Logging**
- Centralized logging with Serilog
- Structured logs for better observability
- File-based log persistence

---

## Error Handling & Validation

### **Unified API Response**
All service methods return `ApiResponse<T>`:
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string?  Message { get; set; }
    public T? Data { get; set; }
    public object? Errors { get; set; }
    public string? TraceId { get; set; }
}
```

### **Validation Strategy**
- Controllers validate `ModelState` for DTOs
- Return `BadRequest` with validation errors when invalid
- Return `NotFound` for missing entities
- Localized error messages from `SharedResource`

### **Common Validation Messages**
- `LeaveRequest_InvalidDateRange`
- `LeaveBalance_Insufficient`
- `Employee_ManagerCannotBeSelf`
- `Tenant_CrossTenantNotAllowed`

---

## Testing

Unit tests live in `Hr_ManagmentSystem_Test`:

**FinancialRequestServiceTests covers:**
- Invalid amount validation
- Missing employee validation
- Insufficient balance scenarios
- Success path

**Run Tests:**
```bash
dotnet test
```

---

## Future Enhancements

- [ ] Full UI (Angular/React/Blazor) for HR portal & employee self-service
- [ ] Comprehensive unit & integration tests for all request types
- [ ] Notification templates & in-app notifications
- [ ] Audit logs UI dashboard
- [ ] SSO / external identity provider integration
- [ ] Export reports (Excel/PDF) for HR dashboards
- [ ] Webhooks for external payroll or ERP integration
- [ ] Performance analytics & reporting
- [ ] Mobile app support

---

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. 

---

## License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## Support

For questions or issues, please open an issue on the repository. 

---

**Happy HR Managing!  🚀**
