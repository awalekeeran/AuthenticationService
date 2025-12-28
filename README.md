# Authentication Service
A modern, production-ready authentication service implementing **OAuth 2.0** and **OpenID Connect (OIDC)** using **Duende IdentityServer**, **.NET 8**, **Angular**, **JWT**, and **Redis**.

## 🏗️ Architecture

```
┌─────────────────────┐
│   Angular SPA       │  ← Client Application
│   (Port 4200)       │
└──────────┬──────────┘
           │ OIDC Flow
┌──────────▼──────────┐
│  IdentityServer     │  ← OAuth/OIDC Provider
│   (Port 5001)       │     (To be implemented)
└──────────┬──────────┘
           │ JWT Validation
┌──────────▼──────────┐
│   WebAPI            │  ← Resource Server
│   (Port 5000)       │     (.NET 8)
└──────────┬──────────┘
           │
     ┌─────┴─────┐
     │           │
┌────▼───┐  ┌───▼────┐
│SQL     │  │ Redis  │
│Server  │  │ Cache  │
└────────┘  └────────┘
```

## 🛠️ Technology Stack

### Backend
- **.NET 8.0** (LTS) - To be upgraded from .NET 6
- **Duende IdentityServer 7** - OAuth 2.0 / OIDC provider (To be added)
- **Entity Framework Core 8** - ORM
- **SQL Server** - Database
- **Redis 7** - Distributed caching (To be added)
- **BCrypt.NET** - Password hashing (To be added)

### Frontend
- **Angular 14** - To be upgraded to Angular 17+
- **TypeScript 4.7** - To be upgraded
- **angular-oauth2-oidc** - OIDC client library (To be added)

### Authentication & Security
- **OAuth 2.0** - Authorization framework
- **OpenID Connect** - Authentication layer
- **JWT (JSON Web Tokens)** - Token format
- **PKCE** - Enhanced security for SPAs

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK (to be installed)
- Node.js 18+ and npm
- SQL Server (LocalDB or Express)
- Docker (for Redis)
- Git

### Current Setup (Temporary)

```powershell
# Clone repository
git clone https://github.com/awalekeeran/AuthenticationService.git
cd AuthenticationService

# Backend
cd WebAPI
dotnet restore
dotnet run

# Frontend
cd ../RequestSystem.UI.Internal
npm install
npm start
```

## 🎓 Learning Objectives

This project serves as a learning platform for:

### 1. OAuth 2.0 & OpenID Connect
- Understanding OAuth flows (Authorization Code, Client Credentials)
- PKCE for single-page applications
- Token types (Access, ID, Refresh)
- Token lifecycle management

### 2. IdentityServer
- Setting up Duende IdentityServer
- Configuring clients, scopes, and resources
- Custom user stores
- Claims-based authentication

### 3. Security Best Practices
- Password hashing (BCrypt)
- Secure secret management
- HTTPS enforcement
- CORS configuration
- Rate limiting

### 4. Modern Architecture
- Distributed caching with Redis
- Horizontal scalability
- Microservices patterns
- API gateway patterns

### 5. .NET 8 & Angular
- Latest framework features
- Performance optimizations
- Modern development patterns

## 📄 License

[Add your license here]

## 🙏 Acknowledgments

- Duende Software for IdentityServer
- Microsoft for .NET and Entity Framework
- Angular team for the framework
- Redis Labs for Redis
