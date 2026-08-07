using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CosmeticShop.Api.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CosmeticShop.Api.Tests;

public class SchemaUpgradeTests : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"cosmeticshop-pre-i18n-{Guid.NewGuid():N}.db");
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SchemaUpgradeTests()
    {
        CreatePreBilingualDatabase(_dbPath);

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(AppDbContext));
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite($"Data Source={_dbPath}"));
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Startup_UpgradesPreBilingualSqlite_AndServesCatalog()
    {
        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Assert.True(await ColumnExistsAsync(db, "Products", "NameFa"));
            Assert.True(await ColumnExistsAsync(db, "Products", "Brand"));
            Assert.True(await ColumnExistsAsync(db, "Products", "SkinType"));
            Assert.True(await ColumnExistsAsync(db, "Categories", "NameFa"));
            Assert.True(await ColumnExistsAsync(db, "OrderItems", "ProductNameFa"));
        }

        var productsResponse = await _client.GetAsync("/api/products");
        Assert.Equal(HttpStatusCode.OK, productsResponse.StatusCode);
        var products = await productsResponse.Content.ReadFromJsonAsync<List<ProductRow>>(JsonOptions);
        Assert.NotNull(products);
        Assert.Contains(products, p => p.Name == "Legacy Serum" && p.Stock == 5);

        var categoriesResponse = await _client.GetAsync("/api/categories");
        Assert.Equal(HttpStatusCode.OK, categoriesResponse.StatusCode);

        var orderResponse = await _client.PostAsJsonAsync("/api/orders", new
        {
            customerName = "Legacy Buyer",
            email = "legacy@example.com",
            phone = "555-0100",
            shippingAddress = "1 Upgrade Lane",
            city = "Tehran",
            postalCode = "12345",
            items = new[]
            {
                new { productId = products[0].Id, quantity = 1 }
            }
        });
        Assert.Equal(HttpStatusCode.Created, orderResponse.StatusCode);
    }

    private static void CreatePreBilingualDatabase(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        using var connection = new SqliteConnection($"Data Source={path}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE "Categories" (
              "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
              "Name" TEXT NOT NULL,
              "Slug" TEXT NOT NULL,
              "Description" TEXT NOT NULL
            );
            CREATE TABLE "Products" (
              "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
              "Name" TEXT NOT NULL,
              "Slug" TEXT NOT NULL,
              "Description" TEXT NOT NULL,
              "ShortDescription" TEXT NOT NULL,
              "Price" TEXT NOT NULL,
              "ImageUrl" TEXT NOT NULL,
              "Stock" INTEGER NOT NULL,
              "IsFeatured" INTEGER NOT NULL,
              "CategoryId" INTEGER NOT NULL,
              FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE CASCADE
            );
            CREATE TABLE "Orders" (
              "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
              "CustomerName" TEXT NOT NULL,
              "Email" TEXT NOT NULL,
              "Phone" TEXT NOT NULL,
              "ShippingAddress" TEXT NOT NULL,
              "City" TEXT NOT NULL,
              "PostalCode" TEXT NOT NULL,
              "Status" TEXT NOT NULL,
              "Subtotal" TEXT NOT NULL,
              "ShippingCost" TEXT NOT NULL,
              "Total" TEXT NOT NULL,
              "CreatedAt" TEXT NOT NULL
            );
            CREATE TABLE "OrderItems" (
              "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
              "OrderId" INTEGER NOT NULL,
              "ProductId" INTEGER NOT NULL,
              "ProductName" TEXT NOT NULL,
              "UnitPrice" TEXT NOT NULL,
              "Quantity" INTEGER NOT NULL,
              FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE,
              FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE RESTRICT
            );
            CREATE TABLE "AdminUsers" (
              "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
              "Email" TEXT NOT NULL,
              "PasswordHash" TEXT NOT NULL,
              "DisplayName" TEXT NOT NULL
            );
            CREATE UNIQUE INDEX "IX_AdminUsers_Email" ON "AdminUsers" ("Email");
            INSERT INTO Categories (Name, Slug, Description) VALUES ('Skincare', 'skincare', 'desc');
            INSERT INTO Products (Name,Slug,Description,ShortDescription,Price,ImageUrl,Stock,IsFeatured,CategoryId)
            VALUES ('Legacy Serum','legacy-serum','d','s','10.0','',5,1,1);
            """;
        command.ExecuteNonQuery();
    }

    private static async Task<bool> ColumnExistsAsync(AppDbContext db, string tableName, string columnName)
    {
        var connection = db.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        await using var command = connection.CreateCommand();
        command.CommandText =
            $"SELECT 1 FROM pragma_table_info('{tableName}') WHERE name = $column LIMIT 1;";
        var parameter = command.CreateParameter();
        parameter.ParameterName = "$column";
        parameter.Value = columnName;
        command.Parameters.Add(parameter);
        var result = await command.ExecuteScalarAsync();
        return result is not null && result is not DBNull;
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
        try
        {
            if (File.Exists(_dbPath))
            {
                File.Delete(_dbPath);
            }
        }
        catch
        {
            // best-effort cleanup
        }
    }

    private sealed record ProductRow(int Id, string Name, int Stock);
}
