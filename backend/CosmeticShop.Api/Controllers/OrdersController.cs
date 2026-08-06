using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CosmeticShop.Api.Data;
using CosmeticShop.Api.Dtos;
using CosmeticShop.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CosmeticShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(AppDbContext db) : ControllerBase
{
    private const decimal FreeShippingThreshold = 75m;
    private const decimal StandardShipping = 6.95m;

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await db.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order is null ? NotFound() : Ok(MapOrder(order));
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto request)
    {
        if (!ModelState.IsValid || request.Items.Count == 0)
        {
            return ValidationProblem(ModelState);
        }

        // Aggregate duplicate product lines first. Per-line stock checks against the same
        // balance would otherwise accept split quantities that exceed available stock.
        var quantityByProductId = request.Items
            .GroupBy(i => i.ProductId)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));

        foreach (var (productId, quantity) in quantityByProductId)
        {
            if (quantity < 1 || quantity > 99)
            {
                return BadRequest(new
                {
                    message = $"Quantity for product {productId} must be between 1 and 99."
                });
            }
        }

        var productIds = quantityByProductId.Keys.ToList();
        var products = await db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        if (products.Count != productIds.Count)
        {
            return BadRequest(new { message = "One or more products were not found." });
        }

        foreach (var (productId, quantity) in quantityByProductId)
        {
            var product = products[productId];
            if (product.Stock < quantity)
            {
                return BadRequest(new
                {
                    message = $"Insufficient stock for '{product.Name}'. Available: {product.Stock}."
                });
            }
        }

        var orderItems = quantityByProductId.Select(pair =>
        {
            var product = products[pair.Key];
            return new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductNameFa = product.NameFa,
                UnitPrice = product.Price,
                Quantity = pair.Value
            };
        }).ToList();

        var subtotal = orderItems.Sum(i => i.UnitPrice * i.Quantity);
        var shipping = subtotal >= FreeShippingThreshold ? 0m : StandardShipping;
        var customerId = GetCustomerId();

        var order = new Order
        {
            CustomerId = customerId,
            CustomerName = request.CustomerName.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone.Trim(),
            ShippingAddress = request.ShippingAddress.Trim(),
            City = request.City.Trim(),
            PostalCode = request.PostalCode.Trim(),
            Status = "Pending",
            Subtotal = subtotal,
            ShippingCost = shipping,
            Total = subtotal + shipping,
            CreatedAt = DateTime.UtcNow,
            Items = orderItems
        };

        foreach (var (productId, quantity) in quantityByProductId)
        {
            products[productId].Stock -= quantity;
        }

        if (customerId is not null && request.SaveAddress)
        {
            var hasDefault = await db.CustomerAddresses.AnyAsync(a => a.CustomerId == customerId && a.IsDefault);
            db.CustomerAddresses.Add(new CustomerAddress
            {
                CustomerId = customerId.Value,
                Label = string.IsNullOrWhiteSpace(request.AddressLabel) ? "Home" : request.AddressLabel.Trim(),
                FullName = order.CustomerName,
                Phone = order.Phone,
                Line1 = order.ShippingAddress,
                City = order.City,
                PostalCode = order.PostalCode,
                IsDefault = !hasDefault
            });
        }

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, MapOrder(order));
    }

    private int? GetCustomerId()
    {
        if (User?.Identity?.IsAuthenticated != true || !User.IsInRole("Customer"))
        {
            return null;
        }

        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return int.TryParse(raw, out var id) ? id : null;
    }

    private static OrderDto MapOrder(Order order) =>
        new(
            order.Id,
            order.CustomerName,
            order.Email,
            order.Phone,
            order.ShippingAddress,
            order.City,
            order.PostalCode,
            order.Status,
            order.Subtotal,
            order.ShippingCost,
            order.Total,
            order.CreatedAt,
            order.Items
                .Select(i => new OrderItemDto(
                    i.ProductId,
                    i.ProductName,
                    i.ProductNameFa,
                    i.UnitPrice,
                    i.Quantity,
                    i.UnitPrice * i.Quantity))
                .ToList());
}
