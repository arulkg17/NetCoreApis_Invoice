namespace ProductApi.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public required string Message { get; set; }
    public T? Data { get; set; }
    public ApiError? Error { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int TotalRecords { get; set; }
}
