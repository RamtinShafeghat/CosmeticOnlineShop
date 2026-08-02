namespace CosmeticShop.Api.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameFa { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionFa { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string ShortDescriptionFa { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Brand { get; set; } = "Velora";
    public string SkinType { get; set; } = "All";
    public int Stock { get; set; }
    public bool IsFeatured { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
