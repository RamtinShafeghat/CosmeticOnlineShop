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

public class WishlistTests : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"cosmeticshop-wishlist-{Guid.NewGuid():N}.db");
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public WishlistTests()
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
    public async Task Wishlist_RequiresAuthentication()
    {
        var product = await SeedProductAsync("wishlist-auth");

        var getResponse = await _client.GetAsync("/api/account/wishlist");
        var putResponse = await _client.PutAsync($"/api/account/wishlist/{product.Id}", null);
        var deleteResponse = await _client.DeleteAsync($"/api/account/wishlist/{product.Id}");

        Assert.Equal(HttpStatusCode.Unauthorized, getResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, putResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task AddToWishlist_ReturnsNotFoundForMissingProduct()
    {
        await AuthenticateAsync("wisher-missing@example.com");

        var response = await _client.PutAsync("/api/account/wishlist/999999", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Wishlist_AddListRemove_RoundTrips()
    {
        var product = await SeedProductAsync("wishlist-roundtrip");
        await AuthenticateAsync("wisher@example.com");

        var addResponse = await _client.PutAsync($"/api/account/wishlist/{product.Id}", null);
        Assert.Equal(HttpStatusCode.NoContent, addResponse.StatusCode);

        var list = await _client.GetFromJsonAsync<List<WishlistProduct>>("/api/account/wishlist", JsonOptions);
        Assert.NotNull(list);
        var saved = Assert.Single(list);
        Assert.Equal(product.Id, saved.Id);
        Assert.Equal(product.Name, saved.Name);

        var removeResponse = await _client.DeleteAsync($"/api/account/wishlist/{product.Id}");
        Assert.Equal(HttpStatusCode.NoContent, removeResponse.StatusCode);

        var afterRemove = await _client.GetFromJsonAsync<List<WishlistProduct>>("/api/account/wishlist", JsonOptions);
        Assert.NotNull(afterRemove);
        Assert.Empty(afterRemove);
    }

    [Fact]
    public async Task AddToWishlist_IsIdempotent()
    {
        var product = await SeedProductAsync("wishlist-idempotent");
        await AuthenticateAsync("wisher-twice@example.com");

        var first = await _client.PutAsync($"/api/account/wishlist/{product.Id}", null);
        var second = await _client.PutAsync($"/api/account/wishlist/{product.Id}", null);

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, second.StatusCode);

        var list = await _client.GetFromJsonAsync<List<WishlistProduct>>("/api/account/wishlist", JsonOptions);
        Assert.NotNull(list);
        Assert.Single(list);
    }

    [Fact]
    public async Task Wishlist_IsScopedToCustomer()
    {
        var product = await SeedProductAsync("wishlist-scoped");

        await AuthenticateAsync("wisher-a@example.com");
        var addResponse = await _client.PutAsync($"/api/account/wishlist/{product.Id}", null);
        Assert.Equal(HttpStatusCode.NoContent, addResponse.StatusCode);

        await AuthenticateAsync("wisher-b@example.com");
        var otherList = await _client.GetFromJsonAsync<List<WishlistProduct>>("/api/account/wishlist", JsonOptions);
        Assert.NotNull(otherList);
        Assert.Empty(otherList);
    }

    private async Task AuthenticateAsync(string email)
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            fullName = "Wisher",
            email,
            phone = "555-0100",
            password = "Secret1"
        });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        Assert.NotNull(body);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body.Token);
    }

    private async Task<Product> SeedProductAsync(string slugPrefix)
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
            Name = "Wishlisted Serum",
            NameFa = "سرم",
            Slug = $"{slugPrefix}-{Guid.NewGuid():N}",
            ShortDescription = "Short",
            ShortDescriptionFa = "کوتاه",
            Description = "Desc",
            DescriptionFa = "توضیح",
            Price = 30m,
            ImageUrl = "https://example.com/p.jpg",
            Brand = "Velora",
            SkinType = "All",
            Stock = 15,
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
    private sealed record WishlistProduct(int Id, string Name, string Slug, decimal Price);
}
