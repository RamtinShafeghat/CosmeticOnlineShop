# Velora — Cosmetic Online Shop

An online cosmetics shop with an **ASP.NET Core** Web API backend, an **Angular** customer storefront, and a separate **Angular admin dashboard**.

## Stack

| Layer | Technology |
| --- | --- |
| Backend | C# / ASP.NET Core 8, EF Core, SQLite, JWT auth |
| Storefront | Angular 19 (`frontend/`, port 4200) |
| Admin | Angular 19 (`admin/`, port 4300) |
| API docs | Swagger UI (`/swagger`) |

## Features

- Product catalog with categories, search, featured items, and brand / skin type filters
- Customer wishlist (save favorites with the heart button; managed under Account)
- Product detail pages, cart, checkout, and orders
- Bilingual UI: English and Persian (فارسی), with RTL layout
- **Admin dashboard** (separate app): manage categories & products, upload product images, view orders

## Project structure

```
backend/
  CosmeticShop.sln
  CosmeticShop.Api/          # ASP.NET Core Web API
frontend/                    # Customer storefront
admin/                       # Admin dashboard (separate Angular app)
scripts/                     # Helper run scripts
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/) (includes `npm`)

## Run locally

### 1. Start the API

```bash
cd backend/CosmeticShop.Api
dotnet run --launch-profile http
```

API: `http://localhost:5041` · Swagger: `http://localhost:5041/swagger`

### 2. Start the customer storefront

```bash
cd frontend
npm install
npm start
```

Storefront: `http://localhost:4200`

### 3. Start the admin dashboard

```bash
cd admin
npm install
npm start
```

Admin: `http://localhost:4300`

**Default admin login**
- Email: `admin@velora.com`
- Password: `Admin123!`

Windows helpers: `scripts\run-api.cmd`, `scripts\run-web.cmd`, `scripts\run-admin.cmd`

## Admin capabilities

- Sign in with JWT
- Create / edit / delete **categories**
- Create / edit / delete **products** (treated as catalog items)
- Pick a product image from your file explorer (JPG/PNG/WEBP/GIF upload)
- View the full **orders** list and order details

## Public API overview

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/api/categories` | List categories |
| `GET` | `/api/products` | List products (`categoryId`, `search`, `featured`, `brand`, `skinType`) |
| `GET` | `/api/products/filters` | Distinct brand and skin type filter options |
| `GET` | `/api/products/{id}` | Product by id |
| `POST` | `/api/orders` | Place an order |
| `GET` | `/api/orders/{id}` | Get order details |
| `GET` | `/api/account/wishlist` | List saved products (customer JWT) |
| `PUT` | `/api/account/wishlist/{productId}` | Save a product (customer JWT) |
| `DELETE` | `/api/account/wishlist/{productId}` | Remove a saved product (customer JWT) |

## Admin API overview (JWT required)

| Method | Endpoint | Description |
| --- | --- | --- |
| `POST` | `/api/admin/auth/login` | Admin login |
| `GET/POST/PUT/DELETE` | `/api/admin/categories` | Category CRUD |
| `GET/POST/PUT/DELETE` | `/api/admin/products` | Product CRUD |
| `POST` | `/api/admin/products/{id}/image` | Upload product image |
| `GET` | `/api/admin/orders` | List orders |
| `GET` | `/api/admin/orders/{id}` | Order details |

## Notes

- SQLite database file `cosmeticshop.db` is created automatically on first run.
- Uploaded images are stored under `backend/CosmeticShop.Api/wwwroot/uploads/products/`.
- Free shipping applies to orders of $75 or more; otherwise shipping is $6.95.
- Change the seeded admin password / JWT key in `appsettings.json` before production use.
