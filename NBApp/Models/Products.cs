using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBApp.Models
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        [StringLength(1000)]
        public string? Description { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; } = null! ;
        [ValidateNever]
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
//continue video at 1.39.18 time