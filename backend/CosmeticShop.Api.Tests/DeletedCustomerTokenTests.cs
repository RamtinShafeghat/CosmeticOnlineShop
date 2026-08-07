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

public class DeletedCustomerTokenTests : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"cosmeticshop-delcust-{Guid.NewGuid():N}.db");
    private readonly WebApplicationFactory<Program> _factory;

    public DeletedCustomerTokenTests()
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
    }

    [Fact]
    public async Task Checkout_WithJwtAfterAdminDeletesCustomer_SucceedsAsGuest()
    {
        var product = await SeedProductAsync();
        var customerToken = await RegisterCustomerAsync("doomed@example.com");
        await DeleteCustomerAsAdminAsync("doomed@example.com");

        using var customerClient = _factory.CreateClient();
        customerClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", customerToken);

        var stockBefore = await GetStockAsync(product.Id);

        var orderResponse = await customerClient.PostAsJsonAsync("/api/orders", new
        {
            customerName = "Doomed Customer",
            email = "doomed@example.com",
            phone = "555-0100",
            shippingAddress = "1 Gone St",
            city = "Tehran",
            postalCode = "12345",
            saveAddress = true,
            items = new[] { new { productId = product.Id, quantity = 1 } }
        });

        Assert.Equal(HttpStatusCode.Created, orderResponse.StatusCode);
        var order = await orderResponse.Content.ReadFromJsonAsync<CreatedOrder>(JsonOptions);
        Assert.NotNull(order);

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var stored = await db.Orders.SingleAsync(o => o.Id == order.Id);
            Assert.Null(stored.CustomerId);
            Assert.Equal(0, await db.CustomerAddresses.CountAsync());
        }

        Assert.Equal(stockBefore - 1, await GetStockAsync(product.Id));
    }

    [Fact]
    public async Task WishlistWrite_WithJwtAfterAdminDeletesCustomer_ReturnsUnauthorized()
    {
        var product = await SeedProductAsync();
        var customerToken = await RegisterCustomerAsync("wishlist-gone@example.com");
        await DeleteCustomerAsAdminAsync("wishlist-gone@example.com");

        using var customerClient = _factory.CreateClient();
        customerClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", customerToken);

        var response = await customerClient.PutAsync($"/api/account/wishlist/{product.Id}", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<string> RegisterCustomerAsync(string email)
    {
        using var client = _factory.CreateClient();
        var register = await client.PostAsJsonAsync("/api/auth/register", new
        {
            fullName = "Doomed Customer",
            email,
            phone = "555-0100",
            password = "Secret1"
        });
        Assert.Equal(HttpStatusCode.OK, register.StatusCode);
        var auth = await register.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        Assert.NotNull(auth);
        return auth.Token;
    }

    private async Task DeleteCustomerAsAdminAsync(string email)
    {
        using var adminClient = _factory.CreateClient();
        var login = await adminClient.PostAsJsonAsync("/api/admin/auth/login", new
        {
            email = "admin@velora.com",
            password = "Admin123!"
        });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var adminAuth = await login.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        Assert.NotNull(adminAuth);
        adminClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", adminAuth.Token);

        var customers = await adminClient.GetFromJsonAsync<List<CustomerListItem>>(
            $"/api/admin/customers?search={Uri.EscapeDataString(email)}", JsonOptions);
        var customer = Assert.Single(customers!);
        var delete = await adminClient.DeleteAsync($"/api/admin/customers/{customer.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
    }

    private async Task<int> GetStockAsync(int productId)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.Products.Where(p => p.Id == productId).Select(p => p.Stock).SingleAsync();
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
            Name = "Delete Token Serum",
            NameFa = "سرم",
            Slug = $"delete-token-{Guid.NewGuid():N}",
            ShortDescription = "Short",
            ShortDescriptionFa = "کوتاه",
            Description = "Desc",
            DescriptionFa = "توضیح",
            Price = 30m,
            ImageUrl = "https://example.com/p.jpg",
            Brand = "Velora",
            SkinType = "All",
            Stock = 8,
            IsFeatured = false,
            CategoryId = category.Id
        };
        db.Products.Add(product);
        await db.SaveChangesAsync();
        return product;
    }

    public void Dispose()
    {
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
    private sealed record CustomerListItem(int Id, string Email);
    private sealed record CreatedOrder(int Id);
}
