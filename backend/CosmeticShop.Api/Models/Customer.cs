namespace CosmeticShop.Api.Models;

public class Customer
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<ProductRating> Ratings { get; set; } = new List<ProductRating>();
    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
}
