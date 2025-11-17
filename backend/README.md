C# backend for the application



# Authentication

# Logging


# Unit Testing

The project uses NUnit for unit testing. It also leverages several libraries to streamline the testing process.
- Fixture: For creating test data fixtures and builders
- FluentAssertions: For writing readable and expressive test assertions
- Moq: For creating mock objects in unit tests
- Moq.AutoMocker: For automatic dependency injection and mocking in test setup
- NUnit: For the test framework and test runner

A unit testing library has been created to streamline the setup of the test environment. It is located in the `CleanArch.UnitTests` project.

All unit tests should inherit from the `UnitTestBase<T>` class.
```csharp
public class UserServiceTests : UnitTestBase<UserService>
{
    [Test]
    public void GetUserById_ShouldReturnUser()
    {
        // Arrange
        // Act
        // Assert
    }
}
```

# Open Telemetry

The project uses Open Telemetry to collect traces, metrics, and logs.

For local development, you can use the Aspire dashboard to view the telemetry data. This is included in the docker-compose.yml file.

# Prep

Install the following tools:

```powershell
dotnet tool install --global dotnet-reportgenerator-globaltool 
```

# Run the project

From the `backend` directory, run the following command:
```shell
docker compose up
```

This will start the project and the Aspire dashboard.


# Unit Tests

## Code Coverage

From the `backend` directory, run the following command:
```powershell
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"./**/TestResults/**/coverage.cobertura.xml" -targetdir:"./coveragereport"
```
