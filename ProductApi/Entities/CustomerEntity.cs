using System.ComponentModel.DataAnnotations;

namespace ProductApi.Entities
{
    public class CustomerEntity
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; } 
        public string? Phone { get; set; }
    }
}
