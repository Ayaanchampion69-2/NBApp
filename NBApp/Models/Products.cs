using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NBApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBApp.Models
{
    public class Products
    {
        public int ProductId { get; set; }

        //[Required]
        //[StringLength(200)]
        public string Name { get; set; } = string.Empty;

        //[StringLength(1000)]
        public string? Description { get; set; }

        //[Required]
        //[Column(TypeName = "decimal(18,2)")]
        //[Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }

        public decimal? SalePrice { get; set; }

        // [StringLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime? ReleaseDate { get; set; }


        //[Required]
        //[Range(0, int.MaxValue)]
        public int? StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;

        public string? SKUNumber { get; set; }

        // Foreign key
        public int? CategoryId { get; set; }

        [ValidateNever]
        public Category Category { get; set; } = null!;

        // Navigation property for order items
        [ValidateNever]
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}