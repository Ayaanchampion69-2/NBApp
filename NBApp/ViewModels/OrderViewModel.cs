using System;

namespace NBApp.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        // Minimal User view model to expose Id used by the view
        public UserViewModel User { get; set; } = new UserViewModel();

        // ShippingAddress property required by the Details view
        public AddressViewModel ShippingAddress { get; set; } = new AddressViewModel();
    }

    public class UserViewModel
    {
        public string Id { get; set; }
    }

    public class AddressViewModel
    {
        public string BuildingNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }
}