using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NBApp.Validators;

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
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }

        [Display(Name = "Sale Price")]
        public decimal? SalePrice { get; set; }

        //[StringLength(500)]
        [Display(Name = "Product Image File")]
        public IFormFile? ImageFile { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [NoReleaseDateInPast(ErrorMessage = "Release Date cannot be before today or after 3 months.")]
        [Display(Name = "Release Date")]
        public DateTime? ReleaseDate { get; set; } = DateTime.Today;


        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Stock Quantity")]
        public int? StockQuantity { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;
        [Required]
        [Display(Name = "SKU Number")]
        public string? SKUNumber { get; set; }

        // Foreign key
        [Display(Name ="Category")]
        public int? CategoryId { get; set; }
        // Navigation property

        [ValidateNever]
        public Category Category { get; set; } = null!;
    }
}
