using System;
using System.ComponentModel.DataAnnotations;

namespace NBApp.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        // Minimal User view model to expose Id used by the view
        public UserViewModel User { get; set; } = new UserViewModel();

        // ShippingAddress property required by the Details view
        public AddressViewModel ShippingAddress { get; set; } = new AddressViewModel();
    }

    public class UserViewModel
    {
        [Required]  
        public string Id { get; set; } = "";
        public string DisplayName { get; set; } = "";
    }

    public class AddressViewModel
    {
        [Required]
        public string BuildingNumber { get; set; } = "";
        [Required]
        public string Street { get; set; } ="";
        [Required]
        public string City { get; set; } = "";
        public string PostalCode { get; set; } = "";
    }
}