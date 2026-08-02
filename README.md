# Velora — Cosmetic Online Shop

An online cosmetics shop with an **ASP.NET Core** Web API backend and an **Angular** frontend.

## Stack

| Layer | Technology |
| --- | --- |
| Backend | C# / ASP.NET Core 8, Entity Framework Core, SQLite |
| Frontend | Angular 19 (standalone components, signals) |
| API docs | Swagger UI (`/swagger`) |

## Features

- Product catalog with categories, search, and featured items
- Product detail pages
- Client-side shopping bag (persisted in `localStorage`)
- Checkout and order creation with stock checks
- Seeded Velora sample catalog (skincare, makeup, fragrance, body care)

## Project structure

```
backend/
  CosmeticShop.sln
  CosmeticShop.Api/          # ASP.NET Core Web API
frontend/                    # Angular storefront
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Node.js 20+ and npm

### Install the .NET 8 SDK

If `dotnet` says **command not found**, install the SDK first, then **restart the Cursor terminal** (or Cursor itself).

**macOS (Homebrew):**
```bash
brew install --cask dotnet-sdk
```

**Windows:**
1. Download the [.NET 8 SDK installer](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Run it, then open a **new** terminal and run `dotnet --version`

**Linux (Ubuntu/Debian):**
```bash
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
bash dotnet-install.sh --channel 8.0
echo 'export DOTNET_ROOT="$HOME/.dotnet"' >> ~/.bashrc
echo 'export PATH="$PATH:$HOME/.dotnet:$HOME/.dotnet/tools"' >> ~/.bashrc
source ~/.bashrc
```

Verify:
```bash
dotnet --version
```
You should see something like `8.0.x`.

## Run locally

### 1. Start the API

```bash
cd backend/CosmeticShop.Api
dotnet run --launch-profile http
```

API base URL: `http://localhost:5041`  
Swagger: `http://localhost:5041/swagger`

### 2. Start the Angular storefront

```bash
cd frontend
npm install
npm start
```

Storefront: `http://localhost:4200`

The frontend calls `http://localhost:5041/api` (see `frontend/src/environments/environment.ts`). CORS is enabled for the Angular dev server.

## API overview

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/api/categories` | List categories |
| `GET` | `/api/categories/{slug}` | Category by slug |
| `GET` | `/api/products` | List products (`categoryId`, `search`, `featured`) |
| `GET` | `/api/products/{id}` | Product by id |
| `GET` | `/api/products/slug/{slug}` | Product by slug |
| `POST` | `/api/orders` | Place an order |
| `GET` | `/api/orders/{id}` | Get order details |

## Notes

- SQLite database file `cosmeticshop.db` is created automatically on first run.
- Free shipping applies to orders of $75 or more; otherwise shipping is $6.95.
