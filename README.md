# CompanyEmployees API

A comprehensive ASP.NET Core Web API built with Clean Architecture principles, CQRS pattern, and modern best practices.

## Project Architecture

This project follows **Clean Architecture** with clear separation of concerns across multiple layers:

```
CompanyEmployees/
??? CompanyEmployees.Core.Domain/          # Domain Layer (Entities, Interfaces)
??? CompanyEmployees.Application/          # Application Layer (Business Logic)
??? CompanyEmployees.Infrastructure.Persistence/  # Data Access Layer
??? CompanyEmployees.Infrastructure.Presentation/ # API Controllers
??? CompanyEmployees/                      # Main API Project (Composition Root)
??? Shared/                                # Shared DTOs and Models
??? LoggingService/                        # Logging Infrastructure
```

### Layer Responsibilities

#### 1. **Core.Domain Layer** 
- **Purpose**: Contains enterprise business rules and entities
- **Contents**:
  - Domain entities (`Employee`, `Company`)
  - Repository interfaces
  - Domain-specific exceptions
- **Dependencies**: None (most inner layer)

#### 2. **Application Layer**
- **Purpose**: Contains application business logic and orchestration
- **Contents**:
  - CQRS Commands and Queries
  - Command/Query Handlers
  - Validators (FluentValidation)
  - Notifications
  - Behaviors (e.g., ValidationBehavior)
- **Dependencies**: Core.Domain, Shared
- **Key Patterns**:
  - **CQRS** (Command Query Responsibility Segregation)
  - **MediatR** for request/response and notifications
  - **FluentValidation** for input validation

#### 3. **Infrastructure.Persistence Layer**
- **Purpose**: Data access implementation
- **Contents**:
  - DbContext (Entity Framework Core)
  - Repository implementations
  - Database configurations
  - Migrations
- **Dependencies**: Core.Domain
- **Technologies**: Entity Framework Core, SQL Server

#### 4. **Infrastructure.Presentation Layer**
- **Purpose**: API endpoints and controllers
- **Contents**:
  - API Controllers
  - Route definitions
- **Dependencies**: Application, Shared
- **Pattern**: Thin controllers delegating to MediatR

#### 5. **Main API Project (CompanyEmployees)**
- **Purpose**: Composition root and application startup
- **Contents**:
  - `Program.cs` - Application configuration
  - Service registrations and DI setup
  - AutoMapper profiles
  - Global exception handling
  - Middleware configuration

#### 6. **Shared Layer**
- **Purpose**: Cross-cutting concerns and DTOs
- **Contents**:
  - Data Transfer Objects (DTOs)
  - Shared models

#### 7. **LoggingService**
- **Purpose**: Centralized logging
- **Technologies**: Serilog

## ?? Design Patterns & Principles

### 1. **Clean Architecture**
- Dependency rule: Inner layers don't depend on outer layers
- Domain-centric design
- Framework independence in core business logic

### 2. **CQRS (Command Query Responsibility Segregation)**
- **Commands**: Modify state (Create, Update, Delete)
- **Queries**: Read data (Get, GetAll)
- Separate models for reads and writes

### 3. **Repository Pattern**
- Abstracts data access logic
- `IRepositoryManager` - Unit of Work pattern
- Generic `RepositoryBase<T>` for common operations

### 4. **Mediator Pattern**
- **MediatR** library for request/response
- Decouples controllers from business logic
- Supports pipeline behaviors (validation, logging, etc.)

### 5. **Dependency Injection**
- Constructor injection throughout
- Service registration in `Program.cs`
- Interface-based design

### 6. **AutoMapper**
- Object-to-object mapping
- Separates DTOs from domain entities
- Configured via `MappingProfile`

### 7. **FluentValidation**
- Declarative validation rules
- Automatic validation via pipeline behavior
- Custom validation logic support

### 8. **Global Exception Handling**
- Centralized error handling
- Consistent error responses
- Problem Details (RFC 7807)

## ?? Standards & Conventions

