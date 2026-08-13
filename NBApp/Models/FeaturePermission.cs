using System.ComponentModel.DataAnnotations;

namespace NBApp.Models
{
    public class FeaturePermission
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FeatureKey { get; set; } = string.Empty; // e.g. "Product.Create"

        [Required]
        [MaxLength(256)]
        public string RoleName { get; set; } = string.Empty; // e.g. "Admin"
    }
}