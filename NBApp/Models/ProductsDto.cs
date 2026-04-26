using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBApp.Models
{
    public class ProductsDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(1000)]
        public string? Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; } 

        public decimal? SalePrice { get; set; }

        [StringLength(500)]
        public IFormFile? ImageFile { get; set; }
        [Required]
        public DateTime? ReleaseDate { get; set; }


        [Required]
        [Range(0, int.MaxValue)]
        public int? StockQuantity { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;
        [Required]
        public string? SKUNumber { get; set; }

        // Foreign key
        public int? CategoryId { get; set; }
        // Navigation property

        [ValidateNever]
        public Category Category { get; set; } = null!;
    }
}
