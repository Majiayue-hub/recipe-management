using System.Collections.Generic;
using RecipeManagement.Core;
using Xunit;

namespace RecipeManagement.Tests;

/// <summary>
/// Tests for the RecipeManager. The first three tests are the supplied
/// demonstration tests; the rest cover each nominated collection, the required
/// edge cases and a component interaction.
/// </summary>
public sealed class RecipeManagerTests
{
    // ------------------------------------------------------------------
    // Supplied demonstration tests
    // ------------------------------------------------------------------

    [Fact]
    public void Constructor_BuildsRecipeDictionary()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
    }

    [Fact]
    public void InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.PeekNextInstruction());
    }

    [Fact]
    public void RemovedRecipesAreRestoredLastInFirstOut()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);
        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    // ------------------------------------------------------------------
    // Dictionary<int, Recipe> — catalogue
    // ------------------------------------------------------------------

    [Fact]
    public void AddRecipe_NewId_IsAdded()
    {
        var manager = CreateManager();
        var recipe = new Recipe { Id = 30, Title = "Recipe C" };

        Assert.True(manager.AddRecipe(recipe));
        Assert.Equal(3, manager.RecipeCount);
        Assert.Same(recipe, manager.FindRecipe(30));
    }

    [Fact]
    public void AddRecipe_DuplicateId_IsRejected()
    {
        var manager = CreateManager();

        Assert.False(manager.AddRecipe(new Recipe { Id = 10, Title = "Duplicate" }));
        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
    }

    [Fact]
    public void FindRecipe_MissingId_ReturnsNull()
    {
        var manager = CreateManager();
        Assert.Null(manager.FindRecipe(999));
    }

    [Fact]
    public void RemoveRecipe_ExistingId_RemovesIt()
    {
        var manager = CreateManager();

        Assert.True(manager.RemoveRecipe(10));
        Assert.Equal(1, manager.RecipeCount);
        Assert.Null(manager.FindRecipe(10));
    }

    [Fact]
    public void RemoveRecipe_MissingId_ReturnsFalse()
    {
        var manager = CreateManager();
        Assert.False(manager.RemoveRecipe(999));
    }

    // ------------------------------------------------------------------
    // List<string> — shopping list
    // ------------------------------------------------------------------

    [Fact]
    public void AddIngredients_CopiesStringsInOrder()
    {
        var manager = CreateManager();

        int added = manager.AddIngredientsToShoppingList(10);

        Assert.Equal(1, added);
        Assert.Equal(new[] { "1 apple" }, manager.GetShoppingList());
    }

    [Fact]
    public void AddIngredients_UnknownRecipe_AddsNothing()
    {
        var manager = CreateManager();

        int added = manager.AddIngredientsToShoppingList(999);

        Assert.Equal(0, added);
        Assert.Empty(manager.GetShoppingList());
    }

    [Fact]
    public void ClearShoppingList_EmptiesTheList()
    {
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(10);

        manager.ClearShoppingList();

        Assert.Equal(0, manager.ShoppingItemCount);
        Assert.Empty(manager.GetShoppingList());
    }

    // ------------------------------------------------------------------
    // LinkedList<int> — cooking plan
    // ------------------------------------------------------------------

    [Fact]
    public void AddRecipeToCookingPlan_AppendsInOrder()
    {
        var manager = CreateManager();

        Assert.True(manager.AddRecipeToCookingPlan(20));
        Assert.True(manager.AddRecipeToCookingPlan(10));

        Assert.Equal(new[] { 20, 10 }, manager.GetCookingPlan());
    }

    [Fact]
    public void AddRecipeToCookingPlan_DuplicateInPlan_IsRejected()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);

        Assert.False(manager.AddRecipeToCookingPlan(10));
        Assert.Equal(new[] { 10 }, manager.GetCookingPlan());
    }

    [Fact]
    public void AddRecipeToCookingPlan_UnknownRecipe_IsRejected()
    {
        var manager = CreateManager();
        Assert.False(manager.AddRecipeToCookingPlan(999));
        Assert.Empty(manager.GetCookingPlan());
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_MovesIdToHistory()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);

        Assert.True(manager.RemoveRecipeFromCookingPlan(10));

        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
        Assert.Equal(10, manager.PeekLastRemovedRecipe());
    }

    // ------------------------------------------------------------------
    // Stack<int> — removed-recipe history (edge cases)
    // ------------------------------------------------------------------

    [Fact]
    public void PeekLastRemovedRecipe_WhenStackEmpty_ReturnsNull()
    {
        var manager = CreateManager();
        Assert.Null(manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void RestoreLastRemovedRecipe_WhenStackEmpty_ReturnsFalse()
    {
        var manager = CreateManager();
        Assert.False(manager.RestoreLastRemovedRecipe());
    }

    // ------------------------------------------------------------------
    // Queue<string> — cooking instructions (edge cases)
    // ------------------------------------------------------------------

    [Fact]
    public void CompleteNextInstruction_WhenQueueEmpty_ReturnsNull()
    {
        var manager = CreateManager();
        Assert.Null(manager.PeekNextInstruction());
        Assert.Null(manager.CompleteNextInstruction());
    }

    [Fact]
    public void StartCooking_UnknownRecipe_ReturnsFalse()
    {
        var manager = CreateManager();
        Assert.False(manager.StartCooking(999));
        Assert.Equal(0, manager.PendingInstructionCount);
    }

    [Fact]
    public void StartCooking_ReplacesPreviousQueue()
    {
        var manager = CreateManager();
        manager.StartCooking(10);   // queues "First step", "Second step"

        Assert.True(manager.StartCooking(20));
        Assert.Equal(0, manager.PendingInstructionCount);
        Assert.Null(manager.PeekNextInstruction());
    }

    // ------------------------------------------------------------------
    // Component interaction
    // ------------------------------------------------------------------

    [Fact]
    public void Recipe_FlowsThrough_Catalogue_Plan_And_Cooking()
    {
        var manager = CreateManager();

        // Catalogue lookup.
        Recipe? found = manager.FindRecipe(10);
        Assert.NotNull(found);

        // Shopping list built from the catalogue recipe.
        Assert.Equal(1, manager.AddIngredientsToShoppingList(10));

        // Added to the plan, removed, then restored via the stack.
        Assert.True(manager.AddRecipeToCookingPlan(10));
        Assert.True(manager.RemoveRecipeFromCookingPlan(10));
        Assert.Empty(manager.GetCookingPlan());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 10 }, manager.GetCookingPlan());

        // Cooking session loads instructions FIFO.
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.CompleteNextInstruction());
        Assert.Null(manager.CompleteNextInstruction());
    }

    // ------------------------------------------------------------------
    // Part B: saved recipes (HashSet<int>)
    // ------------------------------------------------------------------

    [Fact]
    public void SavedRecipes_CanBeAdded_Checked_Removed()
    {
        var manager = CreateManager();

        Assert.True(manager.AddSavedRecipe(10));
        Assert.True(manager.IsRecipeSaved(10));
        Assert.False(manager.AddSavedRecipe(10));      // duplicates rejected
        Assert.Equal(new[] { 10 }, manager.GetSavedRecipes());

        Assert.True(manager.RemoveSavedRecipe(10));
        Assert.False(manager.IsRecipeSaved(10));
    }

    [Fact]
    public void AddSavedRecipe_UnknownRecipe_IsRejected()
    {
        var manager = CreateManager();
        Assert.False(manager.AddSavedRecipe(999));
    }

    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = new() { "1 apple" },
                Instructions = new() { "First step", "Second step" }
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe B"
            }
        });
    }
}
