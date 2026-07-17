
using System.ComponentModel.DataAnnotations;

namespace Invoice.Common;

public class ApiError
{
    [Required]
    public string Code { get; set; }
    [Required]
    public string Details { get; set; }
}
