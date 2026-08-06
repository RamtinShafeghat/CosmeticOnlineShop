using CosmeticShop.Api.Data;
using CosmeticShop.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CosmeticShop.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/orders")]
public class AdminOrdersController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminOrderListItemDto>>> GetAll(
        [FromQuery] string? status = null)
    {
        var query = db.Orders.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalized = status.Trim();
            query = query.Where(o => o.Status == normalized);
        }

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new AdminOrderListItemDto(
                o.Id,
                o.CustomerName,
                o.Email,
                o.Status,
                o.Total,
                o.Items.Count,
                o.CreatedAt))
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await db.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(MapOrder(order));
    }

    [HttpPost("{id:int}/confirm")]
    public async Task<ActionResult<OrderDto>> Confirm(int id)
    {
        var order = await db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            return NotFound();
        }

        if (order.Status == "Confirmed")
        {
            return Ok(MapOrder(order));
        }

        if (order.Status != "Pending")
        {
            return BadRequest(new { message = $"Order cannot be confirmed from status '{order.Status}'." });
        }

        order.Status = "Confirmed";
        await db.SaveChangesAsync();

        return Ok(MapOrder(order));
    }

    private static OrderDto MapOrder(Models.Order order) =>
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
