.PHONY: help build test run coverage licenses outdated vulnerable clean

# Default target
help:
	@echo "Available commands:"
	@echo "  make build          - Build the solution"
	@echo "  make test           - Run unit tests"
	@echo "  make run            - Run the API project"
	@echo "  make coverage       - Run tests with code coverage report"
	@echo "  make licenses       - Check package licenses"
	@echo "  make outdated       - List outdated packages"
	@echo "  make vulnerable     - List vulnerable packages"
	@echo "  make clean          - Clean build artifacts"

# Build commands
build:
	dotnet build CleanArch.sln

# Test commands
test:
	dotnet test CleanArch.sln

# Run commands
run:
	dotnet run --project CleanArch.API/CleanArch.API.csproj

# Code coverage
coverage:
	dotnet test CleanArch.sln --collect:"XPlat Code Coverage"
	reportgenerator -reports:"./**/TestResults/**/coverage.cobertura.xml" -targetdir:"./coveragereport"

# Package management
licenses:
	nuget-license -i ./CleanArch.sln -a ./allowed-licenses.json -o JsonPretty -err

outdated:
	dotnet list package --outdated

vulnerable:
	dotnet list package --vulnerable

# Clean commands
clean:
	dotnet clean CleanArch.sln

