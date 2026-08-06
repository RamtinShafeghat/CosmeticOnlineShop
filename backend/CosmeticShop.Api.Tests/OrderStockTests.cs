using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CosmeticShop.Api.Data;
using CosmeticShop.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CosmeticShop.Api.Tests;

public class OrderStockTests : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"cosmeticshop-test-{Guid.NewGuid():N}.db");
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public OrderStockTests()
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
    public async Task CreateOrder_RejectsDuplicateLinesThatExceedStock()
    {
        var product = await SeedProductAsync(stock: 5);

        var response = await _client.PostAsJsonAsync("/api/orders", new
        {
            customerName = "Test Buyer",
            email = "buyer@example.com",
            phone = "555-0100",
            shippingAddress = "1 Test St",
            city = "Tehran",
            postalCode = "12345",
            items = new[]
            {
                new { productId = product.Id, quantity = 4 },
                new { productId = product.Id, quantity = 4 }
            }
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var remaining = await db.Products.Where(p => p.Id == product.Id).Select(p => p.Stock).SingleAsync();
        Assert.Equal(5, remaining);
        Assert.Equal(0, await db.Orders.CountAsync());
    }

    [Fact]
    public async Task CreateOrder_AggregatesDuplicateLinesWithinStock()
    {
        var product = await SeedProductAsync(stock: 10);

        var response = await _client.PostAsJsonAsync("/api/orders", new
        {
            customerName = "Test Buyer",
            email = "buyer@example.com",
            phone = "555-0100",
            shippingAddress = "1 Test St",
            city = "Tehran",
            postalCode = "12345",
            items = new[]
            {
                new { productId = product.Id, quantity = 3 },
                new { productId = product.Id, quantity = 2 }
            }
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var items = doc.RootElement.GetProperty("items");
        Assert.Equal(1, items.GetArrayLength());
        Assert.Equal(5, items[0].GetProperty("quantity").GetInt32());

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var remaining = await db.Products.Where(p => p.Id == product.Id).Select(p => p.Stock).SingleAsync();
        Assert.Equal(5, remaining);
    }

    private async Task<Product> SeedProductAsync(int stock)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var category = await db.Categories.FirstOrDefaultAsync();
        if (category is null)
        {
            category = new Category
            {
                Name = "Test",
                NameFa = "تست",
                Slug = $"test-{Guid.NewGuid():N}",
                Description = "Test category",
                DescriptionFa = "دسته تست"
            };
            db.Categories.Add(category);
            await db.SaveChangesAsync();
        }

        var product = new Product
        {
            Name = "Limited Stock Serum",
            NameFa = "سرم محدود",
            Slug = $"limited-{Guid.NewGuid():N}",
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
}
