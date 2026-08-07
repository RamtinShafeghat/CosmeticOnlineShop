using System.Net.Http.Json;
using System.Text.Json;
using CosmeticShop.Api.Data;
using CosmeticShop.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CosmeticShop.Api.Tests;

public class ProductFilterTests : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"cosmeticshop-filters-{Guid.NewGuid():N}.db");
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProductFilterTests()
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
    public async Task GetProducts_FiltersByBrand()
    {
        await SeedCatalogAsync();

        var products = await _client.GetFromJsonAsync<List<ProductRow>>(
            "/api/products?brand=Lumine", JsonOptions);

        Assert.NotNull(products);
        Assert.NotEmpty(products);
        Assert.All(products, p => Assert.Equal("Lumine", p.Brand));
    }

    [Fact]
    public async Task GetProducts_SkinTypeFilterIncludesAllSkinTypeProducts()
    {
        await SeedCatalogAsync();

        var products = await _client.GetFromJsonAsync<List<ProductRow>>(
            "/api/products?skinType=Dry", JsonOptions);

        Assert.NotNull(products);
        Assert.Contains(products, p => p.SkinType == "Dry");
        Assert.Contains(products, p => p.SkinType == "All");
        Assert.DoesNotContain(products, p => p.SkinType == "Sensitive");
    }

    [Fact]
    public async Task GetProducts_CombinesBrandAndSkinTypeFilters()
    {
        await SeedCatalogAsync();

        var products = await _client.GetFromJsonAsync<List<ProductRow>>(
            "/api/products?brand=Velora&skinType=Dry", JsonOptions);

        Assert.NotNull(products);
        Assert.NotEmpty(products);
        Assert.All(products, p =>
        {
            Assert.Equal("Velora", p.Brand);
            Assert.True(p.SkinType is "Dry" or "All", $"Unexpected skin type {p.SkinType}");
        });
    }

    [Fact]
    public async Task GetFilterOptions_ReturnsDistinctSortedValues()
    {
        await SeedCatalogAsync();

        var options = await _client.GetFromJsonAsync<FilterOptions>("/api/products/filters", JsonOptions);

        Assert.NotNull(options);
        Assert.Contains("Velora", options.Brands);
        Assert.Contains("Lumine", options.Brands);
        Assert.Equal(options.Brands.Distinct().Count(), options.Brands.Count);
        Assert.Equal(options.Brands.OrderBy(b => b).ToList(), options.Brands);
        Assert.Contains("Dry", options.SkinTypes);
        Assert.Contains("All", options.SkinTypes);
        Assert.Equal(options.SkinTypes.Distinct().Count(), options.SkinTypes.Count);
    }

    private async Task SeedCatalogAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();

        var category = new Category
        {
            Name = "Test",
            NameFa = "تست",
            Slug = $"test-{Guid.NewGuid():N}",
            Description = "Test",
            DescriptionFa = "تست"
        };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        db.Products.AddRange(
            NewProduct("Velora Dry Cream", "Velora", "Dry", category.Id),
            NewProduct("Velora Universal Serum", "Velora", "All", category.Id),
            NewProduct("Lumine Sensitive Mist", "Lumine", "Sensitive", category.Id));
        await db.SaveChangesAsync();
    }

    private static Product NewProduct(string name, string brand, string skinType, int categoryId) => new()
    {
        Name = name,
        NameFa = name,
        Slug = $"{name.ToLower().Replace(' ', '-')}-{Guid.NewGuid():N}",
        ShortDescription = "Short",
        ShortDescriptionFa = "کوتاه",
        Description = "Desc",
        DescriptionFa = "توضیح",
        Price = 20m,
        ImageUrl = "https://example.com/p.jpg",
        Brand = brand,
        SkinType = skinType,
        Stock = 10,
        IsFeatured = false,
        CategoryId = categoryId
    };

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

    private sealed record ProductRow(int Id, string Name, string Brand, string SkinType);
    private sealed record FilterOptions(List<string> Brands, List<string> SkinTypes);
}
