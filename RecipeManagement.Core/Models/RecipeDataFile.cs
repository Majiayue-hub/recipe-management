using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RecipeManagement.Core;

internal sealed class RecipeDataFile
{
    [JsonPropertyName("recipes")]
    public List<Recipe> Recipes { get; set; } = new();
}
