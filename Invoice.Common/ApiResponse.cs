using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public required string Message { get; set; }
    public T? Data { get; set; }
    public ApiError? Error { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