### Naming Conventions
- **Entities**: PascalCase (`Employee`, `Company`)
- **DTOs**: Suffixed with purpose (`CompanyForCreationDto`, `CompanyDto`)
- **Commands**: Action + Entity + "Command" (`CreateCompanyCommand`)
- **Queries**: "Get" + Entity + "Query" (`GetCompanyQuery`)
- **Handlers**: Command/Query name + "Handler" (`CreateCompanyHandler`)
- **Validators**: Command name + "Validator" (`CreateCompanyCommandValidator`)

### Project Organization
```
Application/
??? Commands/           # All command definitions
??? Queries/            # All query definitions
??? Handlers/           # Command and query handlers
??? Validators/         # FluentValidation validators
??? Notifications/      # MediatR notifications
??? Behaviors/          # Pipeline behaviors
```

### Coding Standards
- **Async/Await**: All I/O operations are asynchronous
- **Cancellation Tokens**: Supported for long-running operations
- **Nullable Reference Types**: Enabled (.NET 9)
- **Records**: Used for immutable commands/queries
- **Sealed Classes**: Internal handlers are sealed for performance

## ?? How to Create a New API Endpoint

Follow this comprehensive guide to add a new API endpoint. We'll use creating an **Employee API** as an example.

### Step 1: Define the Entity (if not exists)
**Location**: `CompanyEmployees.Core.Domain/Entities/`

```csharp
// Employee.cs - Already exists in your project
public class Employee
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Position { get; set; }
    public Guid CompanyId { get; set; }
    public Company Company { get; set; }
}
```

### Step 2: Create Repository Interface
**Location**: `CompanyEmployees.Core.Domain/Repositories/`

```csharp
// IEmployeeRepository.cs
public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllEmployeesAsync(Guid companyId, bool trackChanges, CancellationToken ct = default);
    Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges, CancellationToken ct = default);
    void CreateEmployee(Guid companyId, Employee employee);
    void DeleteEmployee(Employee employee);
}
```

Update `IRepositoryManager`:
```csharp
public interface IRepositoryManager
{
    ICompanyRepository Company { get; }
    IEmployeeRepository Employee { get; } // Add this
    Task SaveAsync(CancellationToken ct = default);
}
```

### Step 3: Implement Repository
**Location**: `CompanyEmployees.Infrastructure.Persistence/Repositories/`

```csharp
// EmployeeRepository.cs
internal sealed class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync(Guid companyId, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
            .OrderBy(e => e.Name)
            .ToListAsync(ct);

    public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges)
            .SingleOrDefaultAsync(ct);

    public void CreateEmployee(Guid companyId, Employee employee)
    {
        employee.CompanyId = companyId;
        Create(employee);
    }

    public void DeleteEmployee(Employee employee) => Delete(employee);
}
```

Update `RepositoryManager.cs` to include the new repository.

### Step 4: Create DTOs
**Location**: `Shared/DataTransferObjects/`

```csharp
// EmployeeDto.cs - For reading
public record EmployeeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public int Age { get; init; }
    public string Position { get; init; }
}

// EmployeeForCreationDto.cs - For creating
public record EmployeeForCreationDto
{
    public string Name { get; init; }
    public int Age { get; init; }
    public string Position { get; init; }
}

// EmployeeForUpdateDto.cs - For updating
public record EmployeeForUpdateDto
{
    public string Name { get; init; }
    public int Age { get; init; }
    public string Position { get; init; }
}
```

### Step 5: Add AutoMapper Mappings
**Location**: `CompanyEmployees/MappingProfile.cs`

```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Existing mappings...
        
        // Add Employee mappings
        CreateMap<Employee, EmployeeDto>();
        CreateMap<EmployeeForCreationDto, Employee>();
        CreateMap<EmployeeForUpdateDto, Employee>();
    }
}
```

### Step 6: Create Commands and Queries
**Location**: `CompanyEmployees.Application/Commands/` and `Queries/`

