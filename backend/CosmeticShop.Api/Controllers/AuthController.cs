using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CosmeticShop.Api;
using CosmeticShop.Api.Data;
using CosmeticShop.Api.Dtos;
using CosmeticShop.Api.Models;
using CosmeticShop.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CosmeticShop.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    AppDbContext db,
    JwtTokenService tokenService,
    IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<CustomerAuthResponse>> Register([FromBody] CustomerRegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Customers.AnyAsync(c => c.Email == email))
        {
            return Conflict(new { message = "An account with this email already exists." });
        }

        var customer = new Customer
        {
            Email = email,
            FullName = request.FullName.Trim(),
            Phone = request.Phone.Trim(),
            PasswordHash = PasswordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        db.Customers.Add(customer);
        await db.SaveChangesAsync();

        return Ok(BuildAuthResponse(customer));
    }

    [HttpPost("login")]
    public async Task<ActionResult<CustomerAuthResponse>> Login([FromBody] CustomerLoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var customer = await db.Customers.FirstOrDefaultAsync(c => c.Email == email);
        if (customer is null || !PasswordHasher.Verify(request.Password, customer.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(BuildAuthResponse(customer));
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("me")]
    public async Task<ActionResult<CustomerProfileDto>> Me()
    {
        var customerId = await GetCustomerIdAsync();
        if (customerId is null)
        {
            return Unauthorized();
        }

        var customer = await db.Customers.AsNoTracking().FirstAsync(c => c.Id == customerId.Value);
        return Ok(new CustomerProfileDto(customer.Id, customer.Email, customer.FullName, customer.Phone));
    }

    private CustomerAuthResponse BuildAuthResponse(Customer customer)
    {
        var token = tokenService.CreateCustomerToken(customer);
        var expires = DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpiresMinutes);
        return new CustomerAuthResponse(token, customer.Email, customer.FullName, customer.Phone, expires);
    }

    private async Task<int?> GetCustomerIdAsync()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!int.TryParse(raw, out var id))
        {
            return null;
        }

        return await db.Customers.AnyAsync(c => c.Id == id) ? id : null;
    }
}
