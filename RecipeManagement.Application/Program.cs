using System;
using RecipeManagement.Core;

namespace RecipeManagement.Application;

public static class Program
{
    private const string DefaultRecipePath = "data/recipes.json";

    public static void Main(string[] args)
    {
        string path = args.Length > 0 ? args[0] : ResolveRecipePath(DefaultRecipePath);
        IRecipeManager manager = CreateManager(path);

        while (true)
        {
            ConsoleMenu.PrintMenu(manager);
            string choice = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        ConsoleMenu.DisplayRecipe(
                            manager.FindRecipe(ConsoleMenu.ReadInt("Recipe ID")));
                        break;
                    case "2":
                        ConsoleMenu.AddRecipe(manager);
                        break;
                    case "3":
                        ConsoleMenu.RemoveRecipe(manager);
                        break;
                    case "4":
                        ConsoleMenu.AddIngredients(manager);
                        break;
                    case "5":
                        ConsoleMenu.DisplayShoppingList(manager.GetShoppingList());
                        break;
                    case "6":
                        manager.ClearShoppingList();
                        Console.WriteLine("Shopping list cleared.");
                        break;
                    case "7":
                        ConsoleMenu.ShowResult(
                            manager.AddRecipeToCookingPlan(
                                ConsoleMenu.ReadInt("Recipe ID")),
                            "Recipe added to cooking plan.");
                        break;
                    case "8":
                        ConsoleMenu.ShowResult(
                            manager.RemoveRecipeFromCookingPlan(
                                ConsoleMenu.ReadInt("Recipe ID")),
                            "Recipe removed from cooking plan.");
                        break;
                    case "9":
                        ConsoleMenu.DisplayLastRemoved(manager);
                        break;
                    case "10":
                        ConsoleMenu.ShowResult(
                            manager.RestoreLastRemovedRecipe(),
                            "Last removed recipe restored.");
                        break;
                    case "11":
                        ConsoleMenu.DisplayCookingPlan(manager);
                        break;
                    case "12":
                        ConsoleMenu.ShowResult(
                            manager.StartCooking(ConsoleMenu.ReadInt("Recipe ID")),
                            "Cooking session started.");
                        break;
                    case "13":
                        ConsoleMenu.DisplayInstruction(
                            manager.PeekNextInstruction(),
                            completed: false);
                        break;
                    case "14":
                        ConsoleMenu.DisplayInstruction(
                            manager.CompleteNextInstruction(),
                            completed: true);
                        break;
                    case "15":
                        ConsoleMenu.DisplayRecipes(
                            manager.SearchByTitle(
                                ConsoleMenu.ReadText("Title search")));
                        break;
                    case "16":
                        ConsoleMenu.DisplayRecipes(
                            manager.SearchByIngredient(
                                ConsoleMenu.ReadText("Ingredient search")));
                        break;
                    case "17":
                        ConsoleMenu.DisplayRecipes(
                            manager.GetHighestProteinRecipes(
                                ConsoleMenu.ReadInt("Number of recipes")));
                        break;
                    case "18":
                        ConsoleMenu.ShowResult(
                            manager.AddSavedRecipe(ConsoleMenu.ReadInt("Recipe ID")),
                            "Recipe saved.");
                        break;
                    case "19":
                        ConsoleMenu.ShowResult(
                            manager.RemoveSavedRecipe(ConsoleMenu.ReadInt("Recipe ID")),
                            "Recipe removed from saved list.");
                        break;
                    case "20":
                        ConsoleMenu.DisplaySavedRecipes(manager);
                        break;
                    case "21":
                        ConsoleMenu.CheckSavedRecipe(manager);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Unknown menu option.");
                        break;
                }
            }
            catch (NotImplementedException exception)
            {
                Console.WriteLine($"Not implemented: {exception.Message}");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Operation failed: {exception.Message}");
            }

            ConsoleMenu.Pause();
        }
    }

    private static IRecipeManager CreateManager(string path)
    {
        try
        {
            var recipes = RecipeLoader.Load(path);
            Console.WriteLine($"Loaded {recipes.Count} recipes from {path}.");
            return new RecipeManager(recipes);
        }
        catch (NotImplementedException exception)
        {
            Console.WriteLine($"Not implemented: {exception.Message}");
            Console.WriteLine("Starting with an empty recipe catalogue.");
            return new RecipeManager(Array.Empty<Recipe>());
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Could not load recipes: {exception.Message}");
            Console.WriteLine("Starting with an empty recipe catalogue.");
            return new RecipeManager(Array.Empty<Recipe>());
        }
    }

    // Looks for data/recipes.json beside the exe, in the current folder,
    // or in a parent folder (so IDE runs still find the file).
    private static string ResolveRecipePath(string relativePath)
    {
        string[] candidates =
        {
            relativePath,
            Path.Combine(AppContext.BaseDirectory, relativePath),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", relativePath)),
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), relativePath))
        };

        foreach (string candidate in candidates)
        {
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return relativePath;
    }
}
