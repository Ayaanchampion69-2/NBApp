using NBApp.Models;

namespace NBApp.ViewModels
{
    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = new();

        public decimal SubTotal => Items.Sum(i => i.TotalPrice);

        public decimal Surcharge => SubTotal * 0.03m; // 3% surcharge rate

        public decimal Total => SubTotal + Surcharge;

        public int ItemCount => Items.Sum(i => i.Quantity);

        public bool IsEmpty => Items.Count == 0;
    }
}