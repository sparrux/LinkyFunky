# LinkyFunky

## Overview

This project is a URL shortening service developed as a pet project, designed to simulate a production-like, high-load backend system.

The main goal of the project is to implement a scalable and efficient service for shortening user-provided URLs while meeting strict requirements for latency, concurrency, and system reliability defined in the accompanying PRD (Product Requirements Document).

## Architecture

The project follows a DDD-inspired Clean Architecture approach based on principles described in _Clean Architecture_ (Robert C. Martin) and widely adopted in the .NET ecosystem, including Microsoft architectural guidelines. The system is structured into Domain, Application, Infrastructure, and API layers to enforce separation of concerns and keep business logic independent from external frameworks.

At the same time, it is important to note that this level of architectural decomposition is somewhat excessive for a URL shortening service. DDD in this context introduces additional complexity that is not strictly required for the domain, but was intentionally chosen to demonstrate the ability to design and implement enterprise-style architecture.

The project also uses CQRS to separate command and query flows. While this pattern is also an overengineering decision for the current scope, it was applied to showcase structured request handling, clear separation of responsibilities, and familiarity with scalable backend design patterns.

## Technology Choices

### API & Architecture Stack

The project is built on .NET 10 with ASP.NET Core as the core web framework. EF Core is used for data access, with PostgreSQL as the primary relational database and Redis as a caching layer to reduce latency and handle higher load scenarios. This is a standard modern backend stack for building scalable and maintainable API-driven systems.

### Testing Strategy

Testing is implemented using xUnit as the main testing framework, combined with Testcontainers for running integration tests against real infrastructure dependencies. The development approach follows TDD principles, ensuring that both domain logic and application workflows are verified early and consistently during implementation.

### Observability

For system observability, the project integrates Prometheus for metrics collection and Grafana for visualization and dashboards. This setup provides insights into performance, latency, and system health, which is especially important for evaluating behavior under high-load conditions and ensuring production-readiness.

## Run locally

For local development, .NET Aspire is used as the orchestration layer. It allows all system components (API, database, cache, and supporting services) to be started with a single command, significantly simplifying setup and enabling a “one-click” runnable development environment.

1. Clone repository
```bash
git clone https://github.com/sparrux/LinkyFunky.git
```

2. Go to root
```bash
cd LinkyFunky
```

3. Run `Aspire`
```bash
dotnet run --project src/AppHost/AppHost.csproj
```

The browser will automatically open at startup on the Aspire Dashboard page, where you can go to the Grafana Dashboard with metrics, see the results of the load tests and information about the connected services: PostgreSQL and Redis.

## CI/CD

The project follows a Git Flow branching strategy, where `master` represents the production-ready branch and `develop` is used as the main integration branch for ongoing development.

All feature branches are merged into `develop` via pull requests. On every push or pull request to the `develop` branch, a CI pipeline is triggered in GitHub Actions that runs unit tests, integration tests, and a lightweight load test to validate system behavior under basic stress conditions. This helps ensure that both functional correctness and performance stability are maintained throughout development.

The `master` branch is reserved for stable production-ready states, with releases intended to be promoted from `develop` after successful validation. Production deployment pipeline and VPS configuration are planned as a future improvement.
