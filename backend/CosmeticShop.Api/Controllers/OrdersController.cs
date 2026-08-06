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

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        if (products.Count != productIds.Count)
        {
            return BadRequest(new { message = "One or more products were not found." });
        }

        foreach (var item in request.Items)
        {
            var product = products[item.ProductId];
            if (product.Stock < item.Quantity)
            {
                return BadRequest(new
                {
                    message = $"Insufficient stock for '{product.Name}'. Available: {product.Stock}."
                });
            }
        }

        var orderItems = request.Items.Select(item =>
        {
            var product = products[item.ProductId];
            return new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductNameFa = product.NameFa,
                UnitPrice = product.Price,
                Quantity = item.Quantity
            };
        }).ToList();

        var subtotal = orderItems.Sum(i => i.UnitPrice * i.Quantity);
        var shipping = subtotal >= FreeShippingThreshold ? 0m : StandardShipping;

        var order = new Order
        {
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

        foreach (var item in request.Items)
        {
            products[item.ProductId].Stock -= item.Quantity;
        }

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, MapOrder(order));
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
