using System.Text.Json.Serialization;

namespace RecipeManagement.Core;

public sealed class NutritionInfo
{
    [JsonPropertyName("protein_g")]
    public double? ProteinG { get; set; }

    [JsonPropertyName("calories_kcal")]
    public double? CaloriesKcal { get; set; }

    [JsonPropertyName("fat_g")]
    public double? FatG { get; set; }

    [JsonPropertyName("carbohydrate_g")]
    public double? CarbohydrateG { get; set; }
}
