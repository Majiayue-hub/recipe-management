using System;
using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Application;

public static class ConsoleMenu
{
    public static void PrintMenu(IRecipeManager manager)
    {
        Console.Clear();
        Console.WriteLine("RECIPE MANAGEMENT SYSTEM");
        Console.WriteLine($"Recipes: {manager.RecipeCount} | " +
            $"Shopping items: {manager.ShoppingItemCount} | " +
            $"Planned recipes: {manager.CookingPlanCount} | " +
            $"Queued instructions: {manager.PendingInstructionCount}");
        Console.WriteLine();
        Console.WriteLine(" 1. Find and display recipe by ID (PartA)");
        Console.WriteLine(" 2. Add a recipe (PartA)");
        Console.WriteLine(" 3. Remove a recipe (PartA)");
        Console.WriteLine(" 4. Add recipe ingredients to shopping list (PartA)");
        Console.WriteLine(" 5. Display shopping list (PartA)");
        Console.WriteLine(" 6. Clear shopping list (PartA)");
        Console.WriteLine(" 7. Add recipe to cooking plan (PartA)");
        Console.WriteLine(" 8. Remove recipe from cooking plan (PartA)");
        Console.WriteLine(" 9. Peek last removed recipe (PartA)");
        Console.WriteLine("10. Restore last removed recipe (PartA)");
        Console.WriteLine("11. Display cooking plan (PartA)");
        Console.WriteLine("12. Start cooking a recipe (PartA)");
        Console.WriteLine("13. Peek next cooking instruction (PartA)");
        Console.WriteLine("14. Complete next cooking instruction (PartA)");
        Console.WriteLine("15. Search recipes by title (PartB)");
        Console.WriteLine("16. Search recipes by ingredient (PartB)");
        Console.WriteLine("17. Display highest-protein recipes (PartB)");
        Console.WriteLine("18. Save a recipe (PartB)");
        Console.WriteLine("19. Remove a saved recipe (PartB)");
        Console.WriteLine("20. Display saved recipes (PartB)");
        Console.WriteLine("21. Check if a recipe is saved (PartB)");
        Console.WriteLine(" 0. Exit");
        Console.WriteLine();
        Console.Write("Select an option: ");
    }

    public static void AddRecipe(IRecipeManager manager)
    {
        var recipe = new Recipe
        {
            Id = ReadInt("New recipe ID"),
            Title = ReadText("Title"),
            Ingredients = ReadItems("Ingredients separated by |"),
            Instructions = ReadItems("Instructions separated by |")
        };
        ShowResult(manager.AddRecipe(recipe), "Recipe added.");
    }

    public static void RemoveRecipe(IRecipeManager manager)
    {
        ShowResult(
            manager.RemoveRecipe(ReadInt("Recipe ID")),
            "Recipe removed from catalogue.");
    }

    public static void AddIngredients(IRecipeManager manager)
    {
        int added = manager.AddIngredientsToShoppingList(ReadInt("Recipe ID"));
        Console.WriteLine(added == 0
            ? "No ingredients added (recipe not found)."
            : $"Added {added} shopping items.");
    }

    public static void DisplayLastRemoved(IRecipeManager manager)
    {
        int? id = manager.PeekLastRemovedRecipe();
        Console.WriteLine(id is null
            ? "Removed-recipe stack is empty."
            : $"Last removed recipe ID: {id}");
    }

    public static void DisplayCookingPlan(IRecipeManager manager)
    {
        IReadOnlyList<int> plan = manager.GetCookingPlan();
        if (plan.Count == 0)
        {
            Console.WriteLine("Cooking plan is empty.");
            return;
        }

        for (int index = 0; index < plan.Count; index++)
        {
            int id = plan[index];
            string title = manager.FindRecipe(id)?.Title ?? "Missing recipe";
            Console.WriteLine($"{index + 1}. {id} - {title}");
        }
    }

    public static void DisplayInstruction(string? instruction, bool completed)
    {
        if (instruction is null)
        {
            Console.WriteLine("Instruction queue is empty.");
            return;
        }

        Console.WriteLine(completed ? "Completed:" : "Next:");
        Console.WriteLine(instruction);
    }

    public static void DisplayRecipe(Recipe? recipe)
    {
        if (recipe is null)
        {
            Console.WriteLine("Recipe not found.");
            return;
        }

        Console.WriteLine($"{recipe.Id}: {recipe.Title}");
        Console.WriteLine();
        Console.WriteLine("Ingredients:");
        foreach (string ingredient in recipe.Ingredients)
        {
            Console.WriteLine($"- {ingredient}");
        }

        Console.WriteLine();
        Console.WriteLine("Instructions:");
        for (int index = 0; index < recipe.Instructions.Count; index++)
        {
            Console.WriteLine($"{index + 1}. {recipe.Instructions[index]}");
        }

        if (recipe.Nutrition?.ProteinG is double protein)
        {
            Console.WriteLine();
            Console.WriteLine($"Protein: {protein:0.0} g");
        }
    }

    public static void DisplayRecipes(IReadOnlyList<Recipe> recipes)
    {
        if (recipes.Count == 0)
        {
            Console.WriteLine("No matching recipes.");
            return;
        }

        foreach (Recipe recipe in recipes)
        {
            string protein = recipe.Nutrition?.ProteinG?.ToString("0.0") ?? "N/A";
            Console.WriteLine($"{recipe.Id,4} {recipe.Title}  Protein: {protein} g");
        }
    }

    public static void DisplayShoppingList(IReadOnlyList<string> items)
    {
        if (items.Count == 0)
        {
            Console.WriteLine("Shopping list is empty.");
            return;
        }

        for (int index = 0; index < items.Count; index++)
        {
            Console.WriteLine($"{index + 1,3}. {items[index]}");
        }
    }

    public static void DisplaySavedRecipes(IRecipeManager manager)
    {
        IReadOnlyList<int> saved = manager.GetSavedRecipes();
        if (saved.Count == 0)
        {
            Console.WriteLine("No saved recipes.");
            return;
        }

        foreach (int id in saved)
        {
            string title = manager.FindRecipe(id)?.Title ?? "Missing recipe";
            Console.WriteLine($"{id}: {title}");
        }
    }

    public static void CheckSavedRecipe(IRecipeManager manager)
    {
        int id = ReadInt("Recipe ID");
        Console.WriteLine(manager.IsRecipeSaved(id)
            ? "Recipe is saved."
            : "Recipe is not saved.");
    }

    public static int ReadInt(string label)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            if (int.TryParse(Console.ReadLine(), out int value))
            {
                return value;
            }

            Console.WriteLine("Enter a valid whole number.");
        }
    }

    public static string ReadText(string label, string? defaultValue = null)
    {
        Console.Write(defaultValue is null
            ? $"{label}: "
            : $"{label} [{defaultValue}]: ");
        string value = Console.ReadLine()?.Trim() ?? string.Empty;
        return string.IsNullOrWhiteSpace(value)
            ? defaultValue ?? string.Empty
            : value;
    }

    public static List<string> ReadItems(string label)
    {
        string text = ReadText(label);
        return text.Split(
                '|',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }

    public static void ShowResult(bool succeeded, string successMessage)
    {
        Console.WriteLine(succeeded ? successMessage : "Operation was not completed.");
    }

    public static void Pause()
    {
        Console.WriteLine();
        Console.Write("Press Enter to continue...");
        Console.ReadLine();
    }
}
