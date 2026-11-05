using System;
using System.Collections.Generic;
using System.Linq;

namespace RecipeManagement.Core;

/// <summary>
/// Implements the recipe catalogue using the five nominated Part A collections:
/// Dictionary&lt;int, Recipe&gt;, List&lt;string&gt;, LinkedList&lt;int&gt;,
/// Stack&lt;int&gt; and Queue&lt;string&gt;. Part B adds a HashSet&lt;int&gt;
/// for the saved/favourite recipes.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    // Part A collections
    private readonly Dictionary<int, Recipe> _recipes = new();
    private readonly List<string> _shoppingList = new();
    private readonly LinkedList<int> _cookingPlan = new();
    private readonly Stack<int> _removedRecipes = new();
    private readonly Queue<string> _instructions = new();

    // Part B collection: saved/favourite recipe IDs.
    // HashSet gives O(1) add / remove / membership and rejects duplicates automatically.
    private readonly HashSet<int> _savedRecipes = new();

    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        if (recipes is null)
        {
            throw new ArgumentNullException(nameof(recipes));
        }

        foreach (Recipe recipe in recipes)
        {
            if (recipe is null)
            {
                continue;
            }

            // Last occurrence wins if the supplied data contains a repeated ID.
            _recipes[recipe.Id] = recipe;
        }
    }

    // ------------------------------------------------------------------
    // Catalogue (Dictionary<int, Recipe>)
    // ------------------------------------------------------------------

    public int RecipeCount => _recipes.Count;

    public bool AddRecipe(Recipe recipe)
    {
        if (recipe is null)
        {
            return false;
        }

        // Reject duplicate recipe IDs.
        if (_recipes.ContainsKey(recipe.Id))
        {
            return false;
        }

        _recipes.Add(recipe.Id, recipe);
        return true;
    }

    public Recipe? FindRecipe(int recipeId)
    {
        return _recipes.TryGetValue(recipeId, out Recipe? recipe) ? recipe : null;
    }

    public bool RemoveRecipe(int recipeId)
    {
        return _recipes.Remove(recipeId);
    }

    // ------------------------------------------------------------------
    // Shopping list (List<string>)
    // ------------------------------------------------------------------

    public int ShoppingItemCount => _shoppingList.Count;

    public int AddIngredientsToShoppingList(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);
        if (recipe is null)
        {
            return 0;
        }

        int added = 0;
        foreach (string ingredient in recipe.Ingredients)
        {
            _shoppingList.Add(ingredient);
            added++;
        }

        return added;
    }

    public IReadOnlyList<string> GetShoppingList()
    {
        return _shoppingList.AsReadOnly();
    }

    public void ClearShoppingList()
    {
        _shoppingList.Clear();
    }

    // ------------------------------------------------------------------
    // Cooking plan (LinkedList<int>) + removed-history (Stack<int>)
    // ------------------------------------------------------------------

    public int CookingPlanCount => _cookingPlan.Count;

    public int RemovedRecipeCount => _removedRecipes.Count;

    public bool AddRecipeToCookingPlan(int recipeId)
    {
        // Recipe must exist in the catalogue and may appear only once in the plan.
        if (!_recipes.ContainsKey(recipeId))
        {
            return false;
        }

        if (_cookingPlan.Contains(recipeId))
        {
            return false;
        }

        _cookingPlan.AddLast(recipeId);
        return true;
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        LinkedListNode<int>? node = _cookingPlan.Find(recipeId);
        if (node is null)
        {
            return false;
        }

        _cookingPlan.Remove(node);
        _removedRecipes.Push(recipeId);
        return true;
    }

    public bool RestoreLastRemovedRecipe()
    {
        if (_removedRecipes.Count == 0)
        {
            return false;
        }

        int recipeId = _removedRecipes.Pop();

        // Do not restore a recipe that is already in the plan.
        if (!_cookingPlan.Contains(recipeId))
        {
            _cookingPlan.AddLast(recipeId);
        }

        return true;
    }

    public int? PeekLastRemovedRecipe()
    {
        return _removedRecipes.Count == 0 ? null : _removedRecipes.Peek();
    }

    public IReadOnlyList<int> GetCookingPlan()
    {
        return _cookingPlan.ToList();
    }

    // ------------------------------------------------------------------
    // Active cooking instructions (Queue<string>)
    // ------------------------------------------------------------------

    public int PendingInstructionCount => _instructions.Count;

    public bool StartCooking(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);
        if (recipe is null)
        {
            return false;
        }

        // A new session replaces whatever was previously queued.
        _instructions.Clear();
        foreach (string step in recipe.Instructions)
        {
            _instructions.Enqueue(step);
        }

        return true;
    }

    public string? PeekNextInstruction()
    {
        return _instructions.Count == 0 ? null : _instructions.Peek();
    }

    public string? CompleteNextInstruction()
    {
        return _instructions.Count == 0 ? null : _instructions.Dequeue();
    }

    // ------------------------------------------------------------------
    // Part B: searches, protein report and saved recipes (HashSet<int>)
    // ------------------------------------------------------------------

    public IReadOnlyList<Recipe> SearchByTitle(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return Array.Empty<Recipe>();
        }

        return _recipes.Values
            .Where(r => r.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return Array.Empty<Recipe>();
        }

        return _recipes.Values
            .Where(r => r.Ingredients.Any(
                ing => ing.Contains(searchText, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count)
    {
        if (count <= 0)
        {
            return Array.Empty<Recipe>();
        }

        return _recipes.Values
            .Where(r => r.Nutrition?.ProteinG is not null)
            .OrderByDescending(r => r.Nutrition!.ProteinG!.Value)
            .Take(count)
            .ToList();
    }

    public bool AddSavedRecipe(int recipeId)
    {
        if (!_recipes.ContainsKey(recipeId))
        {
            return false;
        }

        return _savedRecipes.Add(recipeId);
    }

    public bool RemoveSavedRecipe(int recipeId)
    {
        return _savedRecipes.Remove(recipeId);
    }

    public bool IsRecipeSaved(int recipeId)
    {
        return _savedRecipes.Contains(recipeId);
    }

    public IReadOnlyList<int> GetSavedRecipes()
    {
        return _savedRecipes.OrderBy(id => id).ToList();
    }
}
