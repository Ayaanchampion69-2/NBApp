using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBApp.Models
{
    public class Suburb
    {
        [Required]
        public int SuburbID { get; set; }
        public required string SuburbName { get; set; }
        [Range(0, 500, ErrorMessage =
            "Delivery cost must be between 0 and FJD500.")]
        public decimal? DeliveryCost { get; set; }
        [ForeignKey("City")]
        public int CityID { get; set; }

        public City City { get; set; }
    }
}
