using CosmeticShop.Api.Data;
using CosmeticShop.Api.Dtos;
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
            .Include(p => p.Category)
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
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                p.NameFa.Contains(search.Trim()) ||
                p.ShortDescription.ToLower().Contains(term) ||
                p.ShortDescriptionFa.Contains(search.Trim()) ||
                p.Brand.ToLower().Contains(term));
        }

        var products = await query
            .OrderByDescending(p => p.IsFeatured)
            .ThenBy(p => p.Name)
            .Select(p => new ProductListItemDto(
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
                p.Category!.Name,
                p.Category!.NameFa))
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDetailDto>> GetById(int id)
    {
        var product = await MapDetailQuery()
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();

        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ProductDetailDto>> GetBySlug(string slug)
    {
        var product = await MapDetailQuery()
            .Where(p => p.Slug == slug)
            .FirstOrDefaultAsync();

        return product is null ? NotFound() : Ok(product);
    }

    private IQueryable<ProductDetailDto> MapDetailQuery() =>
        db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Select(p => new ProductDetailDto(
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
                p.Category!.Name,
                p.Category!.NameFa));
}
