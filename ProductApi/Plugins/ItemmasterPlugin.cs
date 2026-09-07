using System.ComponentModel;
using Microsoft.SemanticKernel;
using ProductApi.Contracts;

namespace ProductApi.Plugins;

public class ItemMasterPlugin
{
    private readonly IItemmasterRepository _itemRepo;
    private readonly ICategoryRepository _categoryRepo;

    public ItemMasterPlugin(IItemmasterRepository itemRepo, ICategoryRepository categoryRepo)
    {
        _itemRepo = itemRepo;
        _categoryRepo = categoryRepo;
    }

    [KernelFunction("get_item_count")]
    [Description("Gets the count of items in ItemMaster, optionally filtered by category name " +
                 "and/or active status. Omit categoryName to count across all categories.")]
    public async Task<int> GetItemCount(
        [Description("Category name, e.g. 'Rice'. Omit or pass null for all categories.")]
        string? categoryName = null,
        [Description("true for active only, false for inactive only, null for all")]
        bool? isActive = null)
    {
        var items = (await _itemRepo.GetAllAsync()).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            var categories = await _categoryRepo.GetAllAsync();
            var match = categories.FirstOrDefault(c =>
                string.Equals(c.Name?.Trim(), categoryName.Trim(), StringComparison.OrdinalIgnoreCase));

            // Category not found - return -1 as a sentinel so the model
            // can tell the user the category doesn't exist, instead of
            // silently reporting 0.
            if (match is null) return -1;

            items = items.Where(i => i.CategoryId == match.Id);
        }

        if (isActive is not null)
            items = items.Where(i => i.IsActive == isActive);

        return items.Count();
    }
}
