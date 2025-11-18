# CQRS Pattern Explained

## What is CQRS?

**CQRS** (Command Query Responsibility Segregation) is a design pattern that separates the read and write operations of an application into distinct models and handlers. The core principle is simple:

- **Commands** = Write operations (Create, Update, Delete) - they change state
- **Queries** = Read operations (Get, List, Search) - they only retrieve data

Think of it like a library: you have different processes for **borrowing books** (commands - changes the library's state) versus **browsing the catalog** (queries - just reading information).

## Why Use CQRS?

### Benefits:

1. **Separation of Concerns**: Read and write logic are completely separate, making code easier to understand and maintain
2. **Performance Optimization**: You can optimize read and write operations independently
   - Read models can be denormalized for fast queries
   - Write models can be normalized for data integrity
3. **Scalability**: Read and write operations can scale independently
4. **Flexibility**: You can use different data stores for reads (e.g., read replicas, caches) and writes
5. **Testability**: Commands and queries are isolated, making unit testing simpler

### When to Use CQRS:

- Applications with complex business logic
- High read/write ratio differences
- Need for independent scaling of read and write operations
- When read and write models have different requirements

## CQRS in Clean Architecture

In Clean Architecture, CQRS fits naturally:

- **Commands and Queries** are defined in the **Core layer** (use cases)
- **Handlers** implement the business logic in the **Core layer**
- **Repositories** (interfaces) are defined in **Core**, implementations in **Infrastructure**
- **Controllers** in the **API layer** orchestrate commands and queries

## Example Implementation

Let's implement CQRS for a User management system. This example shows how to structure commands, queries, and their handlers.

### 1. Base Interfaces (Core Layer)

First, we define the base interfaces for commands, queries, and their handlers:

```csharp
// CleanArch.Core/CQRS/ICommand.cs
namespace CleanArch.Core.CQRS;

/// <summary>
/// Marker interface for commands (write operations)
/// </summary>
public interface ICommand
{
}

/// <summary>
/// Handler interface for commands that return a result
/// </summary>
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handler interface for commands that don't return a result
/// </summary>
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
```

```csharp
// CleanArch.Core/CQRS/IQuery.cs
namespace CleanArch.Core.CQRS;

/// <summary>
/// Marker interface for queries (read operations)
/// </summary>
public interface IQuery<TResult>
{
}

/// <summary>
/// Handler interface for queries
/// </summary>
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
```

### 2. Example: User Commands (Write Operations)

```csharp
// CleanArch.Core/Commands/Users/CreateUserCommand.cs
namespace CleanArch.Core.Commands.Users;

public class CreateUserCommand : ICommand
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

// CleanArch.Core/Commands/Users/CreateUserCommandResult.cs
namespace CleanArch.Core.Commands.Users;

public class CreateUserCommandResult
{
    public required Guid UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

```csharp
// CleanArch.Core/Commands/Users/UpdateUserCommand.cs
namespace CleanArch.Core.Commands.Users;

public class UpdateUserCommand : ICommand
{
    public required Guid UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
}
```

### 3. Example: User Queries (Read Operations)

```csharp
// CleanArch.Core/Queries/Users/GetUserByIdQuery.cs
namespace CleanArch.Core.Queries.Users;

public class GetUserByIdQuery : IQuery<UserDto?>
{
    public required Guid UserId { get; set; }
}

// CleanArch.Core/Queries/Users/GetAllUsersQuery.cs
namespace CleanArch.Core.Queries.Users;

public class GetAllUsersQuery : IQuery<IReadOnlyList<UserDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// CleanArch.Core/Queries/Users/UserDto.cs
namespace CleanArch.Core.Queries.Users;

public class UserDto
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

### 4. Repository Interface (Core Layer)

```csharp
// CleanArch.Core/Interfaces/IUserRepository.cs
namespace CleanArch.Core.Interfaces;

using CleanArch.Core.Queries.Users;

public interface IUserRepository
{
    // Write operations
    Task<Guid> CreateAsync(CreateUserCommand command, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateUserCommand command, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
    
    // Read operations
    Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserDto>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
```

### 5. Command Handlers (Core Layer)

```csharp
// CleanArch.Core/Handlers/Commands/CreateUserCommandHandler.cs
namespace CleanArch.Core.Handlers.Commands;

using CleanArch.Core.Commands.Users;
using CleanArch.Core.CQRS;
using CleanArch.Core.Interfaces;
using Microsoft.Extensions.Logging;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<CreateUserCommandResult> HandleAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating user with username: {Username}", command.Username);

        // Business logic validation
        if (string.IsNullOrWhiteSpace(command.Username))
        {
            throw new ArgumentException("Username is required", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.Email))
        {
            throw new ArgumentException("Email is required", nameof(command));
        }

        // Execute the command
        var userId = await _userRepository.CreateAsync(command, cancellationToken);

        _logger.LogInformation("User created successfully with ID: {UserId}", userId);

        return new CreateUserCommandResult
        {
            UserId = userId,
            Username = command.Username,
            Email = command.Email,
            CreatedAt = DateTime.UtcNow
        };
    }
}
```

```csharp
// CleanArch.Core/Handlers/Commands/UpdateUserCommandHandler.cs
namespace CleanArch.Core.Handlers.Commands;

using CleanArch.Core.Commands.Users;
using CleanArch.Core.CQRS;
using CleanArch.Core.Interfaces;
using Microsoft.Extensions.Logging;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task HandleAsync(
        UpdateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating user with ID: {UserId}", command.UserId);

        // Verify user exists
        var existingUser = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (existingUser == null)
        {
            throw new InvalidOperationException($"User with ID {command.UserId} not found");
        }

        // Execute the command
        await _userRepository.UpdateAsync(command, cancellationToken);

        _logger.LogInformation("User updated successfully with ID: {UserId}", command.UserId);
    }
}
```

### 6. Query Handlers (Core Layer)

```csharp
// CleanArch.Core/Handlers/Queries/GetUserByIdQueryHandler.cs
namespace CleanArch.Core.Handlers.Queries;

using CleanArch.Core.CQRS;
using CleanArch.Core.Interfaces;
using CleanArch.Core.Queries.Users;
using Microsoft.Extensions.Logging;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserDto?> HandleAsync(
        GetUserByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving user with ID: {UserId}", query.UserId);

        var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found", query.UserId);
        }

        return user;
    }
}
```

```csharp
// CleanArch.Core/Handlers/Queries/GetAllUsersQueryHandler.cs
namespace CleanArch.Core.Handlers.Queries;

using CleanArch.Core.CQRS;
using CleanArch.Core.Interfaces;
using CleanArch.Core.Queries.Users;
using Microsoft.Extensions.Logging;

public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAllUsersQueryHandler> _logger;

    public GetAllUsersQueryHandler(
        IUserRepository userRepository,
        ILogger<GetAllUsersQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<UserDto>> HandleAsync(
        GetAllUsersQuery query,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving users - Page: {PageNumber}, Size: {PageSize}",
            query.PageNumber,
            query.PageSize);

        var users = await _userRepository.GetAllAsync(
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        _logger.LogInformation("Retrieved {Count} users", users.Count);

        return users;
    }
}
```

### 7. Using CQRS in Controllers (API Layer)

```csharp
// CleanArch.API/Controllers/UsersController.cs
namespace CleanArch.API.Controllers;

using CleanArch.Core.Commands.Users;
using CleanArch.Core.CQRS;
using CleanArch.Core.Queries.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ICommandHandler<CreateUserCommand, CreateUserCommandResult> _createUserHandler;
    private readonly ICommandHandler<UpdateUserCommand> _updateUserHandler;
    private readonly IQueryHandler<GetUserByIdQuery, UserDto?> _getUserByIdHandler;
    private readonly IQueryHandler<GetAllUsersQuery, IReadOnlyList<UserDto>> _getAllUsersHandler;

    public UsersController(
        ICommandHandler<CreateUserCommand, CreateUserCommandResult> createUserHandler,
        ICommandHandler<UpdateUserCommand> updateUserHandler,
        IQueryHandler<GetUserByIdQuery, UserDto?> getUserByIdHandler,
        IQueryHandler<GetAllUsersQuery, IReadOnlyList<UserDto>> getAllUsersHandler)
    {
        _createUserHandler = createUserHandler;
        _updateUserHandler = updateUserHandler;
        _getUserByIdHandler = getUserByIdHandler;
        _getAllUsersHandler = getAllUsersHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var result = await _createUserHandler.HandleAsync(command);
        return CreatedAtAction(nameof(GetUserById), new { id = result.UserId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        command.UserId = id;
        await _updateUserHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var query = new GetUserByIdQuery { UserId = id };
        var user = await _getUserByIdHandler.HandleAsync(query);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetAllUsersQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var users = await _getAllUsersHandler.HandleAsync(query);
        return Ok(users);
    }
}
```

### 8. Dependency Injection Setup

```csharp
// In CleanArch.API/Api.cs or Program.cs
// Add this to ConfigureServices method:

protected void AddCQRS()
{
    // Register command handlers
    Builder.Services.AddScoped<ICommandHandler<CreateUserCommand, CreateUserCommandResult>, CreateUserCommandHandler>();
    Builder.Services.AddScoped<ICommandHandler<UpdateUserCommand>, UpdateUserCommandHandler>();
    
    // Register query handlers
    Builder.Services.AddScoped<IQueryHandler<GetUserByIdQuery, UserDto?>, GetUserByIdQueryHandler>();
    Builder.Services.AddScoped<IQueryHandler<GetAllUsersQuery, IReadOnlyList<UserDto>>, GetAllUsersQueryHandler>();
    
    // Register repository (implementation would be in Infrastructure layer)
    // Builder.Services.AddScoped<IUserRepository, UserRepository>();
}
```

## Key Takeaways

1. **Commands** represent intent to change state - they're verbs (CreateUser, UpdateUser)
2. **Queries** represent requests for data - they're nouns (GetUserById, GetAllUsers)
3. **Handlers** contain the business logic for executing commands/queries
4. **Separation** allows independent optimization and scaling
5. **Clean Architecture** keeps commands/queries in Core, implementations in Infrastructure

## Next Steps

To complete this implementation, you would:

1. Create the `IUserRepository` implementation in an Infrastructure project (e.g., `CleanArch.Infrastructure.Sql`)
2. Add validation using FluentValidation or similar
3. Add unit tests for handlers
4. Consider adding a mediator pattern (like MediatR) to simplify handler registration and execution
5. Add caching for frequently accessed queries
6. Consider event sourcing for audit trails
