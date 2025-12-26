# Modular Clean Architecture with Service Layer Pattern and OData Endpoints

## Disclaimer

**Warning: This sample application is still in progress**

This repository provides a comprehensive template for implementing a Modular Clean Architecture with Service Layer pattern, Facade-based design, and OData integration. It serves as a starting point and best-practice reference for building scalable, maintainable .NET applications with advanced querying capabilities.

## Recent Major Changes

**December 2025 - Architecture Refactoring**
- Migrated from CQRS pattern to Service Layer pattern
- Removed MediatR and ErrorOr dependencies
- Restructured to Facade-based architecture with clear separation of contracts and implementations
- Migrated database primary keys from Guid to ulong for improved performance
- Removed Categories and Sellers tables, simplified schema
- Split projects into ServiceFacade/Service and DataFacade/Data layers

See commit history for detailed migration information.

## Getting Started

### Development Environment Setup

1. Install .NET 9 SDK or later
2. Install Docker Desktop (if using Docker Compose)
3. Choose your database management approach:
   - **Docker Compose** - Traditional containerized database
   - **.NET Aspire** - Modern orchestration with built-in OpenTelemetry for tracing and monitoring
   - Use whichever you are comfortable with - both are fully supported

### Running with Docker Compose

```bash
cd Docker
docker compose -p docker up -d postgres
```

### Running with .NET Aspire

Follow the .NET Aspire documentation for setup and configuration. Aspire provides enhanced observability and service orchestration out of the box.

### Database Migrations

After setting up the database, run migrations:

```bash
dotnet ef database update --project Source/Data/Data
```

### API Examples

