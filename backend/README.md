C# backend for the application



# Authentication

# Logging


# Unit Testing

The project uses NUnit for unit testing. It also leverages several libraries to streamline the testing process.
- Fixture: For creating test data fixtures and builders
- FluentAssertions: For writing readable and expressive test assertions
  - Version 7.2.0 is the last version before the licensing change that requires a license for commercial use.
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

```shell
dotnet tool install --global dotnet-reportgenerator-globaltool
dotnet tool install --global nuget-license 
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

# Package Management

**tl;dr** Use AI to update the packages and check the licenses:
```
use `dotnet list package --outdated` to find and updated my outdated packages. Use `nuget-license -i ./CleanArch.sln -a ./allowed-licenses.json -o JsonPretty -err` to check for any packages that use a license that is not allowed. If any violate the license, find the last version that does not violate the license and update the package to that version. If no suitable version is found, notify the user.
```

## Find outdated packages

```shell
dotnet list package --outdated
```

or use AI to update them for you... `use `dotnet list package --outdated` to find and updated my outdated packages except FluentAssertions`

## View license information

Allowed licenses are stored in the [allowed-licenses.json](./allowed-licenses.json) file. It uses [nuget-license](https://github.com/sensslen/nuget-license) to check the licenses of the packages.

View all license information for all packages in the solution.
```sh
nuget-license -i ./CleanArch.sln -a ./allowed-licenses.json -o JsonPretty
```

Or, just list the packages that use a license that is not allowed.
```sh
nuget-license -i ./CleanArch.sln -a ./allowed-licenses.json -o JsonPretty -err
```
