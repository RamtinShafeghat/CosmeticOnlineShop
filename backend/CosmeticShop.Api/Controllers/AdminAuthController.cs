using CosmeticShop.Api;
using CosmeticShop.Api.Data;
using CosmeticShop.Api.Dtos;
using CosmeticShop.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CosmeticShop.Api.Controllers;

[ApiController]
[Route("api/admin/auth")]
public class AdminAuthController(
    AppDbContext db,
    JwtTokenService tokenService,
    IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AdminLoginResponse>> Login([FromBody] AdminLoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.AdminUsers.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var token = tokenService.CreateToken(user);
        var expires = DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpiresMinutes);
        return Ok(new AdminLoginResponse(token, user.Email, user.DisplayName, expires));
    }
}
