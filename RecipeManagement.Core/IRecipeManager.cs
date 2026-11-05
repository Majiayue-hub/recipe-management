using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Supplied public API. Students implement RecipeManager.
/// </summary>
public interface IRecipeManager
{
    int RecipeCount { get; }
    int ShoppingItemCount { get; }
    int CookingPlanCount { get; }
    int PendingInstructionCount { get; }
    int RemovedRecipeCount { get; }

    bool AddRecipe(Recipe recipe);
    Recipe? FindRecipe(int recipeId);
    bool RemoveRecipe(int recipeId);

    int AddIngredientsToShoppingList(int recipeId);
    IReadOnlyList<string> GetShoppingList();
    void ClearShoppingList();

    bool AddRecipeToCookingPlan(int recipeId);
    bool RemoveRecipeFromCookingPlan(int recipeId);
    bool RestoreLastRemovedRecipe();
    int? PeekLastRemovedRecipe();
    IReadOnlyList<int> GetCookingPlan();

    bool StartCooking(int recipeId);
    string? PeekNextInstruction();
    string? CompleteNextInstruction();

    // Part B
    IReadOnlyList<Recipe> SearchByTitle(string searchText);
    IReadOnlyList<Recipe> SearchByIngredient(string searchText);
    IReadOnlyList<Recipe> GetHighestProteinRecipes(int count);
    bool AddSavedRecipe(int recipeId);
    bool RemoveSavedRecipe(int recipeId);
    bool IsRecipeSaved(int recipeId);
    IReadOnlyList<int> GetSavedRecipes();
}
