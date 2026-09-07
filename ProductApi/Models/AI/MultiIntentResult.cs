namespace ProductApi.Models.AI;

public class MultiIntentResult
{
    public string? Intent { get; set; }

    public string? CategoryName { get; set; }

    public bool? CategoryActiveOnly { get; set; }

    public bool? ItemActiveOnly { get; set; }

    public bool? CustomerActiveOnly { get; set; }

    public bool? VendorActiveOnly { get; set; }
}