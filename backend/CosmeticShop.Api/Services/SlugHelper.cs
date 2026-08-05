using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CosmeticShop.Api.Services;

public static class SlugHelper
{
    public static string ToSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return $"item-{Guid.NewGuid():N}"[..12];
        }

        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var ch in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (category == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(ch);
            }
            else if (char.IsWhiteSpace(ch) || ch is '-' or '_')
            {
                builder.Append('-');
            }
        }

        var slug = Regex.Replace(builder.ToString(), "-+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? $"item-{Guid.NewGuid():N}"[..12] : slug;
    }
}
