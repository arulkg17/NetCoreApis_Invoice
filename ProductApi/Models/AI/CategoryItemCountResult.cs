namespace ProductApi.Models.AI;

public class CategoryItemCountResult
{
    public string CategoryName { get; set; } = string.Empty;

    public bool CategoryIsActive { get; set; }

    public int ActiveItemCount { get; set; }
}
