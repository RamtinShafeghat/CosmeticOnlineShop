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

### Cursor Desktop / Cloud Agent

This repo includes `.cursor/environment.json`, which installs the .NET SDK and puts `dotnet` on your PATH.

If a terminal still says `dotnet: command not found`, run either:

```bash
# one-time PATH fix for this session
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$HOME/.dotnet:$HOME/.dotnet/tools:$PATH"
sudo ln -sf "$HOME/.dotnet/dotnet" /usr/local/bin/dotnet
```

or use the helper script (installs .NET if needed):

```bash
./scripts/run-api.sh
```

### Install the .NET 8 SDK (local machine)

If you are running outside Cursor’s provisioned environment and `dotnet` is missing:

**macOS (Homebrew):**
```bash
brew install --cask dotnet-sdk
```

**Windows:** download the [.NET 8 SDK installer](https://dotnet.microsoft.com/download/dotnet/8.0), then open a new terminal.

**Linux:**
```bash
curl -fsSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0
echo 'export DOTNET_ROOT="$HOME/.dotnet"' >> ~/.bashrc
echo 'export PATH="$HOME/.dotnet:$HOME/.dotnet/tools:$PATH"' >> ~/.bashrc
source ~/.bashrc
sudo ln -sf "$HOME/.dotnet/dotnet" /usr/local/bin/dotnet
```

Verify with `dotnet --version` (expect `8.0.x`).

## Run locally

### 1. Start the API

```bash
./scripts/run-api.sh
# or:
cd backend/CosmeticShop.Api
dotnet run --launch-profile http
```

API base URL: `http://localhost:5041`  
Swagger: `http://localhost:5041/swagger`

### 2. Start the Angular storefront

You do **not** need a global `ng` command. Use `npm` from the `frontend` folder.

**Windows (Command Prompt / PowerShell):**
```bat
cd frontend
npm install
npm start
```

Or double-run / call:
```bat
scripts\run-web.cmd
```

**macOS / Linux:**
```bash
./scripts/run-web.sh
# or:
cd frontend
npm install
npm start
```

If you see `'ng' is not recognized`, you ran `ng` directly. Use `npm start` instead (it uses the local Angular CLI from `node_modules`).

Storefront: `http://localhost:4200`

The frontend calls `http://localhost:5041/api` (see `frontend/src/environments/environment.ts`). CORS is enabled for the Angular dev server.

### Opening the site from Cursor Desktop

The API and Angular app run **inside the Cursor cloud VM**, not on your laptop. Opening `http://localhost:4200` in Chrome on your computer will fail unless Cursor is forwarding that port.

Do one of these:

1. **Port forward (recommended)**  
   - In Cursor Desktop, open the cloud agent.  
   - Look for the **plug / Ports** control (often top-right).  
   - Confirm ports `4200` (frontend) and `5041` (API) are forwarded.  
   - Open the **forwarded local URL** Cursor shows (it may not be 4200 if that port was busy).

2. **Simple Browser inside Cursor**  
   - `Ctrl/Cmd+Shift+P` → **Simple Browser: Show**  
   - Enter `http://localhost:4200`

3. **Remote desktop**  
   - Take control of the agent desktop and open `http://localhost:4200` in the VM browser.

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
