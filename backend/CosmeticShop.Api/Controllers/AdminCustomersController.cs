using CosmeticShop.Api.Data;
using CosmeticShop.Api.Dtos;
using CosmeticShop.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CosmeticShop.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/customers")]
public class AdminCustomersController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminCustomerListItemDto>>> GetAll(
        [FromQuery] string? search = null)
    {
        var query = db.Customers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(c =>
                c.Email.ToLower().Contains(term)
                || c.FullName.ToLower().Contains(term)
                || c.Phone.ToLower().Contains(term));
        }

        // SQLite cannot aggregate decimals server-side, so sum as double and convert back.
        var customers = await query
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                c.FullName,
                c.Email,
                c.Phone,
                c.CreatedAt,
                OrderCount = c.Orders.Count,
                TotalSpent = c.Orders.Sum(o => (double?)o.Total) ?? 0d
            })
            .ToListAsync();

        return Ok(customers
            .Select(c => new AdminCustomerListItemDto(
                c.Id,
                c.FullName,
                c.Email,
                c.Phone,
                c.CreatedAt,
                c.OrderCount,
                (decimal)c.TotalSpent))
            .ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AdminCustomerDetailDto>> GetById(int id)
    {
        var customer = await db.Customers
            .AsNoTracking()
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer is null)
        {
            return NotFound();
        }

        var orders = await db.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == id)
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

        return Ok(new AdminCustomerDetailDto(
            customer.Id,
            customer.FullName,
            customer.Email,
            customer.Phone,
            customer.CreatedAt,
            orders.Count,
            orders.Sum(o => o.Total),
            customer.Addresses
                .OrderByDescending(a => a.IsDefault)
                .ThenBy(a => a.Id)
                .Select(a => new CustomerAddressDto(
                    a.Id,
                    a.Label,
                    a.FullName,
                    a.Phone,
                    a.Line1,
                    a.City,
                    a.PostalCode,
                    a.IsDefault))
                .ToList(),
            orders));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AdminCustomerDetailDto>> Update(
        int id,
        [FromBody] AdminUpdateCustomerRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer is null)
        {
            return NotFound();
        }

        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Customers.AnyAsync(c => c.Id != id && c.Email == email))
        {
            return Conflict(new { message = "Another account already uses this email." });
        }

        customer.FullName = request.FullName.Trim();
        customer.Email = email;
        customer.Phone = request.Phone.Trim();

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            customer.PasswordHash = PasswordHasher.Hash(request.NewPassword);
        }

        await db.SaveChangesAsync();

        return await GetById(id);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await db.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer is null)
        {
            return NotFound();
        }

        // Orders are kept for bookkeeping; the FK is configured as SetNull so
        // they simply become guest orders. Addresses and ratings cascade away.
        db.Customers.Remove(customer);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
