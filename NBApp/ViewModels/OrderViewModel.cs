using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBApp.ViewModels
{
    public class OrderViewModel
    {
        // Order
        public int OrderId { get; set; }

        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        // User
        [Required]
        public string UserId { get; set; } = "";

        public string UserDisplayName { get; set; } = "";

        // Shipping Address
        [Required]
        public string BuildingNumber { get; set; } = "";

        [Required]
        public string Street { get; set; } = "";

        [Required]
        public string City { get; set; } = "";

        public string PostalCode { get; set; } = "";

        // Order Items
        public List<OrderItemViewModel> OrderItems { get; set; } = new();
    }

    public class OrderItemViewModel
    {
        public int OrderItemId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = "";

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}