using Microsoft.SemanticKernel;
using ProductApi.Contracts;
using System.ComponentModel;
using System.Text.Json;

namespace ProductApi.Plugins;

public class CustomerPlugin
{
    private readonly ICustomerRepository _repo;

    public CustomerPlugin(ICustomerRepository repo) => _repo = repo;

    [KernelFunction("get_customer_count")]
    [Description("Gets the count of active customers.")]
    public async Task<int> GetCustomerCount(
    [Description("Must be true to count active customers.")]
    bool isActive = true)
    {
        var all = await _repo.GetAllAsync();

        return all.Count(c => c.IsActive == isActive);
    }

    [KernelFunction("get_customer_count_by_city")]
    [Description("Gets the number of customers in a specific city. " +
                 "Returns -1 if the city name is not found in the system.")]
    public async Task<int> GetCustomerCountByCity(
        [Description("The city name to count customers for")] string city)
    {
        var normalized = city.Trim().ToLower();
        var all = await _repo.GetAllAsync();

        var exists = all.Any(c => c.City?.Trim().ToLower() == normalized);
        if (!exists) return -1;

        return all.Count(c => c.City?.Trim().ToLower() == normalized);
    }
   
    [KernelFunction("get_customer_count_all_cities")]
    [Description("Gets customer counts grouped by every city. Use this function when the user asks for customer counts for all cities or a citywise customer count breakdown.")]
    public async Task<string> GetCustomerCountAllCities()
    {
        Console.WriteLine("******** CUSTOMER ALL-CITIES FUNCTION CALLED ********");

        var all = await _repo.GetAllAsync();

        Console.WriteLine(
            $"******** CUSTOMER RECORDS FOUND: {all.Count()} ********");

        var result = all
            .GroupBy(c => c.City?.Trim())
            .Select(g => new CityCount
            {
                City = g.Key ?? "",
                Count = g.Count()
            })
            .OrderBy(x => x.City)
            .ToList();

        return JsonSerializer.Serialize(result);
    }
}
public class CityCount
{
    public string City { get; set; } = "";
    public int Count { get; set; }
}
