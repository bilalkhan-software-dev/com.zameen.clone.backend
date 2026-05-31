# PropertyHub – Zameen.com Clone (Internship Project)

A full‑stack real‑estate portal built with **ASP.NET Core 10** (preview) and **Next.js 14 + MUI + TypeScript**.  
Features JWT authentication, Google OAuth2, role‑based access (User/Agent/Admin), advanced property search with pagination, and Docker support.

## ✨ Key Features

- User registration & login (JWT + refresh token rotation)
- Google OAuth2 login (challenge flow)
- Roles: User, Agent, Admin
- Property CRUD (create/edit/delete/toggle active)
- Advanced property filtering (city, price, bedrooms, area, etc.) with pagination & sorting
- Property images (URLs stored as JSON)
- Agent approval workflow (admin)
- Enquiry system (public send + agent/admin view)
- Admin user & agent management
- Serilog logging, FluentValidation, AutoMapper, Swagger

## 🏗 Tech Stack

| Layer    | Technology                                                     |
| -------- | -------------------------------------------------------------- |
| Backend  | ASP.NET Core 10 (preview), EF Core, Identity, JWT, Swashbuckle |
| Frontend | Next.js 14 (App Router), React 18, MUI 5, TypeScript, Axios    |
| Database | SQL Server 2022                                                |
| DevOps   | Docker, Docker Compose                                         |

---

## 🚀 Getting Started (Local Development without Docker)

### Prerequisites

- [.NET 10 SDK Preview](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Node.js 20+](https://nodejs.org/)
- SQL Server (LocalDB, Express, or Docker container)

### Backend Setup

```bash
# 1. Clone the repo
git clone https://github.com/bilalkhan-software-dev/com.zameen.clone.backend.git
cd com.zameen.clone.backend

# 2. Set secrets (development only)
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "YourSuperSecretKeyAtLeast32CharactersLong!"
dotnet user-secrets set "Authentication:Google:ClientId" "your-google-client-id"
dotnet user-secrets set "Authentication:Google:ClientSecret" "your-google-client-secret"

# 3. Update connection string (appsettings.Development.json) if needed
# 4. Apply migrations
dotnet ef database update

# 5. Run the API
dotnet run
```
