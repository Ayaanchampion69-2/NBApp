using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBApp.Models
{
    public class ShippingAddress
    {
        [Key]
        public int ShipID { get; set; }

        [Required]
        public string BuildingNumber { get; set; } = string.Empty;

        [Required]
        public string Street { get; set; } = string.Empty;

        [ForeignKey("Suburb")]
        public int? SuburbID { get; set; }

        [ValidateNever]
        public Suburb? Suburb { get; set; } = null!;

        // Navigation property
        [ValidateNever]
        public Order Order { get; set; } = null!;
    }
}