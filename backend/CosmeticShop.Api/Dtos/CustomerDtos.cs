using System.ComponentModel.DataAnnotations;

namespace CosmeticShop.Api.Dtos;

public class CustomerRegisterRequest
{
    [Required, MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(40)]
    public string Phone { get; set; } = string.Empty;

    [Required, MinLength(6), MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}

public class CustomerLoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public record CustomerAuthResponse(
    string Token,
    string Email,
    string FullName,
    string Phone,
    DateTime ExpiresAtUtc);

public record CustomerProfileDto(
    int Id,
    string Email,
    string FullName,
    string Phone);

public class UpsertCustomerAddressDto
{
    [Required, MaxLength(60)]
    public string Label { get; set; } = "Home";

    [Required, MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(40)]
    public string Phone { get; set; } = string.Empty;

    [Required, MaxLength(250)]
    public string Line1 { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
}

public record CustomerAddressDto(
    int Id,
    string Label,
    string FullName,
    string Phone,
    string Line1,
    string City,
    string PostalCode,
    bool IsDefault);

public record CustomerOrderListItemDto(
    int Id,
    string Status,
    decimal Total,
    int ItemCount,
    DateTime CreatedAt);
