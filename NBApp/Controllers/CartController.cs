using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NBApp.Areas.Identity.Data;
using NBApp.Models;
using NBApp.Services;
using System.Security.Claims;
using static NBApp.Models.Order;

public class CartController(NBAppContext context, StripeService stripeService, MPaisaService mpaisaService, IConfiguration config) : Controller
{
    private readonly NBAppContext _context = context;
    private readonly StripeService _stripeService = stripeService;
    private readonly MPaisaService _mpaisaService = mpaisaService;
    private readonly IConfiguration _config = config;
    private const string CartSessionKey = "ShoppingCart";

    // POST: Cart/MPaisaCheckout
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MPaisaCheckout(string buildingNumber, string street, int suburbId)
    {
        var cart = GetCart();
        if (cart.IsEmpty)
        {
            TempData["Error"] = "Your cart is empty!";
            return RedirectToAction(nameof(Index));
        }

        var suburb = await _context.Suburbs.FirstOrDefaultAsync(s => s.SuburbID == suburbId);
        if (suburb == null)
        {
            TempData["Error"] = "Invalid suburb selected.";
            return RedirectToAction(nameof(Checkout));
        }

        var total = cart.Total + (suburb.DeliveryCost ?? 0m);

        // temporary reference so we can rebuild the order after the redirect
        var tempRef = DateTime.Now.Ticks;

        var returnUrl = Url.Action("MPaisaReturn", "Cart", new
        {
            buildingNumber,
            street,
            suburbId
        }, Request.Scheme);

        var checkoutUrl = _mpaisaService.BuildCheckoutUrl((int)(tempRef % int.MaxValue), total, returnUrl!);
        return Redirect(checkoutUrl);
    }

    // GET: Cart/MPaisaReturn
    [Authorize]
    public async Task<IActionResult> MPaisaReturn(string buildingNumber, string street, int suburbId)
    {
        if (!_mpaisaService.VerifyCallback(Request.Query))
        {
            TempData["Error"] = "Payment verification failed.";
            return RedirectToAction(nameof(Checkout));
        }

        if (Request.Query["status"] != "Success")
        {
            TempData["Error"] = "Payment was not successful.";
            return RedirectToAction(nameof(Checkout));
        }

        return await CreateOrderFromCart(buildingNumber, street, suburbId);
    }

    // shared by Stripe's PaymentSuccess and MPaisaReturn
    private async Task<IActionResult> CreateOrderFromCart(string buildingNumber, string street, int suburbId)
    {
        var cart = GetCart();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (cart.IsEmpty || userId == null)
            return RedirectToAction(nameof(Index));

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

    // your existing PaymentSuccess can now just call:
    // return await CreateOrderFromCart(buildingNumber, street, suburbId);
}