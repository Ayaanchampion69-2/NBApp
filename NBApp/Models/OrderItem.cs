using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NBApp.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        //foerign key
        
        public int OrderId { get; set; } 
        
        public int ProductId { get; set; }
        //navigation properties
        [ValidateNever]
        public Order Order { get; set; } = null!;
        [ValidateNever]
        public Products Product { get; set; } = null!;
        //computed
        [NotMapped]
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
