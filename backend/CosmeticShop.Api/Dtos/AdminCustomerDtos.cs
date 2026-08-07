using System.ComponentModel.DataAnnotations;

namespace CosmeticShop.Api.Dtos;

public record AdminCustomerListItemDto(
    int Id,
    string FullName,
    string Email,
    string Phone,
    DateTime CreatedAt,
    int OrderCount,
    decimal TotalSpent);

public record AdminCustomerDetailDto(
    int Id,
    string FullName,
    string Email,
    string Phone,
    DateTime CreatedAt,
    int OrderCount,
    decimal TotalSpent,
    IReadOnlyList<CustomerAddressDto> Addresses,
    IReadOnlyList<AdminOrderListItemDto> Orders);

public class AdminUpdateCustomerRequest
{
    [Required, MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(40)]
    public string Phone { get; set; } = string.Empty;

    [MinLength(6), MaxLength(100)]
    public string? NewPassword { get; set; }
}
