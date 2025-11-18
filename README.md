This is an example project for a clean architecture application. It is meant to demo how to set up various things following the clean architecture principles.

The project is split into two parts:

- [backend](backend): C# backend for the application
- [frontend](frontend): SolidJs based frontend

Each part has its own README.md file with more details.

# Local Development

To run the project locally, you need to have Docker installed.

```shell
docker compose up
```

This will start the Keycloak server and the Aspire dashboard.

Follow the instructions in the [docs/keycloak-setup.md](docs/keycloak-setup.md) file to set up the Keycloak server.

TODO:
- [ ] Implement CQRS pattern for the backend
- [ ] Add SQL Server database for the backend
- [ ] Instrument specific classes and methods for Open Telemetry
  - Can I create an attribute that can be used to instrument a class or method?
