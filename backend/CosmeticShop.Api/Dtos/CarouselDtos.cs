using System.ComponentModel.DataAnnotations;

namespace CosmeticShop.Api.Dtos;

public record CarouselSlideDto(
    int Id,
    string ImageUrl,
    string Title,
    string TitleFa,
    string LinkUrl,
    int? ProductId,
    string? ProductName,
    int SortOrder,
    bool IsActive);

public record PublicCarouselSlideDto(
    int Id,
    string ImageUrl,
    string Title,
    string TitleFa,
    string? Link);

public class UpsertCarouselSlideRequest
{
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(160)]
    public string TitleFa { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(300)]
    public string? LinkUrl { get; set; }

    public int? ProductId { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
