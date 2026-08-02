namespace CosmeticShop.Api.Dtos;

public record ProductListItemDto(
    int Id,
    string Name,
    string NameFa,
    string Slug,
    string ShortDescription,
    string ShortDescriptionFa,
    decimal Price,
    string ImageUrl,
    string Brand,
    string SkinType,
    int Stock,
    bool IsFeatured,
    int CategoryId,
    string CategoryName,
    string CategoryNameFa);

public record ProductDetailDto(
    int Id,
    string Name,
    string NameFa,
    string Slug,
    string Description,
    string DescriptionFa,
    string ShortDescription,
    string ShortDescriptionFa,
    decimal Price,
    string ImageUrl,
    string Brand,
    string SkinType,
    int Stock,
    bool IsFeatured,
    int CategoryId,
    string CategoryName,
    string CategoryNameFa);

public record CategoryDto(
    int Id,
    string Name,
    string NameFa,
    string Slug,
    string Description,
    string DescriptionFa,
    int ProductCount);
