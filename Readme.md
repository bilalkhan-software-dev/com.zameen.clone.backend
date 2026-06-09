# PropertyHub Backend – ASP.NET Core Web API

> **Internship Project** – A full‑featured real‑estate portal backend built with ASP.NET Core 10 (preview), Entity Framework Core, and JWT authentication.

## 📚 Features

- **User, Agent & Admin roles** with identity management
- **JWT authentication** with refresh token rotation
- **Google OAuth2** login (challenge flow)
- **Property CRUD** (create, read, update, delete, toggle active)
- **Advanced filtering** (city, price, area, bedrooms, etc.) with pagination & sorting
- **Agent approval** by admin
- **Enquiry system** (public can send, agents/admins can view)
- **Background service** for monthly price trend generation
- **Search log** tracking for trending locations
- **FluentValidation**, **AutoMapper**, **Serilog** logging, **Swagger** UI

## 🛠 Tech Stack

- ASP.NET Core 10 
- Entity Framework Core
- SQL Server 2025
- JWT Bearer & Google OAuth2
- FluentValidation, AutoMapper, Serilog
- Docker, Docker Compose

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- SQL Server (LocalDB, Express, or Docker container)

### 1. Clone the repository

```bash
git clone https://github.com/bilalkhan-software-dev/com.zameen.clone.backend.git
cd com.zameen.clone.backend
```

### 2. Set user secrets (for development)

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "YourSuperSecretKeyAtLeast32CharactersLong!"
dotnet user-secrets set "Authentication:Google:ClientId" "your-google-client-id"
dotnet user-secrets set "Authentication:Google:ClientSecret" "your-google-client-secret"
```

### 3. Configure the connection string

Edit `appsettings.Development.json` and set your SQL Server connection string under `ConnectionStrings:DefaultConnection`.

### 4. Apply database migrations

```bash
dotnet ef database update
```

### 5. Run the server

```bash
dotnet run
```

The API will be available at `http://localhost:5118` and Swagger at `http://localhost:5118/swagger`.

## 📂 Project Structure (simplified)

```
com.zameen/
├── Controllers/          # API endpoints
├── Data/                 # DbContext, migrations
├── Models/               # Entities, DTOs
├── Repositories/         # Data access layer
├── Services/             # Business logic
├── Middleware/            # Custom middleware (e.g. global exception handler)
├── Validators/           # FluentValidation validators
└── Program.cs            # App startup
```


## 📡 API Documentation

Once running, open `/swagger` for interactive API docs.

## 🤝 Contributing

Pull requests are welcome. For major changes, please open an issue first.