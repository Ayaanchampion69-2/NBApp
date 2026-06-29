using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace NBApp.Models
{
    public class City
    {
        [Required]
        public int CityID { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "City/Town Name")]
        public required string CityName { get; set; }
        [ValidateNever]
        public List<Suburb>? Suburbs { get; set; }
    }
}
