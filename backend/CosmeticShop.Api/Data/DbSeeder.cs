using System.Data;
using System.Data.Common;
using CosmeticShop.Api;
using CosmeticShop.Api.Models;
using CosmeticShop.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace CosmeticShop.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, AdminSeedOptions adminSeed)
    {
        // EnsureCreated only builds a brand-new DB. Older SQLite files need additive
        // upgrades for customer auth tables/columns without wiping catalog data.
        await EnsureCompatibleSchemaAsync(db);

        await EnsureAdminUserAsync(db, adminSeed);

        if (await db.Products.AnyAsync())
        {
            return;
        }

        var categories = new List<Category>
        {
            new()
            {
                Name = "Skincare",
                NameFa = "مراقبت پوست",
                Slug = "skincare",
                Description = "Serums, moisturizers, and daily essentials for luminous skin.",
                DescriptionFa = "سرم‌ها، مرطوب‌کننده‌ها و ضروریات روزانه برای پوستی درخشان."
            },
            new()
            {
                Name = "Makeup",
                NameFa = "آرایش",
                Slug = "makeup",
                Description = "Color and complexion pieces with a soft, modern finish.",
                DescriptionFa = "محصولات رنگ و پوشش با پایانی نرم و مدرن."
            },
            new()
            {
                Name = "Fragrance",
                NameFa = "عطر",
                Slug = "fragrance",
                Description = "Light, memorable scents crafted for everyday wear.",
                DescriptionFa = "رایحه‌های سبک و ماندگار برای استفاده روزانه."
            },
            new()
            {
                Name = "Body Care",
                NameFa = "مراقبت بدن",
                Slug = "body-care",
                Description = "Nourishing body rituals for soft, hydrated skin.",
                DescriptionFa = "مراقبت‌های مغذی بدن برای پوستی نرم و آبرسانی‌شده."
            }
        };

        db.Categories.AddRange(categories);
        await db.SaveChangesAsync();

        var products = new List<Product>
        {
            new()
            {
                Name = "Dewdrop Hydrating Serum",
                NameFa = "سرم آبرسان شبنم",
                Slug = "dewdrop-hydrating-serum",
                ShortDescription = "A weightless hyaluronic serum for all-day moisture.",
                ShortDescriptionFa = "سرم سبک هیالورونیک برای آبرسانی تمام‌روز.",
                Description = "Dewdrop Hydrating Serum layers silky hydration with multi-weight hyaluronic acid and aloe. Use morning and night under moisturizer for a dewy, plump finish without stickiness.",
                DescriptionFa = "سرم آبرسان شبنم با هیالورونیک اسید چندوزنه و آلوئه‌ورا رطوبتی ابریشمی می‌بخشد. صبح و شب زیر مرطوب‌کننده استفاده کنید تا پوستی شاداب و بدون چسبندگی داشته باشید.",
                Price = 38.00m,
                ImageUrl = "https://images.unsplash.com/photo-1620916567454-8a8a0c2f1f0a?auto=format&fit=crop&w=900&q=80",
                Brand = "Velora",
                SkinType = "All",
                Stock = 48,
                IsFeatured = true,
                CategoryId = categories[0].Id
            },
            new()
            {
                Name = "Velvet Rose Moisturizer",
                NameFa = "مرطوب‌کننده رز مخملی",
                Slug = "velvet-rose-moisturizer",
                ShortDescription = "Creamy daily moisture with rosehip and ceramides.",
                ShortDescriptionFa = "مرطوب‌کننده کرمی روزانه با رز هیپ و سرامید.",
                Description = "Velvet Rose Moisturizer restores the skin barrier with ceramides and rosehip oil. Soft, cushiony texture melts in and leaves skin calm, smooth, and softly scented.",
                DescriptionFa = "مرطوب‌کننده رز مخملی سد دفاعی پوست را با سرامید و روغن رز هیپ ترمیم می‌کند. بافت نرم آن جذب می‌شود و پوست را آرام، صاف و با رایحه‌ای ملایم می‌گذارد.",
                Price = 42.00m,
                ImageUrl = "https://images.unsplash.com/photo-1556228720-195a672e8a03?auto=format&fit=crop&w=900&q=80",
                Brand = "Velora",
                SkinType = "Dry",
                Stock = 36,
                IsFeatured = true,
                CategoryId = categories[0].Id
            },
            new()
            {
                Name = "Sunrise Vitamin C Cream",
                NameFa = "کرم ویتامین C طلوع",
                Slug = "sunrise-vitamin-c-cream",
                ShortDescription = "Brightening day cream with stable vitamin C.",
                ShortDescriptionFa = "کرم روز روشن‌کننده با ویتامین C پایدار.",
                Description = "Sunrise Vitamin C Cream supports a more even-looking complexion with 15% vitamin C and niacinamide. Lightweight enough for layering under SPF and makeup.",
                DescriptionFa = "کرم ویتامین C طلوع با ۱۵٪ ویتامین C و نیاسینامید به یکنواختی پوست کمک می‌کند. سبک است و زیر ضدآفتاب و آرایش به‌خوبی لایه‌بندی می‌شود.",
                Price = 46.00m,
                ImageUrl = "https://images.unsplash.com/photo-1571781926291-c477ebfd024b?auto=format&fit=crop&w=900&q=80",
                Brand = "Velora",
                SkinType = "Normal",
                Stock = 40,
                IsFeatured = false,
                CategoryId = categories[0].Id
            },
            new()
            {
                Name = "Soft Focus Foundation",
                NameFa = "کرم‌پودر فوکوس نرم",
                Slug = "soft-focus-foundation",
                ShortDescription = "Buildable medium coverage with a natural satin finish.",
                ShortDescriptionFa = "پوشش متوسط قابل لایه‌بندی با پایان ساتن طبیعی.",
                Description = "Soft Focus Foundation blends seamlessly for medium coverage that looks like skin. Infused with light-reflecting pigments and skincare oils for a breathable all-day wear.",
                DescriptionFa = "کرم‌پودر فوکوس نرم برای پوششی متوسط که شبیه پوست به نظر می‌رسد یکدست می‌شود. با رنگدانه‌های بازتاب‌دهنده نور و روغن‌های مراقبتی برای ماندگاری تمام‌روز تنفس‌پذیر است.",
                Price = 34.00m,
                ImageUrl = "https://images.unsplash.com/photo-1596462502278-27bfdd403348?auto=format&fit=crop&w=900&q=80",
                Brand = "Velora",
                SkinType = "All",
                Stock = 55,
                IsFeatured = true,
                CategoryId = categories[1].Id
            },
            new()
            {
                Name = "Bloom Blush Stick",
                NameFa = "استیک رژگونه شکوفه",
                Slug = "bloom-blush-stick",
                ShortDescription = "Cream blush that melts into a natural flush.",
                ShortDescriptionFa = "رژگونه کرمی که به سرخی طبیعی تبدیل می‌شود.",
                Description = "Bloom Blush Stick delivers a soft petal flush in one swipe. The creamy formula blends with fingers and works beautifully alone or over foundation.",
                DescriptionFa = "استیک رژگونه شکوفه با یک حرکت سرخی ملایم گلبرگی می‌دهد. فرمول کرمی با انگشت پخش می‌شود و به‌تنهایی یا روی کرم‌پودر عالی است.",
                Price = 24.00m,
                ImageUrl = "https://images.unsplash.com/photo-1512496015851-a90fb38ba796?auto=format&fit=crop&w=900&q=80",
                Brand = "Velora",
                SkinType = "All",
                Stock = 62,
                IsFeatured = true,
                CategoryId = categories[1].Id
            },
            new()
            {
                Name = "Lash Poetry Mascara",
                NameFa = "ریمل شعر مژه",
                Slug = "lash-poetry-mascara",
                ShortDescription = "Lengthening mascara with a soft, flexible hold.",
                ShortDescriptionFa = "ریمل بلندکننده با ماندگاری نرم و انعطاف‌پذیر.",
                Description = "Lash Poetry Mascara lifts and lengthens without clumping. The tapered brush reaches every lash for defined, wearable drama from day to night.",
                DescriptionFa = "ریمل شعر مژه بدون گلوله شدن مژه‌ها را بلند و بلندتر می‌کند. برس مخروطی به همه مژه‌ها می‌رسد تا جلوه‌ای مشخص و قابل استفاده از روز تا شب بسازد.",
                Price = 22.00m,
                ImageUrl = "https://images.unsplash.com/photo-1631214524020-7e18db9a8f92?auto=format&fit=crop&w=900&q=80",
                Brand = "Velora",
                SkinType = "All",
                Stock = 70,
                IsFeatured = false,
                CategoryId = categories[1].Id
            },
            new()
            {
                Name = "Petal Mist Eau de Parfum",
                NameFa = "ادوپرفیوم غبار گلبرگ",
                Slug = "petal-mist-eau-de-parfum",
                ShortDescription = "A soft floral with pear, peony, and warm musk.",
                ShortDescriptionFa = "رایحه گل‌دار ملایم با گلابی، پیونی و مشک گرم.",
                Description = "Petal Mist opens with crisp pear, blooms into peony and jasmine, and settles into soft musk. An everyday fragrance that feels clean, luminous, and quietly memorable.",
                DescriptionFa = "غبار گلبرگ با گلابی تازه آغاز می‌شود، به پیونی و یاسمن می‌رسد و با مشک نرم تمام می‌شود. عطری روزانه که پاک، درخشان و به‌آرامی ماندگار است.",
                Price = 68.00m,
                ImageUrl = "https://images.unsplash.com/photo-1541643600914-78b084683601?auto=format&fit=crop&w=900&q=80",
                Brand = "Velora",
                SkinType = "All",
                Stock = 28,
                IsFeatured = true,
                CategoryId = categories[2].Id
            },
            new()
            {
                Name = "Citrus Veil Body Oil",
                NameFa = "روغن بدن پرده مرکبات",
                Slug = "citrus-veil-body-oil",
                ShortDescription = "Fast-absorbing oil with bergamot and jojoba.",
                ShortDescriptionFa = "روغن زودجذب با برگاموت و جوجوبا.",
                Description = "Citrus Veil Body Oil leaves skin satin-soft with a sheer bergamot scent. Massage onto damp skin after showering for a luminous finish that never feels greasy.",
                DescriptionFa = "روغن بدن پرده مرکبات پوست را ساتن‌نرم و با رایحه ملایم برگاموت می‌گذارد. پس از دوش روی پوست مرطوب ماساژ دهید تا پایانی درخشان و بدون چربی داشته باشید.",
                Price = 32.00m,
                ImageUrl = "https://images.unsplash.com/photo-1608248543803-ba4f8c70ae0b?auto=format&fit=crop&w=900&q=80",
                Brand = "Velora",
                SkinType = "Dry",
                Stock = 44,
                IsFeatured = false,
                CategoryId = categories[3].Id
            },
            new()
            {
                Name = "Silk Rain Body Cream",
                NameFa = "کرم بدن باران ابریشم",
                Slug = "silk-rain-body-cream",
                ShortDescription = "Rich body cream with shea and oat extract.",
                ShortDescriptionFa = "کرم غنی بدن با شی باتر و عصاره جو دوسر.",
                Description = "Silk Rain Body Cream comforts dry skin with shea butter and colloidal oat. The texture is plush yet fast-absorbing—ideal for evening rituals.",
                DescriptionFa = "کرم بدن باران ابریشم پوست خشک را با شی باتر و جو کلوئیدی تسکین می‌دهد. بافتی مخملی اما زودجذب دارد و برای آیین مراقبت شبانه ایده‌آل است.",
                Price = 29.00m,
                ImageUrl = "https://images.unsplash.com/photo-1608571423902-eed4a5adbb8a?auto=format&fit=crop&w=900&q=80",
                Brand = "Velora",
                SkinType = "Sensitive",
                Stock = 50,
                IsFeatured = false,
                CategoryId = categories[3].Id
            },
            new()
            {
                Name = "Cloud Clean Foaming Cleanser",
                NameFa = "پاک‌کننده فومی ابر پاک",
                Slug = "cloud-clean-foaming-cleanser",
                ShortDescription = "Gentle foam cleanser that never strips the skin.",
                ShortDescriptionFa = "پاک‌کننده فومی ملایم که پوست را خشک نمی‌کند.",
                Description = "Cloud Clean Foaming Cleanser lifts makeup and daily residue while keeping the skin barrier calm. Amino-acid surfactants create a soft cloud of foam that rinses clean.",
                DescriptionFa = "پاک‌کننده فومی ابر پاک آرایش و آلودگی روزانه را برمی‌دارد و سد پوست را آرام نگه می‌دارد. سورفکتانت‌های آمینواسیدی فومی نرم می‌سازند که تمیز آبکشی می‌شود.",
                Price = 26.00m,
                ImageUrl = "https://images.unsplash.com/photo-1556228578-0d85b1a4d571?auto=format&fit=crop&w=900&q=80",
                Brand = "Velora",
                SkinType = "Sensitive",
                Stock = 58,
                IsFeatured = false,
                CategoryId = categories[0].Id
            }
        };

        db.Products.AddRange(products);
        await db.SaveChangesAsync();
    }

    private static async Task EnsureCompatibleSchemaAsync(AppDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        // Additive upgrades for databases created before customer accounts existed.
        // EnsureCreated does not alter an already-created SQLite file.
        if (!await TableExistsAsync(db, "Customers"))
        {
            Console.WriteLine("Adding Customers table to existing SQLite database…");
            await db.Database.ExecuteSqlRawAsync(
                """
                CREATE TABLE "Customers" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_Customers" PRIMARY KEY AUTOINCREMENT,
                    "Email" TEXT NOT NULL,
                    "PasswordHash" TEXT NOT NULL,
                    "FullName" TEXT NOT NULL,
                    "Phone" TEXT NOT NULL,
                    "CreatedAt" TEXT NOT NULL
                );
                """);
            await db.Database.ExecuteSqlRawAsync(
                """CREATE UNIQUE INDEX "IX_Customers_Email" ON "Customers" ("Email");""");
        }

        if (!await TableExistsAsync(db, "CustomerAddresses"))
        {
            Console.WriteLine("Adding CustomerAddresses table to existing SQLite database…");
            await db.Database.ExecuteSqlRawAsync(
                """
                CREATE TABLE "CustomerAddresses" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_CustomerAddresses" PRIMARY KEY AUTOINCREMENT,
                    "CustomerId" INTEGER NOT NULL,
                    "Label" TEXT NOT NULL,
                    "FullName" TEXT NOT NULL,
                    "Phone" TEXT NOT NULL,
                    "Line1" TEXT NOT NULL,
                    "City" TEXT NOT NULL,
                    "PostalCode" TEXT NOT NULL,
                    "IsDefault" INTEGER NOT NULL,
                    CONSTRAINT "FK_CustomerAddresses_Customers_CustomerId"
                        FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE
                );
                """);
            await db.Database.ExecuteSqlRawAsync(
                """CREATE INDEX "IX_CustomerAddresses_CustomerId" ON "CustomerAddresses" ("CustomerId");""");
        }

        if (await TableExistsAsync(db, "Orders") && !await ColumnExistsAsync(db, "Orders", "CustomerId"))
        {
            Console.WriteLine("Adding Orders.CustomerId column to existing SQLite database…");
            await db.Database.ExecuteSqlRawAsync(
                """ALTER TABLE "Orders" ADD COLUMN "CustomerId" INTEGER NULL;""");
            await db.Database.ExecuteSqlRawAsync(
                """CREATE INDEX "IX_Orders_CustomerId" ON "Orders" ("CustomerId");""");
        }

        if (!await TableExistsAsync(db, "ProductRatings"))
        {
            Console.WriteLine("Adding ProductRatings table to existing SQLite database…");
            await db.Database.ExecuteSqlRawAsync(
                """
                CREATE TABLE "ProductRatings" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_ProductRatings" PRIMARY KEY AUTOINCREMENT,
                    "ProductId" INTEGER NOT NULL,
                    "CustomerId" INTEGER NOT NULL,
                    "Stars" INTEGER NOT NULL,
                    "CreatedAt" TEXT NOT NULL,
                    "UpdatedAt" TEXT NOT NULL,
                    CONSTRAINT "FK_ProductRatings_Products_ProductId"
                        FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE,
                    CONSTRAINT "FK_ProductRatings_Customers_CustomerId"
                        FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE
                );
                """);
            await db.Database.ExecuteSqlRawAsync(
                """CREATE UNIQUE INDEX "IX_ProductRatings_ProductId_CustomerId" ON "ProductRatings" ("ProductId", "CustomerId");""");
            await db.Database.ExecuteSqlRawAsync(
                """CREATE INDEX "IX_ProductRatings_CustomerId" ON "ProductRatings" ("CustomerId");""");
        }

        if (!await TableExistsAsync(db, "AdminUsers"))
        {
            Console.WriteLine("Adding AdminUsers table to existing SQLite database…");
            await db.Database.ExecuteSqlRawAsync(
                """
                CREATE TABLE "AdminUsers" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_AdminUsers" PRIMARY KEY AUTOINCREMENT,
                    "Email" TEXT NOT NULL,
                    "PasswordHash" TEXT NOT NULL,
                    "DisplayName" TEXT NOT NULL
                );
                """);
            await db.Database.ExecuteSqlRawAsync(
                """CREATE UNIQUE INDEX "IX_AdminUsers_Email" ON "AdminUsers" ("Email");""");
        }
    }

    private static async Task<bool> TableExistsAsync(AppDbContext db, string tableName)
    {
        var connection = await OpenConnectionAsync(db);
        await using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT 1 FROM sqlite_master WHERE type = 'table' AND name = $name LIMIT 1;";
        AddParameter(command, "$name", tableName);
        var result = await command.ExecuteScalarAsync();
        return result is not null && result is not DBNull;
    }

    private static async Task<bool> ColumnExistsAsync(AppDbContext db, string tableName, string columnName)
    {
        var connection = await OpenConnectionAsync(db);
        await using var command = connection.CreateCommand();
        command.CommandText =
            $"SELECT 1 FROM pragma_table_info('{tableName}') WHERE name = $column LIMIT 1;";
        AddParameter(command, "$column", columnName);
        var result = await command.ExecuteScalarAsync();
        return result is not null && result is not DBNull;
    }

    private static async Task<DbConnection> OpenConnectionAsync(AppDbContext db)
    {
        var connection = db.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        return connection;
    }

    private static void AddParameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    private static async Task EnsureAdminUserAsync(AppDbContext db, AdminSeedOptions adminSeed)
    {
        var email = adminSeed.Email.Trim().ToLowerInvariant();
        if (await db.AdminUsers.AnyAsync(u => u.Email == email))
        {
            return;
        }

        db.AdminUsers.Add(new AdminUser
        {
            Email = email,
            DisplayName = string.IsNullOrWhiteSpace(adminSeed.DisplayName) ? "Admin" : adminSeed.DisplayName.Trim(),
            PasswordHash = PasswordHasher.Hash(adminSeed.Password)
        });
        await db.SaveChangesAsync();
    }
}
