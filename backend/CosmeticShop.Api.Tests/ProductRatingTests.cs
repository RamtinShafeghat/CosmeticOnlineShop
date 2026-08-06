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

public class ProductRatingTests : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"cosmeticshop-rating-{Guid.NewGuid():N}.db");
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProductRatingTests()
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
    public async Task RateProduct_RequiresAuthentication()
    {
        var product = await SeedProductAsync();

        var response = await _client.PutAsJsonAsync($"/api/products/{product.Id}/rating", new { stars = 5 });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RateProduct_SavesStarsAndUpdatesAverage()
    {
        var product = await SeedProductAsync();
        var token = await RegisterAndGetTokenAsync("rater@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PutAsJsonAsync($"/api/products/{product.Id}/rating", new { stars = 4 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var summary = await response.Content.ReadFromJsonAsync<RatingSummary>(JsonOptions);
        Assert.NotNull(summary);
        Assert.Equal(4, summary.MyRating);
        Assert.Equal(1, summary.RatingCount);
        Assert.Equal(4, summary.AverageRating);

        var detail = await _client.GetFromJsonAsync<ProductDetail>($"/api/products/{product.Id}", JsonOptions);
        Assert.NotNull(detail);
        Assert.Equal(4, detail.MyRating);
        Assert.Equal(4, detail.AverageRating);
        Assert.Equal(1, detail.RatingCount);
    }

    private async Task<string> RegisterAndGetTokenAsync(string email)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            fullName = "Rater",
            email,
            phone = "555-0199",
            password = "Secret1"
        });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        Assert.NotNull(body);
        return body.Token;
    }

    private async Task<Product> SeedProductAsync()
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
            Name = "Rated Serum",
            NameFa = "سرم",
            Slug = $"rated-serum-{Guid.NewGuid():N}",
            ShortDescription = "Short",
            ShortDescriptionFa = "کوتاه",
            Description = "Desc",
            DescriptionFa = "توضیح",
            Price = 25m,
            ImageUrl = "https://example.com/p.jpg",
            Brand = "Velora",
            SkinType = "All",
            Stock = 20,
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

    private sealed record AuthResponse(string Token);
    private sealed record RatingSummary(int ProductId, double AverageRating, int RatingCount, int? MyRating);
    private sealed record ProductDetail(int Id, double AverageRating, int RatingCount, int? MyRating);
}
