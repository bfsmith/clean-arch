C# backend for the application

# Prep

Install the following tools:

```powershell
dotnet tool install --global dotnet-reportgenerator-globaltool 
```

# Unit Tests

## Code Coverage

From the `backend` directory, run the following command:
```powershell
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"./**/TestResults/**/coverage.cobertura.xml" -targetdir:"./coveragereport"
```
