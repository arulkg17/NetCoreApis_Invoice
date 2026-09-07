using System.ComponentModel;
using Microsoft.SemanticKernel;
using ProductApi.Contracts;

namespace ProductApi.Plugins;

public class CategoryPlugin
{
    private readonly ICategoryRepository _repo;

    public CategoryPlugin(ICategoryRepository repo) => _repo = repo;

    [KernelFunction("get_category_count")]
    [Description("Gets the count of categories. Pass isActive=true for active only, " +
                 "isActive=false for inactive only, or omit isActive for the total count.")]
    public async Task<int> GetCategoryCount(
        [Description("true for active only, false for inactive only, null for all")]
        bool? isActive = null)
    {
        var all = await _repo.GetAllAsync();
        return isActive is null
            ? all.Count()
            : all.Count(c => c.IsActive == isActive);
    }

    [KernelFunction("get_item_count_by_category")]
    [Description("Gets the count of items within a specific category. Reuses the existing " +
                 "GetCategoryItemCountAsync logic used by the original /api/ai/ask endpoint.")]
    public async Task<string> GetItemCountByCategory(
        [Description("Category name, e.g. 'Rice'")]
        string categoryName,
        [Description("true for active only, false for inactive only, null for all")]
        bool? itemActiveOnly = null)
    {
        var result = await _repo.GetCategoryItemCountAsync(
            categoryName,
            categoryActiveOnly: true,
            itemActiveOnly: itemActiveOnly);

        if (result is null)
            return $"Category '{categoryName}' was not found.";

        return $"Category '{categoryName}' has {result.ItemCount} matching item(s).";
    }
}