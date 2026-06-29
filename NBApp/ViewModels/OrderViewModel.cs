using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NBApp.Models;
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

        public Order.OrderStatus Status { get; set; } = Order.OrderStatus.Pending;

        public decimal TotalAmount { get; set; }

        // User
        [Required]
        public string UserId { get; set; } = "";

        public string DisplayName { get; set; } = "";

        // Shipping Address
        [Required]
        [Display(Name = "House Number/Building Number")]
        public string BuildingNumber { get; set; } = "";

        [Required]
        [Display(Name = "Street Name")]
        public string Street { get; set; } = "";

        [Required]
        [Display(Name = "Suburb")]
        public int SuburbID { get; set; }

        // Populated for display only
        public string SuburbName { get; set; } = "";
        public string CityName { get; set; } = "";
        public decimal DeliveryCost { get; set; }

        // For the edit dropdown
        [ValidateNever]
        public List<Suburb> Suburbs { get; set; } = new();

        // Order Items
        public List<OrderItemViewModel> OrderItems { get; set; } = new();
    }

    public class OrderItemViewModel
    {
        public int OrderItemId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = "";

        [ForeignKey("Products")]
        public string? ImageUrl { get; set; }

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