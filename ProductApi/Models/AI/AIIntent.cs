namespace ProductApi.Models.AI;

public class AIIntent
{
    public string Intent { get; set; } = string.Empty;

    public string? CategoryName { get; set; }

    public bool? CategoryActiveOnly { get; set; }

    public bool? ItemActiveOnly { get; set; }
}
