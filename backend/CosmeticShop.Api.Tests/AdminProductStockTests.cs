using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CosmeticShop.Api.Data;
using CosmeticShop.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CosmeticShop.Api.Tests;

public class AdminProductStockTests : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"cosmeticshop-admin-stock-{Guid.NewGuid():N}.db");
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AdminProductStockTests()
    {
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
    public async Task UpdateProduct_DoesNotRestoreStockSoldDuringStaleAdminEdit()
    {
        var product = await SeedProductAsync(stock: 10);
        await AuthorizeAsAdminAsync();

        // Simulate checkout selling 3 units while the admin form still holds stock=10.
        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var reserved = await db.Products
                .Where(p => p.Id == product.Id && p.Stock >= 3)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(p => p.Stock, p => p.Stock - 3));
            Assert.Equal(1, reserved);
        }

        var response = await _client.PutAsJsonAsync($"/api/admin/products/{product.Id}", new
        {
            name = "Renamed Serum",
            nameFa = "سرم",
            slug = product.Slug,
            description = "Updated description",
            descriptionFa = "توضیح",
            shortDescription = "Short",
            shortDescriptionFa = "کوتاه",
            price = 20m,
            imageUrl = product.ImageUrl,
            brand = "Velora",
            skinType = "All",
            stock = 10, // stale value from when the edit form loaded
            isFeatured = false,
            categoryId = product.CategoryId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var row = await db.Products.AsNoTracking().SingleAsync(p => p.Id == product.Id);
            Assert.Equal("Renamed Serum", row.Name);
            Assert.Equal(7, row.Stock);
        }
    }

    [Fact]
    public async Task UpdateProductStock_SetsAbsoluteStock_WhenExpectedMatches()
    {
        var product = await SeedProductAsync(stock: 10);
        await AuthorizeAsAdminAsync();

        var response = await _client.PutAsJsonAsync($"/api/admin/products/{product.Id}/stock", new
        {
            stock = 25,
            expectedStock = 10
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ProductDetail>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(25, body.Stock);

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var remaining = await db.Products.Where(p => p.Id == product.Id).Select(p => p.Stock).SingleAsync();
        Assert.Equal(25, remaining);
    }

    [Fact]
    public async Task UpdateProductStock_RejectsNullStock_WithoutWipingInventory()
    {
        var product = await SeedProductAsync(stock: 42);
        await AuthorizeAsAdminAsync();

        // Mirrors the admin form when the stock <input type="number"> is cleared:
        // Angular binds null and previously POSTed { "stock": null }, which bound to 0.
        using var nullContent = new StringContent(
            """{"stock":null}""",
            System.Text.Encoding.UTF8,
            "application/json");
        var nullResponse = await _client.PutAsync(
            $"/api/admin/products/{product.Id}/stock",
            nullContent);
        Assert.Equal(HttpStatusCode.BadRequest, nullResponse.StatusCode);

        using var omittedContent = new StringContent(
            "{}",
            System.Text.Encoding.UTF8,
            "application/json");
        var omittedResponse = await _client.PutAsync(
            $"/api/admin/products/{product.Id}/stock",
            omittedContent);
        Assert.Equal(HttpStatusCode.BadRequest, omittedResponse.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var remaining = await db.Products.Where(p => p.Id == product.Id).Select(p => p.Stock).SingleAsync();
        Assert.Equal(42, remaining);
    }

    [Fact]
    public async Task UpdateProductStock_RejectsStaleExpectedStock_AfterConcurrentCheckout()
    {
        var product = await SeedProductAsync(stock: 10);
        await AuthorizeAsAdminAsync();

        // Checkout reserves 3 while the admin form still believes stock is 10.
        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var reserved = await db.Products
                .Where(p => p.Id == product.Id && p.Stock >= 3)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(p => p.Stock, p => p.Stock - 3));
            Assert.Equal(1, reserved);
        }

        // Stale restock (10 → 11) must not restore the 3 sold units.
        var response = await _client.PutAsJsonAsync($"/api/admin/products/{product.Id}/stock", new
        {
            stock = 11,
            expectedStock = 10
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var remaining = await db.Products.Where(p => p.Id == product.Id).Select(p => p.Stock).SingleAsync();
            Assert.Equal(7, remaining);
        }
    }

    private async Task AuthorizeAsAdminAsync()
    {
        var login = await _client.PostAsJsonAsync("/api/admin/auth/login", new
        {
            email = "admin@velora.com",
            password = "Admin123!"
        });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var auth = await login.Content.ReadFromJsonAsync<AdminAuth>(JsonOptions);
        Assert.NotNull(auth);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);
    }

    private async Task<Product> SeedProductAsync(int stock)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();

        var category = await db.Categories.FirstOrDefaultAsync();
        if (category is null)
        {
            category = new Category
            {
                Name = "Test",
                NameFa = "تست",
                Slug = $"test-{Guid.NewGuid():N}",
                Description = "Test",
                DescriptionFa = "تست"
            };
            db.Categories.Add(category);
            await db.SaveChangesAsync();
        }

        var product = new Product
        {
            Name = "Admin Stock Serum",
            NameFa = "سرم",
            Slug = $"admin-stock-{Guid.NewGuid():N}",
            ShortDescription = "Short",
            ShortDescriptionFa = "کوتاه",
            Description = "Description",
            DescriptionFa = "توضیح",
            Price = 20m,
            ImageUrl = "/uploads/products/test.jpg",
            Brand = "Velora",
            SkinType = "All",
            Stock = stock,
            IsFeatured = false,
            CategoryId = category.Id
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();
        return product;
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }
    }

    private sealed record AdminAuth(string Token);
    private sealed record ProductDetail(int Id, string Name, int Stock);
}
