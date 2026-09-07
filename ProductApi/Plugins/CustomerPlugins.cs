using System.ComponentModel;
using Microsoft.SemanticKernel;
using ProductApi.Contracts;

namespace ProductApi.Plugins;

public class CustomerPlugin
{
    private readonly ICustomerRepository _repo;

    public CustomerPlugin(ICustomerRepository repo) => _repo = repo;

    [KernelFunction("get_customer_count")]
    [Description("Gets the count of customers. Pass isActive=true for active only, " +
                 "isActive=false for inactive only, or omit isActive for the total count.")]
    public async Task<int> GetCustomerCount(
        [Description("true for active only, false for inactive only, null for all")]
        bool? isActive = null)
    {
        var all = await _repo.GetAllAsync();
        return isActive is null
            ? all.Count()
            : all.Count(c => c.IsActive == isActive);
    }
}
