using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBApp.Models
{
    public class ShippingAddress
    {
        [Key]
        public int ShipID { get; set; }
        public string BuildingNumber { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? PostalCode { get; set; } = string.Empty;
        //navigation property
        public Order Order { get; set; }
    }
}
