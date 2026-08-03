using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NBApp.Areas.Identity.Data;
using NBApp.Extensions;
using NBApp.Models;
using NBApp.Services;
using NBApp.ViewModels;
using Stripe;
using System.Security.Claims;
using static NBApp.Models.Order;

namespace NBApp.Controllers
{
    public class CartController(NBAppContext context, StripeService stripeService, MPaisaService mpaisaService, IConfiguration config) : Controller
    {
        private readonly NBAppContext _context = context;
        private readonly StripeService _stripeService = stripeService;
        private readonly MPaisaService _mpaisaService = mpaisaService;
        private readonly IConfiguration _config = config;
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
                    cart.Items.Remove(item);
                else
                    item.Quantity = quantity;

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
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCart();
            if (cart.IsEmpty)
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction(nameof(Index));
            }

            var intent = _stripeService.CreatePaymentIntent((long)(cart.Total * 100));
            ViewBag.ClientSecret = intent.ClientSecret;
            ViewBag.PublishableKey = _config["Stripe:PublishableKey"];

            // Load cities and suburbs for dropdowns
            ViewBag.Cities = await _context.Cities.OrderBy(c => c.CityName).ToListAsync();
            ViewBag.Suburbs = await _context.Suburbs.Include(s => s.City).OrderBy(s => s.SuburbName).ToListAsync();

            return View(cart);
        }

        // POST: Cart/PlaceOrder
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string? buildingNumber, string? street, int suburbId)
        {
            var cart = GetCart();
            if (cart.IsEmpty)
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var suburb = await _context.Suburbs.Include(s => s.City).FirstOrDefaultAsync(s => s.SuburbID == suburbId);
            if (suburb == null)
            {
                TempData["Error"] = "Invalid suburb selected.";
                return RedirectToAction(nameof(Checkout));
            }

            var shippingAddress = new ShippingAddress
            {
                BuildingNumber = buildingNumber ?? string.Empty,
                Street = street ?? string.Empty,
                SuburbID = suburbId
            };

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Total + (suburb.DeliveryCost ?? 0m),
                Status = OrderStatus.Pending,
                ShippingAddress = shippingAddress
            };

            foreach (var cartItem in cart.Items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Price
                });

                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product != null)
                    product.StockQuantity -= cartItem.Quantity;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove(CartSessionKey);
            TempData["Message"] = "Order placed successfully!";
            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
        }

        // POST: Cart/MPaisaCheckout
        // Creates the order as Pending FIRST, then redirects to the M-PAiSA gateway.
        // This way the order isn't lost if the user closes the tab mid-payment.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MPaisaCheckout(string? buildingNumber, string? street, int suburbId)
        {
            var cart = GetCart();
            if (cart.IsEmpty)
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var suburb = await _context.Suburbs.Include(s => s.City).FirstOrDefaultAsync(s => s.SuburbID == suburbId);
            if (suburb == null)
            {
                TempData["Error"] = "Invalid suburb selected.";
                return RedirectToAction(nameof(Checkout));
            }

            var shippingAddress = new ShippingAddress
            {
                BuildingNumber = buildingNumber ?? string.Empty,
                Street = street ?? string.Empty,
                SuburbID = suburbId
            };

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Total + (suburb.DeliveryCost ?? 0m),
                Status = OrderStatus.Pending,
                ShippingAddress = shippingAddress
            };

            foreach (var cartItem in cart.Items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Price
                });

                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product != null)
                    product.StockQuantity -= cartItem.Quantity;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Cart is cleared now — the order already exists and holds the state.
            HttpContext.Session.Remove(CartSessionKey);

            var returnUrl = Url.Action(nameof(MPaisaReturn), "Cart", null, Request.Scheme)!;
            var checkoutUrl = _mpaisaService.BuildCheckoutUrl(order.OrderId, order.TotalAmount, returnUrl);

            return Redirect(checkoutUrl);
        }

        // GET: Cart/MPaisaReturn
        // M-PAiSA redirects the customer back here after they complete (or cancel) payment.
        [Authorize]
        public async Task<IActionResult> MPaisaReturn()
        {
            if (!_mpaisaService.VerifyCallback(Request.Query))
            {
                TempData["Error"] = "Payment verification failed. Please contact support if you were charged.";
                return RedirectToAction(nameof(Index));
            }

            if (!int.TryParse(Request.Query["orderRefNum"], out var orderId))
            {
                TempData["Error"] = "Payment reference was missing or invalid.";
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
                return NotFound();

            var status = Request.Query["status"].ToString();

            if (status == "Success")
            {
                order.Status = OrderStatus.Processing; // adjust to whatever "paid" status NBApp uses
                await _context.SaveChangesAsync();
                TempData["Message"] = "Payment successful!";
                return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
            }

            // Payment failed or was cancelled — restore stock and mark the order accordingly.
            var orderItems = await _context.OrderItem.Where(oi => oi.OrderId == order.OrderId).ToListAsync();
            foreach (var item in orderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                    product.StockQuantity += item.Quantity;
            }

            order.Status = OrderStatus.Cancelled; // adjust to whatever "failed/cancelled" status NBApp uses
            await _context.SaveChangesAsync();

            TempData["Error"] = "Payment was not completed.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Cart/OrderConfirmation
        [Authorize]
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingAddress)
                .ThenInclude(sa => sa.Suburb)
                .ThenInclude(s => s.City)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // GET: Cart/PaymentSuccess
        [Authorize]
        public async Task<IActionResult> PaymentSuccess(string buildingNumber, string street, int suburbId)
        {
            var cart = GetCart();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!cart.IsEmpty && userId != null)
            {
                var suburb = await _context.Suburbs.Include(s => s.City).FirstOrDefaultAsync(s => s.SuburbID == suburbId);
                if (suburb == null)
                    return RedirectToAction(nameof(Index));

                var shippingAddress = new ShippingAddress
                {
                    BuildingNumber = buildingNumber ?? string.Empty,
                    Street = street ?? string.Empty,
                    SuburbID = suburbId
                };

                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    TotalAmount = cart.Total + (suburb.DeliveryCost ?? 0m),
                    Status = OrderStatus.Pending,
                    ShippingAddress = shippingAddress
                };

                foreach (var cartItem in cart.Items)
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Price
                    });

                    var product = await _context.Products.FindAsync(cartItem.ProductId);
                    if (product != null)
                        product.StockQuantity -= cartItem.Quantity;
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                HttpContext.Session.Remove(CartSessionKey);

                return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
            }

            return RedirectToAction(nameof(Index));
        }

        // Webhook (not yet configured)
        [HttpPost]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var webhookSecret = _config["Stripe:WebhookSecret"];

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    webhookSecret
                );

                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    // save order here as a fallback
                }

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        private void SaveCart(CartViewModel cart) =>
            HttpContext.Session.SetObject(CartSessionKey, cart);

        private CartViewModel GetCart() =>
            HttpContext.Session.GetObject<CartViewModel>(CartSessionKey) ?? new CartViewModel();
    }
}