using System.Text.Json;
using ProductApi.Contracts;

namespace ProductApi.Services;

public class MockMultiIntentAIProvider : IMultiIntentAIProvider
{
    public Task<string> GetMultiIntentAsync(string question)
    {
        var q = question.Trim().ToLowerInvariant();

        var intents = new List<object>();

        // --------------------------------------------------
        // 1. Category item count
        // --------------------------------------------------
        if (q.Contains("item") && q.Contains("how many"))
        {
            string? categoryName = null;

            if (q.Contains("rice"))
                categoryName = "Rice";
            else if (q.Contains("vegetables") || q.Contains("vegitables"))
                categoryName = "Vegetables";
            else if (q.Contains("snacks"))
                categoryName = "Snacks";
            else if (q.Contains("oil"))
                categoryName = "Oil";
            else if (q.Contains("soap"))
                categoryName = "Soap";
            else if (q.Contains("general"))
                categoryName = "General";
            else if (q.Contains("pulses"))
                categoryName = "Pulses";

            if (categoryName != null)
            {
                bool? itemActiveOnly = null;

                if (q.Contains("active"))
                    itemActiveOnly = true;

                if (q.Contains("inactive"))
                    itemActiveOnly = false;

                intents.Add(new
                {
                    intent = "CategoryItemCount",
                    categoryName,
                    itemActiveOnly
                });
            }
        }

        // --------------------------------------------------
        // 2. Category count
        // --------------------------------------------------
        if (q.Contains("category") &&
            (q.Contains("how many") || q.Contains("count")))
        {
            bool? categoryActiveOnly = null;

            if (q.Contains("active category"))
                categoryActiveOnly = true;

            if (q.Contains("inactive category"))
                categoryActiveOnly = false;

            intents.Add(new
            {
                intent = "CategoryCount",
                categoryActiveOnly
            });
        }

        // --------------------------------------------------
        // 3. Customer count
        // --------------------------------------------------
        if (q.Contains("customer") &&
            (q.Contains("how many") || q.Contains("count")))
        {
            bool? customerActiveOnly = null;

            if (q.Contains("active customer"))
                customerActiveOnly = true;

            if (q.Contains("inactive customer"))
                customerActiveOnly = false;

            intents.Add(new
            {
                intent = "CustomerCount",
                customerActiveOnly
            });
        }

        // --------------------------------------------------
        // 4. Vendor count
        // --------------------------------------------------
        if (q.Contains("vendor") &&
            (q.Contains("how many") || q.Contains("count")))
        {
            bool? vendorActiveOnly = null;

            if (q.Contains("active vendor"))
                vendorActiveOnly = true;

            if (q.Contains("inactive vendor"))
                vendorActiveOnly = false;

            intents.Add(new
            {
                intent = "VendorCount",
                vendorActiveOnly
            });
        }

        // --------------------------------------------------
        // 5. Unknown
        // --------------------------------------------------
        if (intents.Count == 0)
        {
            intents.Add(new
            {
                intent = "Unknown"
            });
        }

        var result =
            JsonSerializer.Serialize(intents);

        return Task.FromResult(result);
    }
}