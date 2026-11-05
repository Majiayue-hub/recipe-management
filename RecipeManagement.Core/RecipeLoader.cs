using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RecipeManagement.Core;

/// <summary>
/// Supplied JSON loader. Students do not implement this class.
/// </summary>
public static class RecipeLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static IReadOnlyList<Recipe> Load(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException(
                "File path must not be null, empty or white-space.",
                nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"Recipe file was not found: {filePath}",
                filePath);
        }

        string json = File.ReadAllText(filePath);
        RecipeDataFile? data;
        try
        {
            data = JsonSerializer.Deserialize<RecipeDataFile>(json, JsonOptions);
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException(
                "The recipe file does not contain valid JSON.",
                exception);
        }

        if (data?.Recipes is null)
        {
            throw new InvalidDataException(
                "The recipe file is missing a recipes collection.");
        }

        return data.Recipes.AsReadOnly();
    }
}
