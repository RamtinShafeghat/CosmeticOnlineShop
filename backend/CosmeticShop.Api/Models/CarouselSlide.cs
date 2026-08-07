namespace CosmeticShop.Api.Models;

public class CarouselSlide
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string TitleFa { get; set; } = string.Empty;
    public string LinkUrl { get; set; } = string.Empty;
    public int? ProductId { get; set; }
    public Product? Product { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
