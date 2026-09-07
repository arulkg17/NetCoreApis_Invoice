using System.ComponentModel;
using Microsoft.SemanticKernel;
using ProductApi.Contracts;

namespace ProductApi.Plugins;

public class UserPlugin
{
    private readonly IUserRepository _repo;

    public UserPlugin(IUserRepository repo) => _repo = repo;

    [KernelFunction("get_user_count")]
    [Description("Gets the count of system users. Pass isActive=true for active only, " +
                 "isActive=false for inactive only, or omit isActive for the total count.")]
    public async Task<int> GetUserCount(
        [Description("true for active only, false for inactive only, null for all")]
        bool? isActive = null)
    {
        var all = await _repo.GetAllAsync();
        return isActive is null
            ? all.Count()
            : all.Count(u => u.IsActive == isActive);
    }
}
