using CosmeticShop.Api.Data;
using CosmeticShop.Api.Dtos;
using CosmeticShop.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CosmeticShop.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/carousel-slides")]
public class AdminCarouselController(
    AppDbContext db,
    IWebHostEnvironment env) : ControllerBase
{
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarouselSlideDto>>> GetAll()
    {
        var slides = await db.CarouselSlides
            .AsNoTracking()
            .Include(s => s.Product)
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.Id)
            .ToListAsync();

        return Ok(slides.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CarouselSlideDto>> GetById(int id)
    {
        var slide = await db.CarouselSlides
            .AsNoTracking()
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.Id == id);

        return slide is null ? NotFound() : Ok(ToDto(slide));
    }

    [HttpPost]
    public async Task<ActionResult<CarouselSlideDto>> Create([FromBody] UpsertCarouselSlideRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (request.ProductId is not null && !await db.Products.AnyAsync(p => p.Id == request.ProductId))
        {
            return BadRequest(new { message = "Product not found." });
        }

        var slide = new CarouselSlide();
        Apply(slide, request);

        db.CarouselSlides.Add(slide);
        await db.SaveChangesAsync();

        await db.Entry(slide).Reference(s => s.Product).LoadAsync();
        return CreatedAtAction(nameof(GetById), new { id = slide.Id }, ToDto(slide));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CarouselSlideDto>> Update(int id, [FromBody] UpsertCarouselSlideRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var slide = await db.CarouselSlides.FirstOrDefaultAsync(s => s.Id == id);
        if (slide is null)
        {
            return NotFound();
        }

        if (request.ProductId is not null && !await db.Products.AnyAsync(p => p.Id == request.ProductId))
        {
            return BadRequest(new { message = "Product not found." });
        }

        Apply(slide, request);
        await db.SaveChangesAsync();

        await db.Entry(slide).Reference(s => s.Product).LoadAsync();
        return Ok(ToDto(slide));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var slide = await db.CarouselSlides.FirstOrDefaultAsync(s => s.Id == id);
        if (slide is null)
        {
            return NotFound();
        }

        db.CarouselSlides.Remove(slide);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:int}/image")]
    [RequestSizeLimit(5_000_000)]
    public async Task<ActionResult<UploadImageResponse>> UploadImage(int id, IFormFile file)
    {
        var slide = await db.CarouselSlides.FirstOrDefaultAsync(s => s.Id == id);
        if (slide is null)
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

        var uploadsRoot = Path.Combine(env.ContentRootPath, "wwwroot", "uploads", "carousel");
        Directory.CreateDirectory(uploadsRoot);

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var savePath = Path.Combine(uploadsRoot, fileName);

        await using (var stream = System.IO.File.Create(savePath))
        {
            await file.CopyToAsync(stream);
        }

        slide.ImageUrl = $"/uploads/carousel/{fileName}";
        await db.SaveChangesAsync();

        return Ok(new UploadImageResponse(slide.ImageUrl));
    }

    private static void Apply(CarouselSlide slide, UpsertCarouselSlideRequest request)
    {
        slide.Title = request.Title.Trim();
        slide.TitleFa = request.TitleFa.Trim();
        slide.LinkUrl = request.LinkUrl?.Trim() ?? string.Empty;
        slide.ProductId = request.ProductId;
        slide.SortOrder = request.SortOrder;
        slide.IsActive = request.IsActive;

        if (!string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            slide.ImageUrl = request.ImageUrl.Trim();
        }
        else if (string.IsNullOrWhiteSpace(slide.ImageUrl))
        {
            slide.ImageUrl = string.Empty;
        }
    }

    private static CarouselSlideDto ToDto(CarouselSlide s) => new(
        s.Id,
        s.ImageUrl,
        s.Title,
        s.TitleFa,
        s.LinkUrl,
        s.ProductId,
        s.Product?.Name,
        s.SortOrder,
        s.IsActive);
}
