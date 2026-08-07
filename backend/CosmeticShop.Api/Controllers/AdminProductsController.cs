using CosmeticShop.Api.Data;
using CosmeticShop.Api.Dtos;
using CosmeticShop.Api.Models;
using CosmeticShop.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CosmeticShop.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/products")]
public class AdminProductsController(
    AppDbContext db,
    IWebHostEnvironment env) : ControllerBase
{
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductListItemDto>>> GetAll([FromQuery] int? categoryId)
    {
        var query = db.Products.AsNoTracking().Include(p => p.Category).Include(p => p.Ratings).AsQueryable();
        if (categoryId is not null)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        var products = await query
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        return Ok(products.Select(MapListItem));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDetailDto>> GetById(int id)
    {
        var product = await db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Ratings)
            .FirstOrDefaultAsync(p => p.Id == id);

        return product is null ? NotFound() : Ok(MapDetail(product));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDetailDto>> Create([FromBody] UpsertProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!await db.Categories.AnyAsync(c => c.Id == request.CategoryId))
        {
            return BadRequest(new { message = "Category not found." });
        }

        var product = new Product();
        Apply(product, request, applyStock: true);
        product.Slug = await EnsureUniqueSlugAsync(SlugHelper.ToSlug(request.Slug ?? request.Name));

        db.Products.Add(product);
        await db.SaveChangesAsync();

        await db.Entry(product).Reference(p => p.Category).LoadAsync();
        await db.Entry(product).Collection(p => p.Ratings).LoadAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, MapDetail(product));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDetailDto>> Update(int id, [FromBody] UpsertProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var product = await db.Products
            .Include(p => p.Category)
            .Include(p => p.Ratings)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        if (!await db.Categories.AnyAsync(c => c.Id == request.CategoryId))
        {
            return BadRequest(new { message = "Category not found." });
        }

        // Do not apply request.Stock here. The admin edit form often still holds the
        // stock value from page load; writing it absolutely would restore units already
        // sold by concurrent checkout (ExecuteUpdate). Intentional stock changes go
        // through UpdateStock.
        Apply(product, request, applyStock: false);
        product.Slug = await EnsureUniqueSlugAsync(
            SlugHelper.ToSlug(request.Slug ?? request.Name),
            product.Id);

        await db.SaveChangesAsync();
        await db.Entry(product).Reference(p => p.Category).LoadAsync();
        return Ok(MapDetail(product));
    }

    [HttpPut("{id:int}/stock")]
    public async Task<ActionResult<ProductDetailDto>> UpdateStock(
        int id,
        [FromBody] UpdateProductStockRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var stock = request.Stock!.Value;

        // Compare-and-swap on ExpectedStock so a stale admin form cannot restore
        // units already reserved by concurrent checkout (ExecuteUpdate).
        var affected = await db.Products
            .Where(p => p.Id == id && p.Stock == request.ExpectedStock)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(p => p.Stock, stock));

        if (affected == 0)
        {
            var exists = await db.Products.AnyAsync(p => p.Id == id);
            if (!exists)
            {
                return NotFound();
            }

            return Conflict(new
            {
                message = "Stock was changed by another operation. Reload the product and try again."
            });
        }

        var product = await db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Ratings)
            .FirstAsync(p => p.Id == id);

        return Ok(MapDetail(product));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        var inOrders = await db.OrderItems.AnyAsync(i => i.ProductId == id);
        if (inOrders)
        {
            return BadRequest(new { message = "Cannot delete a product that appears in existing orders." });
        }

        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:int}/image")]
    [RequestSizeLimit(5_000_000)]
    public async Task<ActionResult<UploadImageResponse>> UploadImage(int id, IFormFile file)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        if (file is null || file.Length == 0)
        {
            return BadRequest(new { message = "Please choose an image file." });
        }

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedImageExtensions.Contains(extension))
        {
            return BadRequest(new { message = "Only JPG, PNG, WEBP, and GIF images are allowed." });
        }

        if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Uploaded file must be an image." });
        }

        var uploadsRoot = Path.Combine(env.ContentRootPath, "wwwroot", "uploads", "products");
        Directory.CreateDirectory(uploadsRoot);

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var savePath = Path.Combine(uploadsRoot, fileName);

        await using (var stream = System.IO.File.Create(savePath))
        {
            await file.CopyToAsync(stream);
        }

        product.ImageUrl = $"/uploads/products/{fileName}";
        await db.SaveChangesAsync();

        return Ok(new UploadImageResponse(product.ImageUrl));
    }

    private static void Apply(Product product, UpsertProductRequest request, bool applyStock)
    {
        product.Name = request.Name.Trim();
        product.NameFa = request.NameFa.Trim();
        product.Description = request.Description.Trim();
        product.DescriptionFa = request.DescriptionFa.Trim();
        product.ShortDescription = request.ShortDescription.Trim();
        product.ShortDescriptionFa = request.ShortDescriptionFa.Trim();
        product.Price = request.Price;
        product.Brand = string.IsNullOrWhiteSpace(request.Brand) ? "Velora" : request.Brand.Trim();
        product.SkinType = string.IsNullOrWhiteSpace(request.SkinType) ? "All" : request.SkinType.Trim();
        if (applyStock)
        {
            product.Stock = request.Stock;
        }

        product.IsFeatured = request.IsFeatured;
        product.CategoryId = request.CategoryId;

        if (!string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            product.ImageUrl = request.ImageUrl.Trim();
        }
        else if (string.IsNullOrWhiteSpace(product.ImageUrl))
        {
            product.ImageUrl = string.Empty;
        }
    }

    private async Task<string> EnsureUniqueSlugAsync(string slug, int? excludeId = null)
    {
        var baseSlug = slug;
        var suffix = 2;
        while (await db.Products.AnyAsync(p => p.Slug == slug && p.Id != excludeId))
        {
            slug = $"{baseSlug}-{suffix++}";
        }

        return slug;
    }

    private static ProductListItemDto MapListItem(Product p)
    {
        var ratingCount = p.Ratings?.Count ?? 0;
        var average = ratingCount > 0
            ? Math.Round(p.Ratings!.Average(r => r.Stars), 1)
            : 0;

        return new(
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
            p.Category?.Name ?? string.Empty,
            p.Category?.NameFa ?? string.Empty,
            average,
            ratingCount);
    }

    private static ProductDetailDto MapDetail(Product p)
    {
        var ratingCount = p.Ratings?.Count ?? 0;
        var average = ratingCount > 0
            ? Math.Round(p.Ratings!.Average(r => r.Stars), 1)
            : 0;

        return new(
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
            p.Category?.Name ?? string.Empty,
            p.Category?.NameFa ?? string.Empty,
            average,
            ratingCount,
            null);
    }
}