```csharp
// Commands/CreateEmployeeCommand.cs
public sealed record CreateEmployeeCommand(
    Guid CompanyId, 
    EmployeeForCreationDto Employee
) : IRequest<EmployeeDto>;

// Commands/UpdateEmployeeCommand.cs
public sealed record UpdateEmployeeCommand(
    Guid CompanyId,
    Guid Id,
    EmployeeForUpdateDto Employee,
    bool TrackChanges
) : IRequest;

// Commands/DeleteEmployeeCommand.cs
public sealed record DeleteEmployeeCommand(
    Guid CompanyId,
    Guid Id,
    bool TrackChanges
) : IRequest;

// Queries/GetEmployeesQuery.cs
public sealed record GetEmployeesQuery(
    Guid CompanyId,
    bool TrackChanges
) : IRequest<IEnumerable<EmployeeDto>>;

// Queries/GetEmployeeQuery.cs
public sealed record GetEmployeeQuery(
    Guid CompanyId,
    Guid Id,
    bool TrackChanges
) : IRequest<EmployeeDto>;
```

### Step 7: Create Handlers
**Location**: `CompanyEmployees.Application/Handlers/`

```csharp
// Handlers/CreateEmployeeHandler.cs
internal sealed class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;

    public CreateEmployeeHandler(IRepositoryManager repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        // Verify company exists
        var company = await _repository.Company.GetCompanyAsync(request.CompanyId, trackChanges: false, cancellationToken);
        if (company is null)
            throw new NotFoundException($"Company with id {request.CompanyId} not found.");

        var employeeEntity = _mapper.Map<Employee>(request.Employee);
        _repository.Employee.CreateEmployee(request.CompanyId, employeeEntity);
        await _repository.SaveAsync(cancellationToken);

        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
        return employeeToReturn;
    }
}

// Handlers/GetEmployeesHandler.cs
internal sealed class GetEmployeesHandler : IRequestHandler<GetEmployeesQuery, IEnumerable<EmployeeDto>>
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;

    public GetEmployeesHandler(IRepositoryManager repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _repository.Employee.GetAllEmployeesAsync(request.CompanyId, request.TrackChanges, cancellationToken);
        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        return employeesDto;
    }
}

// Create similar handlers for Update, Delete, and GetEmployee
```

### Step 8: Create Validators
**Location**: `CompanyEmployees.Application/Validators/`

```csharp
// Validators/CreateEmployeeCommandValidator.cs
public sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(c => c.Employee.Name)
            .NotEmpty().WithMessage("Employee name is required.")
            .MaximumLength(30).WithMessage("Maximum length for the Name is 30 characters.");

        RuleFor(c => c.Employee.Age)
            .NotEmpty().WithMessage("Age is required.")
            .GreaterThan(0).WithMessage("Age must be greater than 0.");

        RuleFor(c => c.Employee.Position)
            .NotEmpty().WithMessage("Position is required.")
            .MaximumLength(20).WithMessage("Maximum length for the Position is 20 characters.");
    }

    public override ValidationResult Validate(ValidationContext<CreateEmployeeCommand> context)
    {
        return context.InstanceToValidate.Employee is null
            ? new ValidationResult(new[] { new ValidationFailure("EmployeeForCreationDto", "Employee object is null") })
            : base.Validate(context);
    }
}
```

### Step 9: Create Controller
**Location**: `CompanyEmployees.Infrastructure.Presentation/Controllers/`

```csharp
// Controllers/EmployeesController.cs
[Route("api/companies/{companyId:guid}/employees")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly ISender _sender;

    public EmployeesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployeesForCompany(Guid companyId)
    {
        var employees = await _sender.Send(new GetEmployeesQuery(companyId, TrackChanges: false));
        return Ok(employees);
    }

    [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
    public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
    {
        var employee = await _sender.Send(new GetEmployeeQuery(companyId, id, TrackChanges: false));
        return Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
    {
        if (employee is null)
            return BadRequest("EmployeeForCreationDto object is null");

        var employeeToReturn = await _sender.Send(new CreateEmployeeCommand(companyId, employee));
        return CreatedAtRoute("GetEmployeeForCompany", 
            new { companyId, id = employeeToReturn.Id }, 
            employeeToReturn);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee)
    {
        if (employee is null)
            return BadRequest("EmployeeForUpdateDto object is null");

        await _sender.Send(new UpdateEmployeeCommand(companyId, id, employee, TrackChanges: true));
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
    {
        await _sender.Send(new DeleteEmployeeCommand(companyId, id, TrackChanges: false));
        return NoContent();
    }
}
```

