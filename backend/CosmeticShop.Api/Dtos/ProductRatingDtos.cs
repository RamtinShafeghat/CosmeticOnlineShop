using System.ComponentModel.DataAnnotations;

namespace CosmeticShop.Api.Dtos;

public class UpsertProductRatingDto
{
    [Range(1, 5)]
    public int Stars { get; set; }
}

public record ProductRatingSummaryDto(
    int ProductId,
    double AverageRating,
    int RatingCount,
    int? MyRating);
