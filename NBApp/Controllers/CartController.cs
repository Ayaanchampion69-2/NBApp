using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NBApp.Areas.Identity.Data;
using NBApp.Extensions;

using NBApp.Models;
using NBApp.ViewModels;
using System.Security.Claims;
using static NBApp.Models.Order;


namespace NBApp.Controllers
{
    public class CartController(NBAppContext context) : Controller
    {
        private readonly NBAppContext _context = context;
        private const string CartSessionKey = "ShoppingCart";

        // GET: Cart
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }


        // POST: Cart/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || !product.IsActive)
            {
                return NotFound();
            }

            var cart = GetCart();
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    Price = product.Price ?? 0m,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl
                });
            }

            SaveCart(cart);
            TempData["Message"] = $"{product.Name} added to cart!";
            return RedirectToAction(nameof(Index));
        }


        // POST: Cart/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Items.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
                SaveCart(cart);
            }

            return RedirectToAction(nameof(Index));
        }




        // POST: Cart/RemoveFromCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                cart.Items.Remove(item);
                SaveCart(cart);
            }

            return RedirectToAction(nameof(Index));
        }


        // POST: Cart/ClearCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToAction(nameof(Index));
        }


        // GET: Cart/Checkout
        [Authorize]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (cart.IsEmpty)
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction(nameof(Index));
            }
            return View(cart);
        }


        // POST: Cart/PlaceOrder
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string? buildingNumber, string? street, string? city, string? postalCode)
        {
            var cart = GetCart();
            if (cart.IsEmpty)
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Create the shipping address
            var shippingAddress = new ShippingAddress
            {
                BuildingNumber = buildingNumber ?? string.Empty,
                Street = street ?? string.Empty,
                City = city ?? string.Empty,
                PostalCode = postalCode ?? string.Empty
            };

            // Create the order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Total,
                Status = OrderStatus.Pending,
                ShippingAddress = shippingAddress
            };


            // Add order items
            foreach (var cartItem in cart.Items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Price
                });

                // Update stock quantity
                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= cartItem.Quantity;
                }
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Clear the cart
            HttpContext.Session.Remove(CartSessionKey);

            TempData["Message"] = "Order placed successfully!";
            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
        }



        // GET: Cart/OrderConfirmation
        [Authorize]
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }



        // Helper method to save cart to session
        private void SaveCart(CartViewModel cart)
        {
            HttpContext.Session.SetObject(CartSessionKey, cart);
        }

        // Helper method to get cart from session
        private CartViewModel GetCart()
        {
            return HttpContext.Session.GetObject<CartViewModel>(CartSessionKey) ?? new CartViewModel();
        }
    }
}