### Step 10: Register Services (if needed)
**Location**: `CompanyEmployees/Program.cs` or extension methods

Most services are registered automatically via MediatR assembly scanning, but ensure:
- Repository is added to `RepositoryManager`
- AutoMapper profile includes new mappings
- Validators are in the correct assembly

## ?? Checklist for New API Endpoint

When creating a new API endpoint, ensure you've completed:

- [ ] **Domain Layer**
  - [ ] Entity created/updated
  - [ ] Repository interface defined
  - [ ] Added to IRepositoryManager

- [ ] **Persistence Layer**
  - [ ] Repository implementation created
  - [ ] Added to RepositoryManager
  - [ ] Database configuration (if needed)
  - [ ] Migration created (if schema changed)

- [ ] **Shared Layer**
  - [ ] DTOs created (Read, Create, Update)

- [ ] **Application Layer**
  - [ ] Commands created (Create, Update, Delete)
  - [ ] Queries created (Get, GetAll)
  - [ ] Handlers implemented for all commands/queries
  - [ ] Validators created for all commands
  - [ ] Notifications created (if needed)

- [ ] **Presentation Layer**
  - [ ] Controller created with all endpoints
  - [ ] Routes properly defined
  - [ ] HTTP verbs correctly applied

- [ ] **Main Project**
  - [ ] AutoMapper mappings added
  - [ ] Services registered (if custom)

- [ ] **Testing**
  - [ ] Build succeeds
  - [ ] API endpoints tested
  - [ ] Validation works correctly
  - [ ] Error handling verified

## ?? Technologies & Libraries

### Core
- **.NET 9** - Target framework
- **C# 13.0** - Programming language
- **ASP.NET Core** - Web API framework

### Data Access
- **Entity Framework Core 9.0** - ORM
- **SQL Server** - Database

### Architecture & Patterns
- **MediatR 12.4.1** - CQRS and Mediator pattern
- **AutoMapper 13.0.1** - Object mapping
- **FluentValidation 11.10.0** - Input validation

### Logging
- **Serilog 8.0.3** - Structured logging
- **Serilog.Sinks.Console** - Console logging
- **Serilog.Sinks.File** - File logging
- **Serilog.Sinks.Seq** - Centralized logging

### API Features
- **Newtonsoft.Json** - JSON serialization and JSON Patch support
- **XML Formatters** - XML support for content negotiation
- **Problem Details** - RFC 7807 error responses
- **CORS** - Cross-Origin Resource Sharing

## ?? Getting Started

### Prerequisites
- .NET 9 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Setup
1. Clone the repository
2. Update connection string in `appsettings.json`
3. Run migrations: `dotnet ef database update`
4. Run the application: `dotnet run --project CompanyEmployees`

### Configuration
Key configuration in `appsettings.json`:
- Database connection strings
- Serilog settings
- CORS policies

## ?? Additional Resources

### Key Files to Reference
- `Program.cs` - Application startup and DI configuration
- `MappingProfile.cs` - AutoMapper configuration
- `GlobalExceptionHandler.cs` - Error handling
- `ValidationBehavior.cs` - Validation pipeline

### Common Tasks
- **Add Migration**: `dotnet ef migrations add <MigrationName> --project CompanyEmployees.Infrastructure.Persistence --startup-project CompanyEmployees`
- **Update Database**: `dotnet ef database update --project CompanyEmployees.Infrastructure.Persistence --startup-project CompanyEmployees`
- **Run Application**: `dotnet run --project CompanyEmployees`

## ?? License

[Add your license information here]

## ?? Contributors

[Add contributor information here]

---

**Note**: This architecture is designed to be scalable, maintainable, and testable. Each layer has a specific responsibility, and dependencies flow inward toward the domain layer. Following these patterns and conventions will ensure consistency across the codebase.
