using System.ComponentModel.DataAnnotations;

namespace CosmeticShop.Api.Dtos;

public class CreateOrderItemDto
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, 99)]
    public int Quantity { get; set; }
}

public class CreateOrderDto
{
    [Required, MaxLength(120)]
    public string CustomerName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(40)]
    public string Phone { get; set; } = string.Empty;

    [Required, MaxLength(250)]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    public bool SaveAddress { get; set; }

    [MaxLength(60)]
    public string? AddressLabel { get; set; }

    [Required, MinLength(1)]
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public record OrderItemDto(
    int ProductId,
    string ProductName,
    string ProductNameFa,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);

public record OrderDto(
    int Id,
    string CustomerName,
    string Email,
    string Phone,
    string ShippingAddress,
    string City,
    string PostalCode,
    string Status,
    decimal Subtotal,
    decimal ShippingCost,
    decimal Total,
    DateTime CreatedAt,
    IReadOnlyList<OrderItemDto> Items);
