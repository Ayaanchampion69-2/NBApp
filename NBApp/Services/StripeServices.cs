using Stripe;

public class StripeService
{
    public PaymentIntent CreatePaymentIntent(long amountInCents, string currency = "fjd")
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = amountInCents,
            Currency = currency,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            }
        };

        var service = new PaymentIntentService();
        return service.Create(options);
    }
}