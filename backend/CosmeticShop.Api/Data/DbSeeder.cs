using CosmeticShop.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CosmeticShop.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        if (await db.Products.AnyAsync())
        {
            return;
        }

        var categories = new List<Category>
        {
            new()
            {
                Name = "Skincare",
                Slug = "skincare",
                Description = "Serums, moisturizers, and daily essentials for luminous skin."
            },
            new()
            {
                Name = "Makeup",
                Slug = "makeup",
                Description = "Color and complexion pieces with a soft, modern finish."
            },
            new()
            {
                Name = "Fragrance",
                Slug = "fragrance",
                Description = "Light, memorable scents crafted for everyday wear."
            },
            new()
            {
                Name = "Body Care",
                Slug = "body-care",
                Description = "Nourishing body rituals for soft, hydrated skin."
            }
        };

        db.Categories.AddRange(categories);
        await db.SaveChangesAsync();

        var products = new List<Product>
        {
            new()
            {
                Name = "Dewdrop Hydrating Serum",
                Slug = "dewdrop-hydrating-serum",
                ShortDescription = "A weightless hyaluronic serum for all-day moisture.",
                Description = "Dewdrop Hydrating Serum layers silky hydration with multi-weight hyaluronic acid and aloe. Use morning and night under moisturizer for a dewy, plump finish without stickiness.",
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
                Slug = "velvet-rose-moisturizer",
                ShortDescription = "Creamy daily moisture with rosehip and ceramides.",
                Description = "Velvet Rose Moisturizer restores the skin barrier with ceramides and rosehip oil. Soft, cushiony texture melts in and leaves skin calm, smooth, and softly scented.",
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
                Slug = "sunrise-vitamin-c-cream",
                ShortDescription = "Brightening day cream with stable vitamin C.",
                Description = "Sunrise Vitamin C Cream supports a more even-looking complexion with 15% vitamin C and niacinamide. Lightweight enough for layering under SPF and makeup.",
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
                Slug = "soft-focus-foundation",
                ShortDescription = "Buildable medium coverage with a natural satin finish.",
                Description = "Soft Focus Foundation blends seamlessly for medium coverage that looks like skin. Infused with light-reflecting pigments and skincare oils for a breathable all-day wear.",
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
                Slug = "bloom-blush-stick",
                ShortDescription = "Cream blush that melts into a natural flush.",
                Description = "Bloom Blush Stick delivers a soft petal flush in one swipe. The creamy formula blends with fingers and works beautifully alone or over foundation.",
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
                Slug = "lash-poetry-mascara",
                ShortDescription = "Lengthening mascara with a soft, flexible hold.",
                Description = "Lash Poetry Mascara lifts and lengthens without clumping. The tapered brush reaches every lash for defined, wearable drama from day to night.",
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
                Slug = "petal-mist-eau-de-parfum",
                ShortDescription = "A soft floral with pear, peony, and warm musk.",
                Description = "Petal Mist opens with crisp pear, blooms into peony and jasmine, and settles into soft musk. An everyday fragrance that feels clean, luminous, and quietly memorable.",
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
                Slug = "citrus-veil-body-oil",
                ShortDescription = "Fast-absorbing oil with bergamot and jojoba.",
                Description = "Citrus Veil Body Oil leaves skin satin-soft with a sheer bergamot scent. Massage onto damp skin after showering for a luminous finish that never feels greasy.",
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
                Slug = "silk-rain-body-cream",
                ShortDescription = "Rich body cream with shea and oat extract.",
                Description = "Silk Rain Body Cream comforts dry skin with shea butter and colloidal oat. The texture is plush yet fast-absorbing—ideal for evening rituals.",
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
                Slug = "cloud-clean-foaming-cleanser",
                ShortDescription = "Gentle foam cleanser that never strips the skin.",
                Description = "Cloud Clean Foaming Cleanser lifts makeup and daily residue while keeping the skin barrier calm. Amino-acid surfactants create a soft cloud of foam that rinses clean.",
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
}
