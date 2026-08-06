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

public class OrderAccessTests : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"cosmeticshop-test-{Guid.NewGuid():N}.db");
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public OrderAccessTests()
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
    public async Task GetOrder_WithoutToken_ReturnsNotFound()
    {
        var created = await PlaceOrderAsync();

        var response = await _client.GetAsync($"/api/orders/{created.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetOrder_WithWrongToken_ReturnsNotFound()
    {
        var created = await PlaceOrderAsync();

        var response = await _client.GetAsync(
            $"/api/orders/{created.Id}?token={Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetOrder_WithPublicToken_ReturnsOrder()
    {
        var created = await PlaceOrderAsync();

        var response = await _client.GetAsync(
            $"/api/orders/{created.Id}?token={created.PublicToken}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<OrderResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(created.Id, body.Id);
        Assert.Equal("buyer@example.com", body.Email);
        Assert.Equal(created.PublicToken, body.PublicToken);
    }

    private async Task<OrderResponse> PlaceOrderAsync()
    {
        var product = await SeedProductAsync(stock: 10);

        var response = await _client.PostAsJsonAsync("/api/orders", new
        {
            customerName = "Test Buyer",
            email = "buyer@example.com",
            phone = "555-0100",
            shippingAddress = "1 Private Lane",
            city = "Tehran",
            postalCode = "12345",
            items = new[]
            {
                new { productId = product.Id, quantity = 1 }
            }
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<OrderResponse>(JsonOptions);
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created.PublicToken);
        return created;
    }

    private async Task<Product> SeedProductAsync(int stock)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Ensure schema exists (seeder runs at host start against the replaced DbContext).
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
            Name = "Test Serum",
            NameFa = "سرم تست",
            Slug = $"test-serum-{Guid.NewGuid():N}",
            ShortDescription = "Short",
            ShortDescriptionFa = "کوتاه",
            Description = "Desc",
            DescriptionFa = "توضیح",
            Price = 20m,
            ImageUrl = "https://example.com/p.jpg",
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

    private sealed record OrderResponse(
        int Id,
        Guid PublicToken,
        string Email);
}
