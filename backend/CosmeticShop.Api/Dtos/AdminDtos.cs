using System.ComponentModel.DataAnnotations;

namespace CosmeticShop.Api.Dtos;

public class AdminLoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public record AdminLoginResponse(
    string Token,
    string Email,
    string DisplayName,
    DateTime ExpiresAtUtc);

public class UpsertCategoryRequest
{
    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(120)]
    public string NameFa { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? Slug { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500)]
    public string DescriptionFa { get; set; } = string.Empty;
}

public class UpsertProductRequest
{
    [Required, MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(160)]
    public string NameFa { get; set; } = string.Empty;

    [MaxLength(160)]
    public string? Slug { get; set; }

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string DescriptionFa { get; set; } = string.Empty;

    [MaxLength(400)]
    public string ShortDescription { get; set; } = string.Empty;

    [MaxLength(400)]
    public string ShortDescriptionFa { get; set; } = string.Empty;

    [Range(0.01, 100000)]
    public decimal Price { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(80)]
    public string Brand { get; set; } = "Velora";

    [MaxLength(40)]
    public string SkinType { get; set; } = "All";

    [Range(0, 100000)]
    public int Stock { get; set; }

    public bool IsFeatured { get; set; }

    [Required]
    public int CategoryId { get; set; }
}

public record AdminOrderListItemDto(
    int Id,
    string CustomerName,
    string Email,
    string Status,
    decimal Total,
    int ItemCount,
    DateTime CreatedAt);

public record UploadImageResponse(string ImageUrl);

public class UpdateProductStockRequest
{
    [Range(0, 100000)]
    public int Stock { get; set; }
}
