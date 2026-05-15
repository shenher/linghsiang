# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

LING HSIANG BAKERY — a static brand image website with no database and no contact form. The Contact page shows store info and social media QR codes. `EmailService` exists as a generic SMTP utility but is not currently wired to any page action.

## Commands

All commands run from the repo root. The solution and project files are under `OfficialWeb/`.

```bash
# Restore dependencies
dotnet restore OfficialWeb/OfficialWeb.csproj

# Build
dotnet build OfficialWeb/OfficialWeb.csproj

# Run (HTTP, development)
dotnet run --project OfficialWeb/OfficialWeb.csproj --urls="http://localhost:5227"

# Run (HTTPS, development)
dotnet run --project OfficialWeb/OfficialWeb.csproj --urls="https://localhost:7097"

# Docker (production)
docker-compose up --build
```

There are no automated tests in this project.

## Architecture

- **Framework**: ASP.NET Core 8 MVC, .NET SDK 8.0.300 (`OfficialWeb/global.json`)
- **Single controller**: `HomeController.cs` handles all pages — `Index`, `Products`, `Contact` (GET only); no form submission logic
- **Services**: `Tools/EmailService.cs` — implements `IEmailService` as a generic SMTP utility with `SendAsync(subject, body)`; reads SMTP config from `appsettings.json` → `EmailSettings` (`SmtpHost`, `SmtpPort`, `SmtpUser`, `SmtpPassword`/`SMTP_PASSWORD` env var, `FromAddress`, `FromDisplayName`, `Recipients` semicolon-separated); registered in DI via `Program.cs` but not currently injected into any controller
- **Models**: `ErrorViewModel` only (`ContactViewModel` has been removed)
- **Views**: Razor (`.cshtml`) under `Views/Home/` and `Views/Shared/`; `_Layout.cshtml` is the master layout; `_ViewImports.cshtml` declares global usings and Tag Helpers; `_ValidationScriptsPartial.cshtml` renders client-side validation scripts
- **Static assets**: `wwwroot/` — `css/site.css` contains all theme styles organized in 9 sections; `_Layout.cshtml.css` is layout-scoped; `wwwroot/lib/` contains local copies of Bootstrap 5.3.2, jQuery, and jquery-validation (used by validation partial; layout loads Bootstrap via CDN)

### HTTPS & Certificate

In non-Development environments, the app loads a PFX certificate. Password is resolved in this priority:

1. `appsettings.json` → `CertificatePassword`
2. Environment variable `CERT_PASSWORD`
3. Exception thrown

In Docker, mount certs into `/app/certs/` and set `CERT_PASSWORD` in `docker-compose.yml` or a `.env` file.

### Security middleware (Program.cs)

`X-Content-Type-Options`, `X-Frame-Options`, `X-XSS-Protection`, `Referrer-Policy`, `Permissions-Policy` headers are added manually. Global `AutoValidateAntiforgeryToken` is configured — do **not** add `[ValidateAntiForgeryToken]` on individual actions.

## Coding Conventions

These apply to all MVC code in this project (sourced from `.github/instructions/dotnetCodingGuidelines.instructions.md`):

- **Tag Helpers over Html Helpers** — always use `asp-for`, `asp-action`, `asp-controller`, `asp-validation-for`, etc.
- **Strong-typed ViewModels required** — no `ViewBag` or `ViewData` for passing data to views
- **Data Annotations for validation** — place validation attributes on ViewModel properties; pair with `asp-validation-for` spans in views
- **Single responsibility per action** — keep action methods focused; extract logic to private methods or services if needed
- **Anti-forgery**: global setup is already in place; do not add per-action attributes
- Controller actions that accept form data must use the corresponding ViewModel as the parameter type
