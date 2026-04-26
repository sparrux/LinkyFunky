# PRD 001 - LinkyFunky

## Description

Creating a service for shortening user URLs as part of a pet project. The project focuses on developing a high-load system with the latency, PRD, and concurrency requirements specified in this PRD.
After implementing the requirements in this PRD, the project should run with a single command and without errors.

## Functional requirements

### Backend

1. The client sends his link and gets back a short one
2. Users can follow a short link and be redirected to the user's path
3. Each click on a link is tracked by a counter
4. As part of the pet project, we use anonymous authorization on the server to skip the stage of implementing the login system. It will creates an anonymous account is the system

## Stack

**Backend:** `C#14`, `ASP.NET Core`, `EF Core`, `MediatR`, `FluentResults`, `Aspire`, `Scalar`.

## Non-functional requirements (NFR)

- Limit rate redirects per user 50/day
- Limit rate links creations per user 10/day
- Active links creators = 1_000_000
- Active users clicking on links = 10_000_000
- RPS create link = 1_000_000 * 10 / 86_400 = 116/sec * 5 = ~600 RPS peak create
- RPS redirect link = 10_000_000 * 50 / 86_400 = 579/sec * 5 = ~3000 RPS peak update
- Create link latency p95=100ms p50=25ms, concurrency = 0.1 * 600 = 60 concurrent requests
- Redirect link latency p95=15ms p50=5ms = 0.015 * 3000 = 45 concurrent requests
- Total concurrency 105

## Security

### Authentication

Client will automatically authenticated in the system by anonymously cookie scheme after shortcut request. It will creates an account and attach an authentication cookie to client browser for identification. As part of this pet project, I will not be implementing full-fledged authentication based on OAuth 2.0 or external ID services.

The authentication cookie will contain the user ID.

User authentication is also required to configure rate limits with the previously specified request restrictions.

## Endpoints

### Create user's shortcut link

#### Request

```http
POST /shortcut HTTP/1.1
```

| Key       | Value                           |
| --------- | ------------------------------- |
| long_link | User's long link for shortening |

#### Response

```http
HTTP/1.1 201
```

| Key           | Value              |
| ------------- | ------------------ |
| shortcut_path | Shortened full URL |

### Redirect by user's shortcut

#### Request

```http
GET /:code HTTP/1.1
```
#### Response

```http
HTTP/1.1 302
Location: short_url
```

## Data models

### Glossary & Ubiquitous Language

| Term     | Definition     |
| -------- | -------------- |
| Shortcut | Shortened link |

### Logical data models

#### Shortcut

| Name       | Type   |
| ---------- | ------ |
| Id         | Guid   |
| LongString | String |
| ShortCode  | String |
| UserId     | Guid   |

`ShortCode` will be indexed.

#### User

| Name | Type |
| ---- | ---- |
| Id   | Guid |

## Testing

### Unit tests

**Tools:** xUnit

### Integration tests

**Tools:** Testcontainers

### Performance tests

**Tools:** k6

## Deployment

#### CI/CD

- Build & Unit Test
- Integration Test (Testcontainers)
- Build Docker Image
- Manual Approval → Production Deploy

**Tools:** GitHub Actions
