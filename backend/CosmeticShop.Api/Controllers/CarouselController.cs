using CosmeticShop.Api.Data;
using CosmeticShop.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CosmeticShop.Api.Controllers;

[ApiController]
[Route("api/carousel-slides")]
public class CarouselController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PublicCarouselSlideDto>>> GetActive()
    {
        var rows = await db.CarouselSlides
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.Id)
            .Select(s => new
            {
                s.Id,
                s.ImageUrl,
                s.Title,
                s.TitleFa,
                s.LinkUrl,
                ProductSlug = s.Product != null ? s.Product.Slug : null
            })
            .ToListAsync();

        var slides = rows.Select(s => new PublicCarouselSlideDto(
            s.Id,
            s.ImageUrl,
            s.Title,
            s.TitleFa,
            !string.IsNullOrWhiteSpace(s.LinkUrl)
                ? s.LinkUrl
                : s.ProductSlug is not null ? $"/product/{s.ProductSlug}" : null));

        return Ok(slides);
    }
}
