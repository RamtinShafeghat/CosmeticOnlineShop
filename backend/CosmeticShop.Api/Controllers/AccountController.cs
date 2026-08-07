using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CosmeticShop.Api.Data;
using CosmeticShop.Api.Dtos;
using CosmeticShop.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CosmeticShop.Api.Controllers;

[ApiController]
[Authorize(Roles = "Customer")]
[Route("api/account")]
public class AccountController(AppDbContext db) : ControllerBase
{
    [HttpGet("orders")]
    public async Task<ActionResult<IEnumerable<CustomerOrderListItemDto>>> GetOrders()
    {
        var customerId = GetCustomerId();
        if (customerId is null)
        {
            return Unauthorized();
        }

        var orders = await db.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new CustomerOrderListItemDto(
                o.Id,
                o.Status,
                o.Total,
                o.Items.Count,
                o.CreatedAt))
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("orders/{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var customerId = GetCustomerId();
        if (customerId is null)
        {
            return Unauthorized();
        }

        var order = await db.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id && o.CustomerId == customerId);

        return order is null ? NotFound() : Ok(MapOrder(order));
    }

    [HttpGet("addresses")]
    public async Task<ActionResult<IEnumerable<CustomerAddressDto>>> GetAddresses()
    {
        var customerId = GetCustomerId();
        if (customerId is null)
        {
            return Unauthorized();
        }

        var addresses = await db.CustomerAddresses
            .AsNoTracking()
            .Where(a => a.CustomerId == customerId)
            .OrderByDescending(a => a.IsDefault)
            .ThenBy(a => a.Id)
            .Select(a => MapAddress(a))
            .ToListAsync();

        return Ok(addresses);
    }

    [HttpPost("addresses")]
    public async Task<ActionResult<CustomerAddressDto>> CreateAddress([FromBody] UpsertCustomerAddressDto request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var customerId = GetCustomerId();
        if (customerId is null)
        {
            return Unauthorized();
        }

        if (request.IsDefault)
        {
            await ClearDefaultAddressesAsync(customerId.Value);
        }

        var address = new CustomerAddress
        {
            CustomerId = customerId.Value,
            Label = request.Label.Trim(),
            FullName = request.FullName.Trim(),
            Phone = request.Phone.Trim(),
            Line1 = request.Line1.Trim(),
            City = request.City.Trim(),
            PostalCode = request.PostalCode.Trim(),
            IsDefault = request.IsDefault
                || !await db.CustomerAddresses.AnyAsync(a => a.CustomerId == customerId)
        };

        db.CustomerAddresses.Add(address);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAddresses), MapAddress(address));
    }

    [HttpPut("addresses/{id:int}")]
    public async Task<ActionResult<CustomerAddressDto>> UpdateAddress(
        int id,
        [FromBody] UpsertCustomerAddressDto request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var customerId = GetCustomerId();
        if (customerId is null)
        {
            return Unauthorized();
        }

        var address = await db.CustomerAddresses
            .FirstOrDefaultAsync(a => a.Id == id && a.CustomerId == customerId);
        if (address is null)
        {
            return NotFound();
        }

        if (request.IsDefault)
        {
            await ClearDefaultAddressesAsync(customerId.Value);
        }

        address.Label = request.Label.Trim();
        address.FullName = request.FullName.Trim();
        address.Phone = request.Phone.Trim();
        address.Line1 = request.Line1.Trim();
        address.City = request.City.Trim();
        address.PostalCode = request.PostalCode.Trim();
        address.IsDefault = request.IsDefault;

        await db.SaveChangesAsync();
        return Ok(MapAddress(address));
    }

    [HttpDelete("addresses/{id:int}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        var customerId = GetCustomerId();
        if (customerId is null)
        {
            return Unauthorized();
        }

        var address = await db.CustomerAddresses
            .FirstOrDefaultAsync(a => a.Id == id && a.CustomerId == customerId);
        if (address is null)
        {
            return NotFound();
        }

        db.CustomerAddresses.Remove(address);
        await db.SaveChangesAsync();

        if (address.IsDefault)
        {
            var next = await db.CustomerAddresses
                .Where(a => a.CustomerId == customerId)
                .OrderBy(a => a.Id)
                .FirstOrDefaultAsync();
            if (next is not null)
            {
                next.IsDefault = true;
                await db.SaveChangesAsync();
            }
        }

        return NoContent();
    }

    [HttpGet("wishlist")]
    public async Task<ActionResult<IEnumerable<ProductListItemDto>>> GetWishlist()
    {
        var customerId = GetCustomerId();
        if (customerId is null)
        {
            return Unauthorized();
        }

        var rows = await db.WishlistItems
            .AsNoTracking()
            .Where(w => w.CustomerId == customerId)
            .OrderByDescending(w => w.CreatedAt)
            .ThenByDescending(w => w.Id)
            .Select(w => new
            {
                w.Product!.Id,
                w.Product.Name,
                w.Product.NameFa,
                w.Product.Slug,
                w.Product.ShortDescription,
                w.Product.ShortDescriptionFa,
                w.Product.Price,
                w.Product.ImageUrl,
                w.Product.Brand,
                w.Product.SkinType,
                w.Product.Stock,
                w.Product.IsFeatured,
                w.Product.CategoryId,
                CategoryName = w.Product.Category!.Name,
                CategoryNameFa = w.Product.Category!.NameFa,
                AverageRating = w.Product.Ratings.Average(r => (double?)r.Stars) ?? 0,
                RatingCount = w.Product.Ratings.Count()
            })
            .ToListAsync();

        var products = rows.Select(p => new ProductListItemDto(
            p.Id,
            p.Name,
            p.NameFa,
            p.Slug,
            p.ShortDescription,
            p.ShortDescriptionFa,
            p.Price,
            p.ImageUrl,
            p.Brand,
            p.SkinType,
            p.Stock,
            p.IsFeatured,
            p.CategoryId,
            p.CategoryName,
            p.CategoryNameFa,
            Math.Round(p.AverageRating, 1),
            p.RatingCount));

        return Ok(products);
    }

    [HttpPut("wishlist/{productId:int}")]
    public async Task<IActionResult> AddToWishlist(int productId)
    {
        var customerId = GetCustomerId();
        if (customerId is null)
        {
            return Unauthorized();
        }

        var productExists = await db.Products.AnyAsync(p => p.Id == productId);
        if (!productExists)
        {
            return NotFound();
        }

        var alreadySaved = await db.WishlistItems
            .AnyAsync(w => w.CustomerId == customerId && w.ProductId == productId);
        if (!alreadySaved)
        {
            db.WishlistItems.Add(new WishlistItem
            {
                CustomerId = customerId.Value,
                ProductId = productId,
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }

        return NoContent();
    }

    [HttpDelete("wishlist/{productId:int}")]
    public async Task<IActionResult> RemoveFromWishlist(int productId)
    {
        var customerId = GetCustomerId();
        if (customerId is null)
        {
            return Unauthorized();
        }

        var item = await db.WishlistItems
            .FirstOrDefaultAsync(w => w.CustomerId == customerId && w.ProductId == productId);
        if (item is not null)
        {
            db.WishlistItems.Remove(item);
            await db.SaveChangesAsync();
        }

        return NoContent();
    }

    private async Task ClearDefaultAddressesAsync(int customerId)
    {
        var defaults = await db.CustomerAddresses
            .Where(a => a.CustomerId == customerId && a.IsDefault)
            .ToListAsync();
        foreach (var item in defaults)
        {
            item.IsDefault = false;
        }
    }

    private int? GetCustomerId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return int.TryParse(raw, out var id) ? id : null;
    }

    private static CustomerAddressDto MapAddress(CustomerAddress address) =>
        new(
            address.Id,
            address.Label,
            address.FullName,
            address.Phone,
            address.Line1,
            address.City,
            address.PostalCode,
            address.IsDefault);

    private static OrderDto MapOrder(Order order) =>
        new(
            order.Id,
            order.PublicToken,
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
