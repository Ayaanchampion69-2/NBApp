document.addEventListener('DOMContentLoaded', function () {
    const publishableKey = document.getElementById('stripe-data').dataset.publishableKey;
    const clientSecret = document.getElementById('stripe-data').dataset.clientSecret;
    const returnUrl = document.getElementById('stripe-data').dataset.returnUrl;

    const stripe = Stripe(publishableKey);
    const elements = stripe.elements({ clientSecret });
    const paymentElement = elements.create('payment');
    paymentElement.mount('#payment-element');

    document.getElementById('pay-btn').addEventListener('click', async () => {
        const btn = document.getElementById('pay-btn');
        const form = document.getElementById('checkout-form');

        if (!form.checkValidity()) {
            form.reportValidity();
            return;
        }

        btn.disabled = true;
        btn.textContent = 'Processing...';

        const { error } = await stripe.confirmPayment({
            elements,
            confirmParams: {
                return_url: returnUrl
            }
        });

        if (error) {
            document.getElementById('error-message').textContent = error.message;
            document.getElementById('error-message').classList.remove('hidden');
            btn.disabled = false;
            btn.textContent = btn.dataset.label;
        }
    });
});