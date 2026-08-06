using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CosmeticShop.Api;
using CosmeticShop.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CosmeticShop.Api.Services;

public class JwtTokenService(IOptions<JwtOptions> options)
{
    private readonly JwtOptions _options = options.Value;

    public string CreateToken(AdminUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Role, "Admin")
        };

        return WriteToken(claims);
    }

    public string CreateCustomerToken(Customer customer)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
            new(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, customer.Email),
            new(ClaimTypes.Name, customer.FullName),
            new(ClaimTypes.Role, "Customer")
        };

        return WriteToken(claims);
    }

    private string WriteToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_options.ExpiresMinutes);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
