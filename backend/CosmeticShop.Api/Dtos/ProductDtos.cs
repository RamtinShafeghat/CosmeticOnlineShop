namespace CosmeticShop.Api.Dtos;

public record ProductListItemDto(
    int Id,
    string Name,
    string Slug,
    string ShortDescription,
    decimal Price,
    string ImageUrl,
    string Brand,
    string SkinType,
    int Stock,
    bool IsFeatured,
    int CategoryId,
    string CategoryName);

public record ProductDetailDto(
    int Id,
    string Name,
    string Slug,
    string Description,
    string ShortDescription,
    decimal Price,
    string ImageUrl,
    string Brand,
    string SkinType,
    int Stock,
    bool IsFeatured,
    int CategoryId,
    string CategoryName);

public record CategoryDto(
    int Id,
    string Name,
    string Slug,
    string Description,
    int ProductCount);
