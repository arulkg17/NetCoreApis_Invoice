using System.ComponentModel;
using Microsoft.SemanticKernel;
using ProductApi.Contracts;

namespace ProductApi.Plugins;

public class VendorPlugin
{
    private readonly IVendorRepository _repo;

    public VendorPlugin(IVendorRepository repo) => _repo = repo;

    [KernelFunction("get_vendor_count")]
    [Description("Gets the count of vendors. Pass isActive=true for active only, " +
                 "isActive=false for inactive only, or omit isActive for the total count.")]
    public async Task<int> GetVendorCount(
        [Description("true for active only, false for inactive only, null for all")]
        bool? isActive = null)
    {
        var all = await _repo.GetAllAsync();
        return isActive is null
            ? all.Count()
            : all.Count(v => v.IsActive == isActive);
    }
    [KernelFunction("get_vendor_count_by_state")]
    [Description("Gets the number of vendors in a specific state. " +
                "Returns -1 if the state name is not found in the system.")]
    public async Task<int> GetVendorCountByState(
       [Description("The state name to count vendors for")] string state)
    {
        var normalized = state.Trim().ToLower();
        var all = await _repo.GetAllAsync();

        var exists = all.Any(v => v.State?.Trim().ToLower() == normalized);
        if (!exists) return -1;

        return all.Count(v => v.State?.Trim().ToLower() == normalized);
    }

    [KernelFunction("get_vendor_count_all_states")]
    [Description("Gets vendor counts grouped by every state, when the user " +
                 "wants a full statewise breakdown rather than a single state.")]
    public async Task<List<StateCount>> GetAllStateCounts()
    {
        var all = await _repo.GetAllAsync();

        return all
            .GroupBy(v => v.State?.Trim())
            .Select(g => new StateCount { State = g.Key, Count = g.Count() })
            .ToList();
    }
}


public class StateCount
{
    public string State { get; set; } = "";
    public int Count { get; set; }
}

