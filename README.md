# EmployeeSys

EmployeeSys is an ASP.NET Core 8 Web API for employee management with JWT authentication, refresh tokens, role-based authorization, permissions, and audit logging.

## Tech Stack

- ASP.NET Core 8 Web API
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- JWT Bearer Authentication
- Swagger / OpenAPI

## Solution Structure

- `EmployeeSys/` - API project and controllers
- `EmployeeSys.BLL/` - business logic and services
- `EmployeeSys.DAL/` - EF Core context, repositories, migrations, and seeding
- `EmployeeSys.Models/` - entities and DTOs

## Features

- Employee CRUD operations
- Filtering employees
- Role management
- User and role permission assignment
- JWT access tokens
- Refresh token rotation and revocation
- Audit log retrieval for admins

## Authentication

The API uses:

- Access token for authenticated requests
- Refresh token for issuing new access tokens without logging in again

Current auth endpoints:

- `POST /api/Auth/login`
- `POST /api/Auth/refresh`
- `POST /api/Auth/revoke`

The login response returns:

- `token`
- `refreshToken`
- `expiration`
- `tokenType`
- `username`
- `email`
- `roles`

## Default Seeded Admin

On startup, the app seeds roles and a default admin user if they do not already exist.

- Email: `Abdallah@gmail.com`
- Password: `Abdallah123`

Change these values in [Seed.cs](/D:/Courses/Work/EmployeeSys/EmployeeSys.DAL/Seed.cs) before using this project in a real environment.

## Configuration

Main configuration lives in [appsettings.json](/D:/Courses/Work/EmployeeSys/EmployeeSys/appsettings.json).

Important settings:

- `ConnectionStrings:DefaultConnection`
- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:DurationInMinutes`
- `Jwt:RefreshTokenDurationInDays`

## Getting Started

### 1. Restore dependencies

```powershell
dotnet restore
```

### 2. Update the database

Apply the existing migrations before running the API:

```powershell
dotnet ef database update --project .\EmployeeSys.DAL\EmployeeSys.DAL.csproj --startup-project .\EmployeeSys\EmployeeSys.csproj
```

### 3. Run the API

```powershell
dotnet run --project .\EmployeeSys\EmployeeSys.csproj
```

By default in development, Swagger opens at:

- `https://localhost:7191/swagger`
- `http://localhost:5075/swagger`

## API Overview

### Auth

- `POST /api/Auth/login`
- `POST /api/Auth/refresh`
- `POST /api/Auth/revoke`

### Employees

- `POST /api/Employees/add` - admin only
- `PUT /api/Employees/update/{id}` - admin only
- `GET /api/Employees/filter` - user role required
- `GET /api/Employees/{id}` - user role required
- `DELETE /api/Employees/{id}` - admin only

### Roles

- `POST /api/Role/add-role` - admin only
- `POST /api/Role/remove-role` - admin only

### Permissions

- `POST /api/Permission/assign-role-permissions` - admin only
- `POST /api/Permission/assign-user-permissions` - admin only
- `GET /api/Permission/my-permissions` - authenticated user

### Audit Logs

- `GET /api/AuditLogs` - admin only

## Authorization Notes

- `AdminOnly` policy requires the `Admin` role.
- Some employee endpoints require the `User` role.
- JWT tokens include the user id claim, role claims, and email claim.

## Development Notes

- `bin` and `obj` folders are ignored by Git.
- A refresh-token migration has been added and should be applied to the database.
- Swagger is configured with Bearer authentication support.

## Example Login Request

```json
{
  "username": "Abdallah@gmail.com",
  "password": "Abdallah123"
}
```

## Example Refresh Request

```json
{
  "refreshToken": "your-refresh-token"
}
```
