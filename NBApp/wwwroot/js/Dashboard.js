function filterUsers() {
    const search = document.getElementById('search-input').value.toLowerCase().trim();
    const roleFilter = document.getElementById('role-filter').value.toLowerCase();
    const rows = document.querySelectorAll('#user-tbody tr[data-username]');
    let visible = 0;

    rows.forEach(row => {
        const username = row.dataset.username;
        const roles = row.dataset.roles;

        const matchName = !search || username.includes(search);
        const matchRole = !roleFilter
            || (roleFilter === '__none__' && roles === '')
            || roles.split(',').includes(roleFilter);

        const show = matchName && matchRole;
        row.classList.toggle('hidden', !show);
        if (show) visible++;
    });

    const count = document.getElementById('result-count');
    count.textContent = visible < rows.length
        ? `${visible} of ${rows.length} users`
        : '';
}