For OData request examples, refer to `Documents/Api.http`. For comprehensive OData query syntax, see the [official OData documentation](https://learn.microsoft.com/en-us/odata/).

## Architecture Overview

### Project Structure

```plaintext
Source/
├── ServiceFacade/              # Service interface contracts
│   └── RetailPortal.ServiceFacade/
│       ├── ILoginService
│       ├── IRegisterService
│       ├── IProductService
│       └── ...
│
├── Service/                    # Service implementations
│   └── RetailPortal.Service/
│       ├── LoginService
│       ├── RegisterService
│       ├── ProductService
│       └── Validators/         # FluentValidation rules
│
├── DataFacade/                 # Data access interface contracts
│   └── RetailPortal.DataFacade/
│       ├── IAggregateRepository<T>
│       ├── IReadOnlyRepository<T>
│       └── IUnitOfWork
│
├── Data/                       # Data access implementations
│   └── RetailPortal.Data/
│       ├── Db/
│       │   ├── Context/        # DbContext (split into partials)
│       │   ├── Configurations/ # Entity configurations
│       │   ├── Migrations/     # EF Core migrations
│       │   ├── Repositories/   # Repository implementations
│       │   └── Sql/           # Seed scripts
│       └── UnitOfWork/
│
├── Shared/                     # Shared projects
│   ├── RetailPortal.Model/     # Domain entities and DTOs
│   │   ├── Db/Entities/        # Database entities
│   │   ├── Db/ValueObjects/    # Value objects (Price, etc.)
│   │   └── Dto/                # Data transfer objects
│   │
│   └── RetailPortal.Shared/    # Cross-cutting utilities
│
├── Presentation/               # API and UI layers
│   └── RetailPortal.Api/
│       ├── Controllers/        # API endpoints with OData support
│       └── Middleware/
│
├── Workers/                    # Background services
│   └── RetailPortal.MigrationService/
│
└── ApiConfiguration/           # Centralized DI setup (TODO)
    └── RetailPortal.ApiConfiguration/

tests/                          # Comprehensive test coverage
├── RetailPortal.Service.Tests/
├── RetailPortal.Data.Tests/
└── RetailPortal.Api.Tests/
```

### Quick Start: Adapting the Template

Replace namespace placeholders throughout the solution:
- `RetailPortal` → Your organization/project name
- Update assembly names and namespaces accordingly

## Key Architectural Components

### Service Layer

**ServiceFacade** - Interface contracts for business logic
- Defines service contracts (ILoginService, IProductService, etc.)
- No implementation details
- Consumed by API controllers

**Service** - Business logic implementations
- Implements service interfaces
- Contains business rules and validation
- Uses FluentValidation for input validation
- Orchestrates data access through repositories

### Data Access Layer

**DataFacade** - Data access contracts
- IAggregateRepository<T> - Write operations with eager loading
- IReadOnlyRepository<T> - Read-only queries with AsNoTracking
- IUnitOfWork - Transaction management

**Data** - Infrastructure implementations
- Entity Framework Core DbContext
- Repository pattern implementations
- Database configurations
- Migrations and seed scripts

### Shared Layer

**Model** - Domain entities and DTOs
- Domain entities (User, Product, Role, Address)
- Value objects (Price, Email, etc.)
- Data transfer objects for API responses
- Shared across all layers

**Shared** - Cross-cutting utilities
- Common extensions
- Helper classes
- Shared constants

### Presentation Layer

**API Controllers**
- RESTful endpoints
- OData query support for complex filtering, sorting, pagination
- Depends on ServiceFacade interfaces
- Minimal logic - delegates to services

## Database Design

### Primary Key Strategy: ulong (numeric(20,0))

**Why ulong instead of Guid?**
- **Performance**: Sequential numeric keys provide better index performance
- **Storage**: Smaller storage footprint (8 bytes vs 16 bytes for Guid)
- **Indexing**: B-tree indexes work more efficiently with sequential integers
- **Human-readable**: Easier to debug and reference in logs
- **Compatibility**: Preserved original Guids in separate column for backward compatibility

### Schema Overview

**Core Tables**
- **Users** - User accounts with authentication
- **Roles** - Role definitions (Admin, Seller, User)
- **UserRoles** - Many-to-many relationship
- **Products** - Product catalog with Category enum
- **Addresses** - User addresses

**Key Schema Changes**
- Removed Categories table → Category enum (Electronics, Books, Clothing, HomeKitchen, ToysGames)
- Removed Sellers table → Products reference Users directly
- All tables have numeric(20,0) primary keys
- Guid column preserved for legacy compatibility

## OData Integration

### Supported OData Features

**Filtering**
```http
GET /odata/products?$filter=Category eq 'Electronics' and Quantity gt 10
```

**Sorting**
```http
GET /odata/products?$orderby=Name desc
```

**Pagination**
```http
GET /odata/products?$top=50&$skip=100
```

**Expansion (Eager Loading)**
```http
GET /odata/users?$expand=Addresses,Roles
```

**Selection (Projection)**
```http
GET /odata/products?$select=Name,Category,Quantity
```

**Complex Queries**
```http
GET /odata/products?$filter=contains(Name,'Phone')&$orderby=Quantity desc&$top=10&$expand=User
```

### OData Configuration

OData is configured per-controller, allowing fine-grained control over queryable endpoints. See `Documents/Api.http` for comprehensive examples.

## Design Patterns Implemented

### Architectural Patterns
- **Clean Architecture** - Dependency inversion, clear boundaries
- **Service Layer Pattern** - Business logic encapsulation
- **Facade Pattern** - Interface segregation for Service and Data layers
- **Repository Pattern** - Data access abstraction
- **Unit of Work Pattern** - Transaction management

### SOLID Principles
- **Single Responsibility** - Each class has one reason to change
- **Open/Closed** - Open for extension, closed for modification
- **Liskov Substitution** - Interfaces are properly substitutable
- **Interface Segregation** - Clients don't depend on unused interfaces
- **Dependency Inversion** - Depend on abstractions, not concretions

### Additional Patterns
- **Value Object Pattern** - Price, Email, etc.
- **Domain-Driven Design** - Entities, value objects, aggregates
- **Factory Pattern** - Object creation (where applicable)
- **Strategy Pattern** - Validation strategies

## Technologies & Dependencies

### Core Framework
- **.NET 9** 
- **C# 13**
- **Entity Framework Core** - ORM and database access
- **PostgreSQL** - Primary database (via Npgsql)

### Libraries & Tools
- **OData** - Advanced querying capabilities
- **FluentValidation** - Input validation in Service layer
- **Mapster** - Object mapping (TODO: implementation pending)
- **Serilog** - Structured logging
- **xUnit** - Unit testing framework
- **.NET Aspire** (Optional) - Service orchestration and observability

### Development Tools
- **Docker** - Containerization
- **OpenAPI/Swagger** - API documentation
- **TypeScript Generator** - Client code generation

## Project Roadmap & TODO

### Completed
- ✅ Service Layer pattern implementation
- ✅ Facade pattern for interface segregation
- ✅ Repository pattern with AggregateRepository and ReadOnlyRepository
- ✅ Database migration from Guid to ulong
- ✅ OData integration
- ✅ FluentValidation integration

### In Progress / TODO
- [ ] **Result Pattern** - Replace exception-based error handling
- [ ] **ApiConfiguration** - Centralized dependency injection setup
- [ ] **Mapster Integration** - Complete object mapping setup
- [ ] **Comprehensive Testing** - Expand unit and integration test coverage
- [ ] **Authentication/Authorization** - Complete JWT implementation
- [ ] **Caching Strategy** - Implement distributed caching
- [ ] **API Versioning** - Support multiple API versions
- [ ] **Rate Limiting** - Protect endpoints from abuse

## Customization Checklist

- [ ] Update namespace from `RetailPortal` to your project name
- [ ] Define your domain entities in Model project
- [ ] Create service interfaces in ServiceFacade
- [ ] Implement business logic in Service project
- [ ] Configure database context and migrations
- [ ] Set up authentication/authorization strategy
- [ ] Implement logging and monitoring
- [ ] Configure dependency injection
- [ ] Add comprehensive validation rules
- [ ] Write unit and integration tests
- [ ] Configure OData endpoints for your entities

## Performance Considerations

### Database Optimization
- Use sequential ulong primary keys for better indexing
- Implement read/write database separation if needed
- Use AsNoTracking() for read-only queries (via ReadOnlyRepository)
- Leverage eager loading with Include() (via AggregateRepository)
- Optimize bulk operations for large datasets

### Application Performance
- Asynchronous programming throughout
- Implement caching strategies (in-memory, distributed)
- Use projection (Select) to return only needed data
- OData query optimization and limits
- Connection pooling and resource management

### Monitoring
- Structured logging with Serilog
- Performance metrics and tracing
- .NET Aspire integration for observability
- Database query profiling

## Security Recommendations

### Input Validation
- FluentValidation in Service layer
- OData query validation and limits
- SQL injection prevention via EF Core parameterization

### Authentication & Authorization
- JWT token-based authentication
- Role-based access control (RBAC)
- Principle of least privilege

### Configuration Management
- Environment-specific configurations
- Secret management (Azure Key Vault, etc.)
- Secure connection strings

### Error Handling
- Proper exception handling
- Avoid exposing sensitive information in errors
- Structured error responses

## Logging & Observability

### Logging Strategy
- **Structured logging** with Serilog
- **Correlation IDs** for request tracing
- **Log levels** appropriately configured per environment
- **Performance metrics** for critical operations

### Observability
- **.NET Aspire** - Built-in OpenTelemetry support
- **Distributed tracing** for microservices scenarios
- **Exception tracking** and alerting
- **Database query logging** in development

## Testing Strategy

### Unit Tests
- Service layer business logic
- Repository implementations
- Validation rules
- Value objects and entities

### Integration Tests (TODO)
- API endpoints
- Database operations
- OData query scenarios
- Authentication flows

### Performance Tests (TODO)
- Large dataset queries (1M products seeded)
- OData complex query performance
- Concurrent request handling

## Support & Resources

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [OData Documentation](https://learn.microsoft.com/en-us/odata/)
- [Clean Architecture Guide](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/)

## Acknowledgments

This template incorporates best practices from Clean Architecture, Domain-Driven Design, and modern .NET development patterns.
