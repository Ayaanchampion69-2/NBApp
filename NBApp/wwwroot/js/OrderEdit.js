document.addEventListener('DOMContentLoaded', function () {
    const suburbsDataEl = document.getElementById('suburbs-data');
    if (!suburbsDataEl) return;

    const allSuburbs = JSON.parse(suburbsDataEl.textContent);
    const citySelect = document.getElementById('citySelect');
    const suburbSelect = document.getElementById('suburbSelect');
    const deliveryCostEl = document.getElementById('delivery-cost');

    const currentSuburbId = parseInt(suburbSelect.dataset.currentValue) || null;

    function populateSuburbs(cityId, preselectId) {
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
            if (preselectId && s.SuburbID === preselectId) {
                opt.selected = true;
            }
            suburbSelect.appendChild(opt);
        });

        suburbSelect.disabled = false;

        const selected = suburbSelect.options[suburbSelect.selectedIndex];
        const cost = selected && selected.dataset.cost ? parseFloat(selected.dataset.cost) : null;
        updateDelivery(cost);
    }

    function updateDelivery(cost) {
        deliveryCostEl.textContent = cost !== null && !isNaN(cost) ? '$' + cost.toFixed(2) : '—';
    }

    citySelect.addEventListener('change', function () {
        populateSuburbs(parseInt(this.value) || null, null);
    });

    suburbSelect.addEventListener('change', function () {
        const selected = this.options[this.selectedIndex];
        const cost = selected && selected.dataset.cost ? parseFloat(selected.dataset.cost) : null;
        updateDelivery(cost);
    });

    // Preselect the order's current city/suburb on page load
    if (citySelect.value) {
        populateSuburbs(parseInt(citySelect.value), currentSuburbId);
    }
});