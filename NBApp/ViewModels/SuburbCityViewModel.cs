using NBApp.Models;
using System.ComponentModel.DataAnnotations;

namespace NBApp.ViewModels
{
    public class CitySuburbViewModel
    {
        public List<City> Cities { get; set; } = new();
        public List<Suburb> Suburbs { get; set; } = new();

        // City form
        public CityFormModel CityForm { get; set; } = new();

        // Suburb form
        public SuburbFormModel SuburbForm { get; set; } = new();
    }

    public class CityFormModel
    {
        public int? CityID { get; set; }

        [Required(ErrorMessage = "City name is required.")]
        [StringLength(20, ErrorMessage = "City name cannot exceed 20 characters.")]
        public string CityName { get; set; } = string.Empty;
    }

    public class SuburbFormModel
    {
        public int? SuburbID { get; set; }

        [Required(ErrorMessage = "Suburb name is required.")]
        [StringLength(100, ErrorMessage = "Suburb name cannot exceed 100 characters.")]
        public string SuburbName { get; set; } = string.Empty;

        
        [Range(0, 500, ErrorMessage = "Delivery cost must be between 0 and FJD500.")]
        public decimal DeliveryCost { get; set; }

        [Required(ErrorMessage = "Please select a city.")]
        public int CityID { get; set; }
    }
}