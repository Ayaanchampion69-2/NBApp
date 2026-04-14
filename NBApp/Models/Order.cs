using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBApp.Models
{
    public class Order
    {
        public enum OrderStatus
        {
         Pending,
         Processing,
         shipped,
         Delivered,
         Cancelled
        }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Today;
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        [StringLength(500)]
        public string? ShippingAddress { get; set; }

        [ValidateNever]
        public NBAppUser User { get; set; }
        public string UserId { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
