using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RecipeManagement.Core;

/// <summary>
/// Supplied recipe model. The assignment uses Id, Title, Ingredients,
/// Instructions and Nutrition.ProteinG.
/// </summary>
public sealed class Recipe
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("ingredients")]
    public List<string> Ingredients { get; set; } = new();

    [JsonPropertyName("instructions")]
    public List<string> Instructions { get; set; } = new();

    [JsonPropertyName("nutrition")]
    public NutritionInfo? Nutrition { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("prep_time")]
    public string? PrepTime { get; set; }

    [JsonPropertyName("cook_time")]
    public string? CookTime { get; set; }

    [JsonPropertyName("total_time")]
    public string? TotalTime { get; set; }

    [JsonPropertyName("servings")]
    public string? Servings { get; set; }
}
