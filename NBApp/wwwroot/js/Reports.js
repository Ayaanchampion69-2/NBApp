(() => {
    const MONTH_NAMES = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
        'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

    const STATUS_COLORS = {
        Completed: '#639922',
        Pending: '#BA7517',
        Cancelled: '#E24B4A',
        Processing: '#185FA5',
        Shipped: '#0F6E56'
    };

    let barChart = null;
    let pieChart = null;

    function fjd(amount) {
        return 'FJD ' + Number(amount).toLocaleString('en-FJ', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
    }

    function getFilters() {
        return {
            months: document.getElementById('rangeFilter').value,
            status: document.getElementById('statusFilter').value
        };
    }

    function setLoading(loading) {
        const spinner = document.getElementById('loadingSpinner');
        if (spinner) spinner.classList.toggle('hidden', !loading);
    }

    async function fetchJson(url) {
        const res = await fetch(url);
        if (!res.ok) throw new Error('Request failed: ' + url);
        return res.json();
    }

    async function loadStatCards(months) {
        const data = await fetchJson(`/Reports/StatCards?months=${months}`);
        document.getElementById('statRevenue').textContent = fjd(data.totalRevenue);
        document.getElementById('statRevSub').textContent = data.totalOrders + ' orders in period';
        document.getElementById('statOrders').textContent = data.totalOrders.toLocaleString();
        document.getElementById('statOrderSub').textContent = 'last ' + months + ' months';
        document.getElementById('statAvg').textContent = fjd(data.avgOrderValue);
        document.getElementById('statPending').textContent = data.pendingOrders.toLocaleString();
    }

    async function loadBarChart(months, status) {
        const data = await fetchJson(`/Reports/RevenueByMonth?months=${months}&status=${status}`);

        const labels = data.map(d => MONTH_NAMES[d.month - 1]);
        const values = data.map(d => parseFloat(d.revenue.toFixed(2)));

        const statusLabel = status === 'All' ? 'All statuses' : status;
        const sub = document.getElementById('barSub');
        if (sub) sub.textContent = `${statusLabel} · last ${months} months`;

        const legendEl = document.getElementById('barLegend');
        if (legendEl) {
            legendEl.innerHTML = `
                <span style="display:flex;align-items:center;gap:5px;">
                    <span style="width:10px;height:10px;border-radius:2px;background:#4F46E5;display:inline-block;"></span>
                    Revenue (FJD)
                </span>`;
        }

        if (barChart) barChart.destroy();

        barChart = new Chart(document.getElementById('barChart'), {
            type: 'bar',
            data: {
                labels,
                datasets: [{
                    label: 'Revenue (FJD)',
                    data: values,
                    backgroundColor: '#4F46E5',
                    borderRadius: 4,
                    borderSkipped: false
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: {
                    x: {
                        grid: { display: false },
                        ticks: { font: { size: 11 }, autoSkip: false, maxRotation: 45 }
                    },
                    y: {
                        grid: { color: 'rgba(0,0,0,0.05)' },
                        ticks: {
                            font: { size: 11 },
                            callback: v => 'FJD ' + v.toLocaleString()
                        }
                    }
                }
            }
        });
    }

    async function loadPieChart(months) {
        const data = await fetchJson(`/Reports/OrdersByStatus?months=${months}`);

        const labels = data.map(d => d.status);
        const values = data.map(d => d.count);
        const colors = labels.map(l => STATUS_COLORS[l] || '#888780');

        const legendEl = document.getElementById('pieLegend');
        if (legendEl) {
            legendEl.innerHTML = labels.map((l, i) => `
                <span style="display:flex;align-items:center;gap:6px;">
                    <span style="width:10px;height:10px;border-radius:2px;background:${colors[i]};display:inline-block;flex-shrink:0;"></span>
                    <span>${l}</span>
                    <span style="margin-left:auto;font-weight:500;color:#374151;">${values[i]}</span>
                </span>`).join('');
        }

        if (pieChart) pieChart.destroy();

        pieChart = new Chart(document.getElementById('pieChart'), {
            type: 'doughnut',
            data: {
                labels,
                datasets: [{
                    data: values,
                    backgroundColor: colors,
                    borderWidth: 0,
                    hoverOffset: 4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '65%',
                plugins: { legend: { display: false } }
            }
        });
    }

    async function loadTopProducts(months) {
        const data = await fetchJson(`/Reports/TopProducts?months=${months}`);
        const tbody = document.getElementById('productTableBody');

        if (!data.length) {
            tbody.innerHTML = `<tr><td colspan="4" class="text-center text-gray-400 py-8 text-sm">No data for this period.</td></tr>`;
            return;
        }

        tbody.innerHTML = data.map(p => `
            <tr class="border-b border-gray-50 hover:bg-gray-50 transition-colors">
                <td class="py-3 font-medium text-gray-900">${escapeHtml(p.name)}</td>
                <td class="py-3 text-gray-500">${escapeHtml(p.category)}</td>
                <td class="py-3 text-right text-gray-900">${p.unitsSold.toLocaleString()}</td>
                <td class="py-3 text-right text-gray-900">${fjd(p.revenue)}</td>
            </tr>`).join('');
    }

    function escapeHtml(str) {
        const d = document.createElement('div');
        d.appendChild(document.createTextNode(str ?? ''));
        return d.innerHTML;
    }

    async function updateAll() {
        const { months, status } = getFilters();
        setLoading(true);
        try {
            await Promise.all([
                loadStatCards(months),
                loadBarChart(months, status),
                loadPieChart(months),
                loadTopProducts(months)
            ]);
        } catch (err) {
            console.error('Reports load error:', err);
        } finally {
            setLoading(false);
        }
    }

    document.addEventListener('DOMContentLoaded', () => {
        document.getElementById('rangeFilter').addEventListener('change', updateAll);
        document.getElementById('statusFilter').addEventListener('change', updateAll);
        updateAll();
    });
})();