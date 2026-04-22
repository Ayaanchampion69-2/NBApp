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
        //foreign key
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        //navigation property
        public Order Order { get; set; }

        public static implicit operator ShippingAddress(string v)
        {
            throw new NotImplementedException();
        }
    }
}
