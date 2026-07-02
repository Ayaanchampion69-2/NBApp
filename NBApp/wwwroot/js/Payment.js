document.addEventListener('DOMContentLoaded', function () {
    const stripeData = document.getElementById('stripe-data');
    const publishableKey = stripeData.dataset.publishableKey;
    const clientSecret = stripeData.dataset.clientSecret;
    const baseReturnUrl = stripeData.dataset.returnUrl;
    const baseTotal = parseFloat(stripeData.dataset.baseTotal);

    const stripe = Stripe(publishableKey);
    const elements = stripe.elements({ clientSecret });
    const paymentElement = elements.create('payment');
    paymentElement.mount('#payment-element');

    // ── Suburb filtering ────────────────────────────────────────────────────
    const allSuburbs = JSON.parse(document.getElementById('suburbs-data').textContent);
    const citySelect = document.getElementById('citySelect');
    const suburbSelect = document.getElementById('suburbId');
    const deliveryRow = document.getElementById('delivery-row');
    const deliveryCostEl = document.getElementById('delivery-cost');
    const orderTotalEl = document.getElementById('order-total');

    citySelect.addEventListener('change', function () {
        const cityId = parseInt(this.value);
        suburbSelect.innerHTML = '<option value="">-- Select suburb --</option>';

        if (!cityId) {
            suburbSelect.disabled = true;
            updateDelivery(null);
            return;
        }

        const filtered = allSuburbs.filter(s => s.CityID === cityId);
        filtered.forEach(s => {
            const opt = document.createElement('option');
            opt.value = s.SuburbID;
            opt.textContent = s.SuburbName;
            opt.dataset.cost = s.DeliveryCost;
            suburbSelect.appendChild(opt);
        });

        suburbSelect.disabled = false;
        updateDelivery(null);
    });

    suburbSelect.addEventListener('change', function () {
        const selected = this.options[this.selectedIndex];
        const cost = selected && selected.dataset.cost ? parseFloat(selected.dataset.cost) : null;
        updateDelivery(cost);
    });

    function updateDelivery(cost) {
        if (cost !== null && !isNaN(cost)) {
            deliveryRow.style.display = 'flex';
            deliveryCostEl.textContent = '$' + cost.toFixed(2);
            orderTotalEl.textContent = '$' + (baseTotal + cost).toFixed(2);
        } else {
            deliveryRow.style.display = 'none';
            deliveryCostEl.textContent = '$0.00';
            orderTotalEl.textContent = '$' + baseTotal.toFixed(2);
        }
    }

    // ── Stripe payment ──────────────────────────────────────────────────────
    document.getElementById('pay-btn').addEventListener('click', async () => {
        const btn = document.getElementById('pay-btn');
        const form = document.getElementById('checkout-form');

        if (!form.checkValidity()) {
            form.reportValidity();
            return;
        }

        const buildingNumber = encodeURIComponent(document.getElementById('buildingNumber').value);
        const street = encodeURIComponent(document.getElementById('street').value);
        const suburbId = encodeURIComponent(suburbSelect.value);

        const returnUrl = `${baseReturnUrl}?buildingNumber=${buildingNumber}&street=${street}&suburbId=${suburbId}`;

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