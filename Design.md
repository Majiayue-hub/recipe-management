# Design

## 1. Saved recipe collection

I chose `HashSet<int>` for the saved/favourite recipe IDs.

- **Add**: `HashSet.Add` is average O(1) and automatically ignores duplicates, which directly satisfies the "one saved recipe only once" requirement without a manual `Contains` check.
- **Remove**: `HashSet.Remove` is average O(1).
- **Duplicate prevention**: attempting to save the same ID twice returns `false` because `Add` already reports whether the element was new.
- **Membership check**: `IsRecipeSaved` uses `HashSet.Contains`, which is average O(1) — much faster than scanning a `List<int>` (O(n)) every time.

`GetSavedRecipes()` copies the set into an ordered `List<int>` so the console display is deterministic.

## 2. Integration

The Part B features all build on the existing Part A `RecipeManager`. The catalogue (`Dictionary<int, Recipe>`) remains the single source of truth: `AddSavedRecipe` first checks that the ID exists in the dictionary, and the title/ingredient searches and the protein report enumerate `_recipes.Values`. The saved-recipe set only stores IDs, so the recipe itself is never duplicated — the menu resolves each saved ID back through `FindRecipe`. This keeps Part B logic thin and reuses the Part A dictionary, plan and queue behaviour unchanged.

## 3. Basic complexity

| Operation | Structure | Expected complexity | Reason |
| --- | --- | --- | --- |
| Lookup recipe by ID | Dictionary | Average O(1) | Hash-based key lookup. |
| Traverse cooking plan | LinkedList | O(n) | Each planned recipe may need to be visited. |
| Complete next instruction | Queue | O(1) | The item at the front is removed. |
| LINQ title/ingredient search | Recipe collection | O(n) | Each recipe may need to be inspected. |
| Check whether a recipe is saved | HashSet<int> | Average O(1) | Hash-based membership test. |
