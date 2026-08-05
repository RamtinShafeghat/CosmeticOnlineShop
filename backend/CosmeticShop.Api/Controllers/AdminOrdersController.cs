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
    public async Task<ActionResult<IEnumerable<AdminOrderListItemDto>>> GetAll()
    {
        var orders = await db.Orders
            .AsNoTracking()
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

        var dto = new OrderDto(
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

        return Ok(dto);
    }
}
