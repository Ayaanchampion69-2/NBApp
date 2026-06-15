using Microsoft.AspNetCore.Mvc;

public class PaymentController : Controller
{
    private readonly StripeService _stripeService;
    private readonly IConfiguration _config;

    public PaymentController(StripeService stripeService, IConfiguration config)
    {
        _stripeService = stripeService;
        _config = config;
    }

    // Show a "Pay Now" page
    public IActionResult Checkout(int orderId, decimal amount)
    {
        ViewBag.PublishableKey = _config["Stripe:PublishableKey"];
        ViewBag.OrderId = orderId;
        ViewBag.Amount = amount;
        return View();
    }

    [HttpPost]
    public IActionResult CreateSession(int orderId, decimal amount)
    {
        var successUrl = Url.Action("Success", "Payment", new { orderId }, Request.Scheme);
        var cancelUrl = Url.Action("Cancel", "Payment", null, Request.Scheme);

        var session = _stripeService.CreateCheckoutSession(
            $"Order #{orderId}",
            (long)(amount * 100),  // convert to cents
            successUrl,
            cancelUrl
        );

        return Redirect(session.Url); // redirect to Stripe hosted page
    }

    public IActionResult Success(int orderId)
    {
        // TODO: mark order as paid in your DB
        ViewBag.OrderId = orderId;
        return View();
    }

    public IActionResult Cancel()
    {
        return View();
    }
}