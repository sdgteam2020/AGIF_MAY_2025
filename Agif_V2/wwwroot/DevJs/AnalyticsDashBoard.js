$(function () {
    // ---- DOM refs ----
    const $yearSelect = $('#yearSelect');
    const currentYear = new Date().getFullYear();

    // year dropdown
    for (let y = currentYear; y >= 2000; y--) $yearSelect.append(new Option(y, y));
    $yearSelect.val(currentYear);

    // ---- Chart 1: Applications by Month ----
    const appCtx = $('#applicationByMonth')[0].getContext('2d');
    const applicationsMonthlyChart = new Chart(appCtx, {
        type: 'bar',
        data: {
            labels: [], datasets: [
                { label: 'Car Applications', data: [], backgroundColor: '#007bff', borderColor: '#ffffff', borderWidth: 2 },
                { label: 'PC Applications', data: [], backgroundColor: '#28a745', borderColor: '#ffffff', borderWidth: 2 },
                { label: 'HBA Applications', data: [], backgroundColor: '#ffc107', borderColor: '#ffffff', borderWidth: 2 },
                { label: 'Total Applications', data: [], backgroundColor: '#dc3545', borderColor: '#ffffff', borderWidth: 2 }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { title: { display: true, text: 'Applications by Month', font: { size: 16 } }, legend: { position: 'top' } },
            scales: { y: { beginAtZero: true, title: { display: true, text: 'Applications Count' } }, x: { title: { display: true, text: 'Month' } } },
            animation: { duration: 2000, easing: 'easeInOutQuart' }
        }
    });
    
    // ---- Chart 2: Top Ranks (Rank on X, Count on Y) ----
    const topCtx = $('#topRanksChart')[0].getContext('2d');
    const topRanksChart = new Chart(topCtx, {
        type: 'bar',
        data: {
            labels: [],
            datasets: [
                {
                    label: 'Count',
                    data: [],
                    backgroundColor: '#17a2b8',
                    borderColor: '#ffffff',
                    borderWidth: 2
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: { display: true, text: 'Top 10 Ranks', font: { size: 16 } },
                legend: { display: false }
            },
            scales: {
                x: { title: { display: true, text: 'Rank' } },
                y: { beginAtZero: true, title: { display: true, text: 'Count' } }
            },
            animation: { duration: 1200, easing: 'easeInOutQuart' }
        }
    });

    // ---- Chart 3: Top Regt/Corps (Regt on X, Count on Y) ----
    const topregtCtx = $('#topRegtChart')[0].getContext('2d');
    const topRegtChart = new Chart(topregtCtx, {
        type: 'bar',
        data: {
            labels: [],
            datasets: [
                {
                    label: 'Count',
                    data: [],
                    backgroundColor: '#17a2b8',
                    borderColor: '#ffffff',
                    borderWidth: 2
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: { display: true, text: 'Top 10 Regt/Corps', font: { size: 16 } },
                legend: { display: false }
            },
            scales: {
                x: { title: { display: true, text: 'Regt/Corps' } },
                y: { beginAtZero: true, title: { display: true, text: 'Count' } }
            },
            animation: { duration: 1200, easing: 'easeInOutQuart' }
        }
    });


    // ---- Data loader (both charts) ----
    function loadChartData(year) {
        $.getJSON(`/Home/GetApplicationAnalytics?year=${year}`, function (resp) {
            if (!resp || !resp.success) {
                alert(resp?.message || "Failed to load data");
                return;
            }
            console.log(resp);
            // Chart 1: monthly
            const monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

            // Ensure months are in order 1..12
            const monthly = (resp.data.monthlyApplications || [])
                .map(m => ({
                    month: Number(m.month),
                    caCount: Number(m.caCount || 0),
                    pcaCount: Number(m.pcaCount || 0),
                    hbaCount: Number(m.hbaCount || 0),
                    totalApplications: Number(m.totalApplications || 0)
                }))
                .sort((a, b) => a.month - b.month);

            applicationsMonthlyChart.data.labels = monthly.map(x => monthNames[(x.month || 1) - 1] || '');
            applicationsMonthlyChart.data.datasets[0].data = monthly.map(x => x.caCount);
            applicationsMonthlyChart.data.datasets[1].data = monthly.map(x => x.pcaCount);
            applicationsMonthlyChart.data.datasets[2].data = monthly.map(x => x.hbaCount);
            applicationsMonthlyChart.data.datasets[3].data = monthly.map(x => x.totalApplications);
            applicationsMonthlyChart.update();

            // Chart 2: top ranks
            const ranksRaw = resp.data.topRanks || [];
            const ranksSorted = ranksRaw
                .map(r => ({ rank: r.rank ?? 'N/A', count: Number(r.rankCount || 0) }))
                .sort((a, b) => b.count - a.count)
                .slice(0, 10);

            topRanksChart.data.labels = ranksSorted.map(x => x.rank);
            topRanksChart.data.datasets[0].data = ranksSorted.map(x => x.count);
            topRanksChart.update();

            // Chart 3: top Regt/Corps
            const regtRaw = resp.data.topRegiments || [];
            console.log(regtRaw);
            const regtSorted = regtRaw
                .map(r => ({ regt: r.regt ?? 'N/A', count: Number(r.regtCount || 0) }))
                .sort((a, b) => b.count - a.count)
                .slice(0, 10);

            topRegtChart.data.labels = regtSorted.map(x => x.regt);
            topRegtChart.data.datasets[0].data = regtSorted.map(x => x.count);
            topRegtChart.update();
        });
    }

    // initial + year change
    loadChartData(currentYear);
    $yearSelect.on('change', function () { loadChartData($(this).val()); });
});