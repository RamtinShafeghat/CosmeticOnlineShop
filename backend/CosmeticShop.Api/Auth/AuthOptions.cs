namespace CosmeticShop.Api;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = "VeloraDevSuperSecretKey_ChangeMe_32chars!";
    public string Issuer { get; set; } = "Velora";
    public string Audience { get; set; } = "VeloraAdmin";
    public int ExpiresMinutes { get; set; } = 480;
}

public class AdminSeedOptions
{
    public const string SectionName = "AdminSeed";

    public string Email { get; set; } = "admin@velora.com";
    public string Password { get; set; } = "Admin123!";
    public string DisplayName { get; set; } = "Velora Admin";
}
