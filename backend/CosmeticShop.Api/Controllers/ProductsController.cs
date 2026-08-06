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
[Route("api/[controller]")]
public class ProductsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductListItemDto>>> GetProducts(
        [FromQuery] int? categoryId,
        [FromQuery] string? search,
        [FromQuery] bool? featured)
    {
        var query = db.Products
            .AsNoTracking()
            .AsQueryable();

        if (categoryId is not null)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        if (featured is true)
        {
            query = query.Where(p => p.IsFeatured);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            var original = search.Trim();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                p.NameFa.Contains(original) ||
                p.ShortDescription.ToLower().Contains(term) ||
                p.ShortDescriptionFa.Contains(original) ||
                p.Brand.ToLower().Contains(term));
        }

        var rows = await query
            .OrderByDescending(p => p.IsFeatured)
            .ThenBy(p => p.Name)
            .Select(p => new
            {
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
                CategoryName = p.Category!.Name,
                CategoryNameFa = p.Category!.NameFa,
                AverageRating = p.Ratings.Average(r => (double?)r.Stars) ?? 0,
                RatingCount = p.Ratings.Count()
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDetailDto>> GetById(int id)
    {
        var product = await LoadDetailAsync(id, bySlug: null);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ProductDetailDto>> GetBySlug(string slug)
    {
        var product = await LoadDetailAsync(null, slug);
        return product is null ? NotFound() : Ok(product);
    }

    [Authorize(Roles = "Customer")]
    [HttpPut("{id:int}/rating")]
    public async Task<ActionResult<ProductRatingSummaryDto>> UpsertRating(
        int id,
        [FromBody] UpsertProductRatingDto request)
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

        var productExists = await db.Products.AnyAsync(p => p.Id == id);
        if (!productExists)
        {
            return NotFound();
        }

        var rating = await db.ProductRatings
            .FirstOrDefaultAsync(r => r.ProductId == id && r.CustomerId == customerId);

        if (rating is null)
        {
            rating = new ProductRating
            {
                ProductId = id,
                CustomerId = customerId.Value,
                Stars = request.Stars,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.ProductRatings.Add(rating);
        }
        else
        {
            rating.Stars = request.Stars;
            rating.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();

        var stats = await db.ProductRatings
            .AsNoTracking()
            .Where(r => r.ProductId == id)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Average = g.Average(r => (double)r.Stars),
                Count = g.Count()
            })
            .FirstOrDefaultAsync();

        return Ok(new ProductRatingSummaryDto(
            id,
            Math.Round(stats?.Average ?? request.Stars, 1),
            stats?.Count ?? 1,
            request.Stars));
    }

    private async Task<ProductDetailDto?> LoadDetailAsync(int? id, string? bySlug)
    {
        var query = db.Products.AsNoTracking().AsQueryable();
        if (id is not null)
        {
            query = query.Where(p => p.Id == id);
        }
        else if (!string.IsNullOrWhiteSpace(bySlug))
        {
            query = query.Where(p => p.Slug == bySlug);
        }
        else
        {
            return null;
        }

        var row = await query
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.NameFa,
                p.Slug,
                p.Description,
                p.DescriptionFa,
                p.ShortDescription,
                p.ShortDescriptionFa,
                p.Price,
                p.ImageUrl,
                p.Brand,
                p.SkinType,
                p.Stock,
                p.IsFeatured,
                p.CategoryId,
                CategoryName = p.Category!.Name,
                CategoryNameFa = p.Category!.NameFa,
                AverageRating = p.Ratings.Average(r => (double?)r.Stars) ?? 0,
                RatingCount = p.Ratings.Count()
            })
            .FirstOrDefaultAsync();

        if (row is null)
        {
            return null;
        }

        int? myRating = null;
        var customerId = GetCustomerId();
        if (customerId is not null)
        {
            myRating = await db.ProductRatings
                .AsNoTracking()
                .Where(r => r.ProductId == row.Id && r.CustomerId == customerId)
                .Select(r => (int?)r.Stars)
                .FirstOrDefaultAsync();
        }

        return new ProductDetailDto(
            row.Id,
            row.Name,
            row.NameFa,
            row.Slug,
            row.Description,
            row.DescriptionFa,
            row.ShortDescription,
            row.ShortDescriptionFa,
            row.Price,
            row.ImageUrl,
            row.Brand,
            row.SkinType,
            row.Stock,
            row.IsFeatured,
            row.CategoryId,
            row.CategoryName,
            row.CategoryNameFa,
            Math.Round(row.AverageRating, 1),
            row.RatingCount,
            myRating);
    }

    private int? GetCustomerId()
    {
        if (User?.Identity?.IsAuthenticated != true || !User.IsInRole("Customer"))
        {
            return null;
        }

        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return int.TryParse(raw, out var parsed) ? parsed : null;
    }
}
