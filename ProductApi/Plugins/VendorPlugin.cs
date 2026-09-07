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
}
