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
[Route("api/admin/categories")]
public class AdminCategoriesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.NameFa,
                c.Slug,
                c.Description,
                c.DescriptionFa,
                c.Products.Count))
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var category = await db.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.NameFa,
                c.Slug,
                c.Description,
                c.DescriptionFa,
                c.Products.Count))
            .FirstOrDefaultAsync();

        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] UpsertCategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var slug = await EnsureUniqueSlugAsync(SlugHelper.ToSlug(request.Slug ?? request.Name));
        var category = new Category
        {
            Name = request.Name.Trim(),
            NameFa = request.NameFa.Trim(),
            Slug = slug,
            Description = request.Description.Trim(),
            DescriptionFa = request.DescriptionFa.Trim()
        };

        db.Categories.Add(category);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, ToDto(category, 0));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] UpsertCategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var category = await db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
        if (category is null)
        {
            return NotFound();
        }

        category.Name = request.Name.Trim();
        category.NameFa = request.NameFa.Trim();
        category.Description = request.Description.Trim();
        category.DescriptionFa = request.DescriptionFa.Trim();
        category.Slug = await EnsureUniqueSlugAsync(
            SlugHelper.ToSlug(request.Slug ?? request.Name),
            category.Id);

        await db.SaveChangesAsync();
        return Ok(ToDto(category, category.Products.Count));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await db.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
        {
            return NotFound();
        }

        if (category.Products.Count > 0)
        {
            return BadRequest(new { message = "Cannot delete a category that still has products." });
        }

        db.Categories.Remove(category);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<string> EnsureUniqueSlugAsync(string slug, int? excludeId = null)
    {
        var baseSlug = slug;
        var suffix = 2;
        while (await db.Categories.AnyAsync(c => c.Slug == slug && c.Id != excludeId))
        {
            slug = $"{baseSlug}-{suffix++}";
        }

        return slug;
    }

    private static CategoryDto ToDto(Category category, int productCount) =>
        new(
            category.Id,
            category.Name,
            category.NameFa,
            category.Slug,
            category.Description,
            category.DescriptionFa,
            productCount);
}
