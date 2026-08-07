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

public class AdminCustomerTests : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"cosmeticshop-admincust-{Guid.NewGuid():N}.db");
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AdminCustomerTests()
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
    public async Task GetCustomers_RequiresAdminAuth()
    {
        var response = await _client.GetAsync("/api/admin/customers");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCustomers_RejectsCustomerToken()
    {
        var customerToken = await RegisterCustomerAsync("not-admin@example.com");
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/customers");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", customerToken);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetCustomers_ListsAndSearches()
    {
        await RegisterCustomerAsync("alice@example.com", "Alice Aria");
        await RegisterCustomerAsync("bob@example.com", "Bob Blend");
        await AuthenticateAsAdminAsync();

        var all = await _client.GetFromJsonAsync<List<CustomerListItem>>("/api/admin/customers", JsonOptions);
        Assert.NotNull(all);
        Assert.Contains(all, c => c.Email == "alice@example.com");
        Assert.Contains(all, c => c.Email == "bob@example.com");

        var filtered = await _client.GetFromJsonAsync<List<CustomerListItem>>(
            "/api/admin/customers?search=alice", JsonOptions);
        Assert.NotNull(filtered);
        Assert.Single(filtered);
        Assert.Equal("Alice Aria", filtered[0].FullName);
    }

    [Fact]
    public async Task UpdateCustomer_ChangesProfileAndPassword()
    {
        await RegisterCustomerAsync("edit-me@example.com", "Before Edit");
        await AuthenticateAsAdminAsync();

        var list = await _client.GetFromJsonAsync<List<CustomerListItem>>(
            "/api/admin/customers?search=edit-me", JsonOptions);
        var id = Assert.Single(list!).Id;

        var response = await _client.PutAsJsonAsync($"/api/admin/customers/{id}", new
        {
            fullName = "After Edit",
            email = "edited@example.com",
            phone = "555-0111",
            newPassword = "NewSecret1"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var detail = await response.Content.ReadFromJsonAsync<CustomerDetail>(JsonOptions);
        Assert.NotNull(detail);
        Assert.Equal("After Edit", detail.FullName);
        Assert.Equal("edited@example.com", detail.Email);

        // The customer can sign in with the admin-issued password.
        using var loginClient = _factory.CreateClient();
        var login = await loginClient.PostAsJsonAsync("/api/auth/login", new
        {
            email = "edited@example.com",
            password = "NewSecret1"
        });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
    }

    [Fact]
    public async Task UpdateCustomer_RejectsDuplicateEmail()
    {
        await RegisterCustomerAsync("taken@example.com");
        await RegisterCustomerAsync("changing@example.com");
        await AuthenticateAsAdminAsync();

        var list = await _client.GetFromJsonAsync<List<CustomerListItem>>(
            "/api/admin/customers?search=changing", JsonOptions);
        var id = Assert.Single(list!).Id;

        var response = await _client.PutAsJsonAsync($"/api/admin/customers/{id}", new
        {
            fullName = "Changing",
            email = "taken@example.com",
            phone = ""
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCustomer_KeepsOrdersAsGuestOrders()
    {
        var token = await RegisterCustomerAsync("leaving@example.com");
        var product = await SeedProductAsync();

        using (var orderClient = _factory.CreateClient())
        {
            orderClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var orderResponse = await orderClient.PostAsJsonAsync("/api/orders", new
            {
                customerName = "Leaving Customer",
                email = "leaving@example.com",
                phone = "555-0100",
                shippingAddress = "1 Farewell Way",
                city = "Tehran",
                postalCode = "12345",
                items = new[] { new { productId = product.Id, quantity = 1 } }
            });
            Assert.Equal(HttpStatusCode.Created, orderResponse.StatusCode);
        }

        await AuthenticateAsAdminAsync();
        var list = await _client.GetFromJsonAsync<List<CustomerListItem>>(
            "/api/admin/customers?search=leaving", JsonOptions);
        var customer = Assert.Single(list!);
        Assert.Equal(1, customer.OrderCount);

        var delete = await _client.DeleteAsync($"/api/admin/customers/{customer.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

        var detail = await _client.GetAsync($"/api/admin/customers/{customer.Id}");
        Assert.Equal(HttpStatusCode.NotFound, detail.StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var order = await db.Orders.SingleAsync(o => o.Email == "leaving@example.com");
        Assert.Null(order.CustomerId);
    }

    private async Task AuthenticateAsAdminAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/admin/auth/login", new
        {
            email = "admin@velora.com",
            password = "Admin123!"
        });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        Assert.NotNull(body);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body.Token);
    }

    private async Task<string> RegisterCustomerAsync(string email, string fullName = "Test Customer")
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            fullName,
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
            Name = "Admin Test Serum",
            NameFa = "سرم",
            Slug = $"admin-test-serum-{Guid.NewGuid():N}",
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
    private sealed record CustomerListItem(
        int Id,
        string FullName,
        string Email,
        string Phone,
        DateTime CreatedAt,
        int OrderCount,
        decimal TotalSpent);
    private sealed record CustomerDetail(
        int Id,
        string FullName,
        string Email,
        string Phone,
        DateTime CreatedAt,
        int OrderCount,
        decimal TotalSpent);
}
