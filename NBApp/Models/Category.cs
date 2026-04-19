using System.ComponentModel.DataAnnotations;
namespace NBApp.Models
{
   public class Category
   {
     [Key]
     public int CategoryId { get; set; }
     [Required]
     [StringLength(100)]
     public string Name { get; set; } = string.Empty;
     [StringLength(500)]
     public string? Description { get; set; }
     // Navigation property for related products
     public List<Products> Products { get; set; } = new();

    }
}
