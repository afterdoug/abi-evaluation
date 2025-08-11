# Ambev Developer Evaluation Backend

This repository contains the backend implementation for the Ambev Developer Evaluation project, built using .NET Core and following Domain-Driven Design (DDD) principles.

## Prerequisites

To run this project in a development environment, you'll need:

- [Docker](https://www.docker.com/get-started) (version 20.10.0 or higher)
- [Docker Compose](https://docs.docker.com/compose/install/) (version 1.29.0 or higher)
- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or higher) - for local development outside Docker

## Getting Started

### Running with Docker Compose

1. Clone this repository
2. Navigate to the backend directory
3. Run the following command to start the application:

```bash
docker-compose up -d
```

This will start all the necessary services defined in the docker-compose file, including the API and any required databases.

### Updating the Database

To apply migrations and update the database:

1. Ensure the Docker containers are running
2. Execute the following command:

```bash
# If running inside Docker
docker-compose exec ambev.developerevaluation.webapi dotnet ef database update

# If running locally
dotnet ef database update --project src/Ambev.DeveloperEvaluation.Infrastructure --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

## API Documentation

The API provides several endpoints for managing different resources:

- [Sales API Documentation](../../.doc/sales-api.md)
- [Users API Documentation](../../.doc/users-api.md)

## Project Architecture

This project follows Domain-Driven Design (DDD) principles and Clean Architecture patterns to create a maintainable and scalable application.

### Architecture Overview

The solution is organized into the following layers:

1. **Domain Layer** - Contains the core business logic, entities, and business rules
2. **Application Layer** - Orchestrates the flow of data between the domain and infrastructure layers
3. **Infrastructure Layer** - Provides implementations for external services, databases, etc.
4. **Web API Layer** - Exposes the application functionality through REST endpoints

### Domain Layer

The domain layer is the core of the application and contains:

- **Entities**: Core business objects (e.g., Sale, SaleItem)
- **Value Objects**: Immutable objects that represent concepts with no identity
- **Domain Services**: Services that operate on multiple entities
- **Repository Interfaces**: Contracts for data access

### Application Layer

The application layer contains:

- **Commands/Queries**: Following CQRS pattern (e.g., `CreateSaleCommand`, `GetSaleCommand`)
- **Command/Query Handlers**: Process commands and queries (e.g., `UpdateSaleHandler`)
- **DTOs**: Data Transfer Objects for communication between layers
- **Validators**: Validation logic for commands and queries

### Infrastructure Layer

The infrastructure layer provides implementations for:

- **Repositories**: Data access implementations
- **External Services**: Integration with external systems
- **Database Context**: Entity Framework Core DbContext
- **Migrations**: Database schema migrations

### Web API Layer

The Web API layer exposes the functionality through REST endpoints and includes:

- **Controllers**: API endpoints (e.g., `SalesController`)
- **Request/Response Models**: Models for API communication
- **Filters**: Cross-cutting concerns like authentication, logging
- **Middleware**: Request processing pipeline components

## Sales Module

The Sales module is a core part of the application and demonstrates the DDD approach:

### Domain Model

The Sale entity represents a sales transaction and includes:

- Sale information (ID, number, date, customer, branch)
- Collection of sale items
- Status information (isCancelled)
- Audit information (createdAt, updatedAt)

### Key Operations

The Sales API supports the following operations:

1. **Create Sale**: Creates a new sale with items
2. **Get Sale**: Retrieves a specific sale by ID
3. **Update Sale**: Updates an existing sale
4. **Cancel Sale**: Marks a sale as cancelled
5. **List Sales**: Retrieves a paginated list of sales

### CQRS Implementation

The Sales module uses the Command Query Responsibility Segregation (CQRS) pattern:

- **Commands**: `CreateSaleCommand`, `UpdateSaleCommand`, `CancelSaleCommand`
- **Queries**: `GetSaleCommand`
- **Handlers**: `CreateSaleHandler`, `UpdateSaleHandler`, `CancelSaleHandler`, `GetSaleHandler`

This separation allows for different optimization strategies for read and write operations.

### Validation

Request validation is implemented using FluentValidation:

- Validates sale data before processing (e.g., sale number format, date validity)
- Ensures business rules are enforced (e.g., preventing updates to cancelled sales)

## Testing

The project includes unit tests for:

- Domain entities and business rules
- Application layer command/query handlers
- API controllers

Test data generators are available in the `TestData` classes to facilitate testing:

- `SaleTestData`: Generates test data for Sale entities
- `GetSaleHandlerTestData`: Generates test data for GetSale operations

## Security

The application implements security through:

- User authentication and authorization
- Role-based access control (defined in `IUser` interface)
- Input validation to prevent injection attacks

## Contributing

Please follow these guidelines when contributing to the project:

1. Create a feature branch from `develop`
2. Follow the existing code style and patterns
3. Write unit tests for new functionality
4. Submit a pull request for review

## License

[License information would go here]