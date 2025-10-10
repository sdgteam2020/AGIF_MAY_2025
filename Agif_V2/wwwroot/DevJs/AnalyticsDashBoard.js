$(function () {
    // ---- DOM refs ----
    const $yearSelect = $('#yearSelect');
    const currentYear = new Date().getFullYear();

    // ---- helpers ----
    function formatCurrency(n) {
        return Number(n).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }



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
            scales: { y: { beginAtZero: true, title: { display: true, text: 'Count', font: { weight: 'bold', size: 14 } } }, x: { title: { display: true, text: 'Month', font: { weight: 'bold', size: 14 } } } },
            animation: { duration: 2000, easing: 'easeInOutQuart' }
        },
        plugins: [{
            id: 'customLabels',
            afterDatasetsDraw: function (chart) {
                const ctx = chart.ctx;
                chart.data.datasets.forEach(function (dataset, i) {
                    const meta = chart.getDatasetMeta(i);
                    if (!meta.hidden) {
                        meta.data.forEach(function (element, index) {
                            // Draw the text
                            ctx.fillStyle = '#000';
                            ctx.font = 'bold 12px Arial';
                            ctx.textAlign = 'center';
                            ctx.textBaseline = 'bottom';

                            const dataString = dataset.data[index].toString();
                            ctx.fillText(dataString, element.x, element.y - 5);
                        });
                    }
                });
            }
        }]
    });
    
    // ---- Chart 2: Top Ranks (Rank on X, Count on Y) ----
    const topCtx = $('#topRanksChart')[0].getContext('2d');
    //const topRanksChart = new Chart(topCtx, {
    //    type: 'bar',
    //    data: {
    //        labels: [],
    //        datasets: [
    //            {
    //                label: 'Count',
    //                data: [],
    //                backgroundColor: '#17a2b8',
    //                borderColor: '#ffffff',
    //                borderWidth: 2
    //            }
    //        ]
    //    },
    //    options: {
    //        responsive: true,
    //        maintainAspectRatio: false,
    //        plugins: {
    //            title: { display: true, text: 'Top 10 Ranks', font: { size: 16 } },
    //            legend: { display: false }
    //        },
    //        scales: {
    //            x: { title: { display: true, text: 'Rank' } },
    //            y: { beginAtZero: true, title: { display: true, text: 'Count' } }
    //        },
    //        animation: { duration: 1200, easing: 'easeInOutQuart' }
    //    }
    //});

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
                title: {
                    display: true,
                    text: 'Top 10 Ranks',
                    font: { size: 16 }
                },
                legend: {
                    display: false
                }
            },
            scales: {
                x: {
                    title: { display: true, text: 'Rank', font: { weight: 'bold', size: 14 } }

                },
                y: {
                    beginAtZero: true,
                    title: { display: true, text: 'Count', font: { weight: 'bold', size: 14 } }
                }
            },
            animation: {
                duration: 1200,
                easing: 'easeInOutQuart'
            }
        },
        plugins: [{
            id: 'customLabels',
            afterDatasetsDraw: function (chart) {
                const ctx = chart.ctx;
                chart.data.datasets.forEach(function (dataset, i) {
                    const meta = chart.getDatasetMeta(i);
                    if (!meta.hidden) {
                        meta.data.forEach(function (element, index) {
                            // Draw the text
                            ctx.fillStyle = '#000';
                            ctx.font = 'bold 12px Arial';
                            ctx.textAlign = 'center';
                            ctx.textBaseline = 'bottom';

                            const dataString = dataset.data[index].toString();
                            ctx.fillText(dataString, element.x, element.y - 5);
                        });
                    }
                });
            }
        }]
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
                x: { title: { display: true, text: 'Regt/Corps', font: { weight: 'bold', size: 14 } } },
                y: { beginAtZero: true, title: { display: true, text: 'Count', font: { weight: 'bold', size: 14 } } }
            },
            animation: { duration: 1200, easing: 'easeInOutQuart' }
        },
        plugins: [{
            id: 'customLabels',
            afterDatasetsDraw: function (chart) {
                const ctx = chart.ctx;
                chart.data.datasets.forEach(function (dataset, i) {
                    const meta = chart.getDatasetMeta(i);
                    if (!meta.hidden) {
                        meta.data.forEach(function (element, index) {
                            // Draw the text
                            ctx.fillStyle = '#000';
                            ctx.font = 'bold 12px Arial';
                            ctx.textAlign = 'center';
                            ctx.textBaseline = 'bottom';

                            const dataString = dataset.data[index].toString();
                            ctx.fillText(dataString, element.x, element.y - 5);
                        });
                    }
                });
            }
        }]
    });


    // ---- Chart 4: Loan Statistics by Application Type & Vehicle Loan Type ----
    const loanCtx = $('#loanStatisticsChart')[0].getContext('2d');

    const loanStatisticsChart = new Chart(loanCtx, {
        type: 'bar',
        data: {
            labels: [],
            datasets: [{
                label: 'Loan Count',
                data: [],
                backgroundColor: '#007bff',
                borderColor: '#ffffff',
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Loan Count by Application Type and Vehicle Loan Type',
                    font: { size: 16 }
                },
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const rawData = context.chart.data.datasets[0].rawData;
                            const item = rawData[context.dataIndex];
                            return [
                                'Loan Type: ' + item.loanType,
                                'Count: ' + item.loanCount,
                            ];
                        }
                    }
                }
            },
            scales: {
                x: {
                    title: {
                        display: true,
                        text: 'Vehicle Loan Type', font: { weight: 'bold', size: 14 }
                    }
                },
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Count', font: { weight: 'bold', size: 14 }
                    },
                    ticks: {
                        stepSize: 1
                    }
                }
            },
            animation: {
                duration: 1200,
                easing: 'easeInOutQuart',

            }
        },
        plugins: [{
            id: 'customLabels',
            afterDatasetsDraw: function (chart) {
                const ctx = chart.ctx;
                chart.data.datasets.forEach(function (dataset, i) {
                    const meta = chart.getDatasetMeta(i);
                    if (!meta.hidden) {
                        meta.data.forEach(function (element, index) {
                            // Draw the text
                            ctx.fillStyle = '#000';
                            ctx.font = 'bold 12px Arial';
                            ctx.textAlign = 'center';
                            ctx.textBaseline = 'bottom';

                            const dataString = dataset.data[index].toString();
                            ctx.fillText(dataString, element.x, element.y - 5);
                        });
                    }
                });
            }
        }]
    });

    // ---- Chart 5=: Top 10 Units by Number of Applications ----
    const topUnitsCtx = $('#topUnitsChart')[0].getContext('2d');

    // ---- Chart: Top 10 Units (Horizontal Bar) ----
    const topUnitsChart = new Chart(topUnitsCtx, {
        type: 'bar',
        data: {
            labels: [],
            datasets: [{
                label: 'Application Count',
                data: [],
                backgroundColor: [],
                borderColor: '#ffffff',
                borderWidth: 2
            }]
        },
        options: {
            indexAxis: 'y', // This makes it horizontal
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Top 10 Units ',
                    font: { size: 16 }
                },
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return 'Applications: ' + context.parsed.x;
                        }
                    }
                }
            },
            scales: {
                x: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Number of Applications', font: { weight: 'bold', size: 14 }
                    },
                     ticks: {
                        maxRotation: 0,
                        minRotation: 0,
                         autoSkip: false,
                         stepSize:2,
                        font: { size: 10 }  // Slightly smaller font to fit better
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'Unit Name', font: { weight: 'bold', size: 14 }
                    }
                }
            },
            animation: {
                duration: 1200,
                easing: 'easeInOutQuart'
            }
        },
        plugins: [{
            id: 'customLabels',
            afterDatasetsDraw: function (chart) {
                const ctx = chart.ctx;
                chart.data.datasets.forEach(function (dataset, i) {
                    const meta = chart.getDatasetMeta(i);
                    if (!meta.hidden) {
                        meta.data.forEach(function (element, index) {
                            // Draw the text
                            ctx.fillStyle = '#000';
                            ctx.font = 'bold 12px Arial';
                            ctx.textAlign = 'center';
                            ctx.textBaseline = 'bottom';

                            const dataString = dataset.data[index].toString();
                            ctx.fillText(dataString, element.x, element.y - 5);
                        });
                    }
                });
            }
        }]
    });

    //chart 6
    const toploanUnitsCtx = $('#topUnitsLoanChart')[0].getContext('2d');

    // ---- Chart: Top 10 Units by Total Loan Amount (Horizontal Bar) ----
    //const topUnitsLoanChart = new Chart(toploanUnitsCtx, {
    //    type: 'bar',
    //    data: {
    //        labels: [],   // Unit names
    //        datasets: [
    //            { label: 'Car Loan', data: [], backgroundColor: '#007bff' },
    //            { label: 'PCA/Computer Loan', data: [], backgroundColor: '#28a745' },
    //            { label: 'HBA Loan', data: [], backgroundColor: '#ffc107' },
    //            { label: 'Total Loan', data: [], backgroundColor: '#dc3545' }

    //        ]
    //    },
    //    options: {
    //        responsive: true,
    //        maintainAspectRatio: false,
    //        plugins: {
    //            title: { display: true, text: 'Top 10 Units by Total Loan Amount', font: { size: 16 } },
    //            legend: { position: 'top' }
    //        },
    //        scales: {
    //            y: {
    //                beginAtZero: true,
    //                title: { display: true, text: 'Loan Amount' }
    //            },
    //            x: {
    //                title: { display: true, text: 'Unit' },
    //                stacked: false  // optional: true if you want stacked bars
    //            }
    //        },
    //        animation: { duration: 1500, easing: 'easeInOutQuart' }
    //    }
    //});
    //const topUnitsLoanChart = new Chart(toploanUnitsCtx, {
    //    type: 'bar',
    //    data: {
    //        labels: [],   // Unit names
    //        datasets: [
    //            { label: 'Car Loan', data: [], backgroundColor: 'rgba(0,123,255,0.85)' },
    //            { label: 'PCA/Computer Loan', data: [], backgroundColor: 'rgba(40,167,69,0.85)' },
    //            { label: 'HBA Loan', data: [], backgroundColor: 'rgba(255,193,7,0.85)' },
    //            { label: 'Total Loan', data: [], backgroundColor: 'rgba(220,53,69,0.85)' }
    //        ]
    //    },
    //    options: {
    //        responsive: true,
    //        maintainAspectRatio: false,
    //        plugins: {
    //            title: { display: true, text: 'Top 10 Units by Total Loan Amount', font: { size: 16 } },
    //            legend: { position: 'top' },
    //            tooltip: {
    //                mode: 'index',
    //                intersect: false,
    //                callbacks: {
    //                    label: function (context) {
    //                        // For vertical bar: parsed.y has numeric value
    //                        const val = context.parsed?.y ?? context.parsed?.x ?? 0;
    //                        return (context.dataset.label || '') + ': ₹' + formatCurrency(val);
    //                    },
    //                    afterBody: function (items) {
    //                        // Will be filled per-update (we attach rawData to datasets later)
    //                        const idx = items && items[0] && items[0].dataIndex ? items[0].dataIndex : null;
    //                        if (idx !== null && topUnitsLoanChart.rawLoanData && topUnitsLoanChart.rawLoanData[idx]) {
    //                            return 'Applications: ' + (topUnitsLoanChart.rawLoanData[idx].totalApplications || 0);
    //                        }
    //                        return '';
    //                    }
    //                }
    //            }
    //        },
    //        scales: {
    //            y: {
    //                beginAtZero: true,
    //                title: { display: true, text: 'Loan Amount (₹)', font: { weight: 'bold', size: 14 } }
    //            },
    //            x: {
    //                title: { display: true, text: 'Unit Name', font: { weight: 'bold', size: 14 } },
    //                stacked: false,
    //                ticks: {
    //                    maxRotation: 0,
    //                    minRotation: 0,
    //                    autoSkip: false,
    //                    font: { size: 12 }  // Slightly smaller font to fit better
    //                }
    //            }
    //        },
    //        animation: { duration: 2000, easing: 'easeInOutQuart' }
    //    },
    //    plugins: [{
    //        id: 'customLabels',
    //        afterDatasetsDraw: function (chart) {
    //            const ctx = chart.ctx;
    //            chart.data.datasets.forEach(function (dataset, i) {
    //                const meta = chart.getDatasetMeta(i);
    //                if (!meta.hidden) {
    //                    meta.data.forEach(function (element, index) {
    //                        // Draw the text
    //                        ctx.fillStyle = '#000';
    //                        ctx.font = 'bold 12px Arial';
    //                        ctx.textAlign = 'center';
    //                        ctx.textBaseline = 'bottom';

    //                        const dataString = dataset.data[index].toString();
    //                        ctx.fillText(dataString, element.x, element.y - 5);
    //                    });
    //                }
    //            });
    //        }
    //    }]
    //});
    const topUnitsLoanChart = new Chart(toploanUnitsCtx, {
        type: 'bar',
        data: {
            labels: [],   // Unit names
            datasets: [
                { label: 'Car Loan', data: [], backgroundColor: 'rgba(0,123,255,0.85)', borderColor: '#ffffff', borderWidth: 2 },
                { label: 'PCA/Computer Loan', data: [], backgroundColor: 'rgba(40,167,69,0.85)', borderColor: '#ffffff', borderWidth: 2 },
                { label: 'HBA Loan', data: [], backgroundColor: 'rgba(255,193,7,0.85)', borderColor: '#ffffff', borderWidth: 2 },
                { label: 'Total Loan', data: [], backgroundColor: 'rgba(220,53,69,0.85)', borderColor: '#ffffff', borderWidth: 2 }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: { display: true, text: 'Top 10 Units by Total Loan Amount', font: { size: 16 } },
                legend: { position: 'top' },
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    callbacks: {
                        label: function (context) {
                            const val = context.parsed?.y ?? context.parsed?.x ?? 0;
                            return (context.dataset.label || '') + ': ₹' + val.toLocaleString('en-IN');
                        },
                        afterBody: function (items) {
                            const idx = items && items[0] && items[0].dataIndex ? items[0].dataIndex : null;
                            if (idx !== null && topUnitsLoanChart.rawLoanData && topUnitsLoanChart.rawLoanData[idx]) {
                                return 'Applications: ' + (topUnitsLoanChart.rawLoanData[idx].totalApplications || 0);
                            }
                            return '';
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    title: { display: true, text: 'Loan Amount (₹)', font: { weight: 'bold', size: 14 } },
                    ticks: {
                        callback: function (value) {
                            if (value >= 10000000) {
                                return '₹' + (value / 10000000).toFixed(1) + 'Cr';
                            } else if (value >= 100000) {
                                return '₹' + (value / 100000).toFixed(1) + 'L';
                            } else if (value >= 1000) {
                                return '₹' + (value / 1000).toFixed(0) + 'K';
                            }
                            return '₹' + value.toLocaleString('en-IN');

                          
                        }
                        
                    }
                },
                x: {
                    title: { display: true, text: 'Unit Name', font: { weight: 'bold', size: 14 } },
                    stacked: false,
                    ticks: {
                        maxRotation: 0,
                        minRotation: 0,
                        autoSkip: false,
                        font: { size: 12 }
                    }
                }
            },
            animation: { duration: 2000, easing: 'easeInOutQuart' }
        },
        plugins: [{
            id: 'customLabels',
            afterDatasetsDraw: function (chart) {
                const ctx = chart.ctx;
                chart.data.datasets.forEach(function (dataset, i) {
                    const meta = chart.getDatasetMeta(i);
                    if (!meta.hidden) {
                        meta.data.forEach(function (element, index) {
                            const value = dataset.data[index];

                            // Skip if value is 0 or too small
                            if (value < 1000) return;

                            // Format the value
                            let displayText;
                            if (value >= 10000000) {
                                displayText = '₹' + (value / 10000000).toFixed(1) + 'Cr';
                            } else if (value >= 100000) {
                                displayText = '₹' + (value / 100000).toFixed(1) + 'L';
                            } else if (value >= 1000) {
                                displayText = '₹' + (value / 1000).toFixed(0) + 'K';
                            } else {
                                displayText = '₹' + value;
                            }

                            // Draw the text
                            ctx.fillStyle = '#000';
                            ctx.font = 'bold 10px Arial';
                            ctx.textAlign = 'center';
                            ctx.textBaseline = 'bottom';
                            ctx.fillText(displayText, element.x, element.y - 5);
                        });
                    }
                });
            }
        }]
    });


    //chart 7 Top 10 Dealers
    const topDealersCtx = $('#topDealersChart')[0].getContext('2d');
    //const topDealersChart = new Chart(topDealersCtx, {
    //    type: 'doughnut',
    //    data: {
    //        labels: [], // Dealer names
    //        datasets: [{
    //            label: 'Number of Applications',
    //            data: [], // Application count per dealer
    //            backgroundColor: [
    //                'rgba(0, 123, 255, 0.7)',
    //                'rgba(40, 167, 69, 0.7)',
    //                'rgba(255, 193, 7, 0.7)',
    //                'rgba(220, 53, 69, 0.7)',
    //                'rgba(23, 162, 184, 0.7)',
    //                'rgba(108, 117, 125, 0.7)',
    //                'rgba(255, 99, 132, 0.7)',
    //                'rgba(54, 162, 235, 0.7)',
    //                'rgba(255, 206, 86, 0.7)',
    //                'rgba(75, 192, 192, 0.7)'
    //            ],
    //            borderColor: '#ffffff',
    //            borderWidth: 2
    //        }]
    //    },
    //    options: {
    //        responsive: true,
    //        maintainAspectRatio: false,
    //        plugins: {
    //            title: {
    //                display: true,
    //                text: 'Top 10 Dealers by Number of Applications',
    //                font: { size: 16 }
    //            },
    //            legend: {
    //                position: 'right'
    //            },
    //            tooltip: {
    //                callbacks: {
    //                    label: function (context) {
    //                        return context.label + ': ' + context.parsed + ' applications';
    //                    }
    //                }
    //            }
    //        },
    //        animation: {
    //            duration: 1500,
    //            easing: 'easeInOutQuart'
    //        }
    //    }
    //});
    const topDealersChart = new Chart(topDealersCtx, {
        type: 'doughnut',
        data: {
            labels: [], // Dealer names
            datasets: [{
                label: 'Number of Applications',
                data: [], // Application count per dealer
                backgroundColor: [
                    'rgba(0, 123, 255, 0.7)',
                    'rgba(40, 167, 69, 0.7)',
                    'rgba(255, 193, 7, 0.7)',
                    'rgba(220, 53, 69, 0.7)',
                    'rgba(23, 162, 184, 0.7)',
                    'rgba(108, 117, 125, 0.7)',
                    'rgba(255, 99, 132, 0.7)',
                    'rgba(54, 162, 235, 0.7)',
                    'rgba(255, 206, 86, 0.7)',
                    'rgba(75, 192, 192, 0.7)'
                ],
                borderColor: '#ffffff',
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Top 10 Dealers by Number of Applications',
                    font: { size: 16 }
                },
                legend: {
                    position: 'right',
                    labels: {
                        boxWidth: 12,
                        font: { size: 12 },
                        generateLabels: function (chart) {
                            const data = chart.data;
                            const total = data.datasets[0].data.reduce((sum, val) => sum + val, 0);

                            return data.labels.map((label, i) => {
                                const value = data.datasets[0].data[i];
                                const percentage = ((value / total) * 100).toFixed(1);

                                return {
                                    text: label + ': ' + value + ' (' + percentage + '%)',
                                    fillStyle: data.datasets[0].backgroundColor[i],
                                    hidden: false,
                                    index: i
                                };
                            });
                        }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const total = context.dataset.data.reduce((sum, val) => sum + val, 0);
                            const percentage = ((context.parsed / total) * 100).toFixed(1);
                            return [
                                context.label,
                                'Applications: ' + context.parsed,
                                'Percentage: ' + percentage + '%'
                            ];
                        }
                    }
                }
            },
            animation: {
                duration: 1500,
                easing: 'easeInOutQuart',
                animateRotate: true,
                animateScale: true
            }
        },
        plugins: [{
            id: 'doughnutSliceLabels',
            afterDraw: function (chart) {
                const ctx = chart.ctx;
                const dataset = chart.data.datasets[0];
                const total = dataset.data.reduce((sum, val) => sum + val, 0);

                chart.getDatasetMeta(0).data.forEach((datapoint, index) => {
                    const value = dataset.data[index];
                    const percentage = ((value / total) * 100).toFixed(1);

                    // Get the position
                    const { x, y } = datapoint.tooltipPosition();

                    // Dynamic font size based on percentage
                    let fontSize = 12;
                    if (parseFloat(percentage) < 3) {
                        //fontSize = 10;
                        return;
                    }
                    

                    // Draw text background
                    ctx.fillStyle = 'rgba(255, 255, 255, 0.8)';
                    ctx.font = `bold ${fontSize}px Arial`;
                    const text1 = value.toString();
                    const text2 = '(' + percentage + '%)';
                    const text1Width = ctx.measureText(text1).width;
                    ctx.font = `normal ${fontSize - 1}px Arial`;
                    const text2Width = ctx.measureText(text2).width;
                    const maxWidth = Math.max(text1Width, text2Width);

                    ctx.fillRect(x - maxWidth / 2 - 4, y - 18, maxWidth + 8, 32);

                    // Draw count text
                    ctx.fillStyle = '#000';
                    ctx.font = `bold ${fontSize}px Arial`;
                    ctx.textAlign = 'center';
                    ctx.textBaseline = 'middle';
                    ctx.fillText(text1, x, y - 6);

                    // Draw percentage text
                    ctx.font = `normal ${fontSize - 1}px Arial`;
                    ctx.fillText(text2, x, y + 6);
                });
            }
        }]
    });


    //chart 8 Loan Amount for Top 10 Dealers

    //const topLoanDealers = $('#topLoanDealersChart')[0].getContext('2d');

    //const topLoanDealersChart = new Chart(topLoanDealers, {
    //    type: 'bar',
    //    data: {
    //        labels: [],   // Dealer names
    //        datasets: [{
    //            label: 'Loan Amount',
    //            data: [],   // Loan amounts
    //            backgroundColor: '#007bff',
    //            borderColor: '#ffffff',
    //            borderWidth: 2
    //        }]
    //    },
    //    options: {
    //        responsive: true,
    //        maintainAspectRatio: false,
    //        plugins: {
    //            title: {
    //                display: true,
    //                text: 'Top 10 Dealers by Loan Amount',
    //                font: { size: 16 }
    //            },
    //            legend: { display: false },
    //            tooltip: {
    //                callbacks: {
    //                    label: function (context) {
    //                        return 'Loan Amount: ' + context.parsed.y.toLocaleString();
    //                    }
    //                }
    //            }
    //        },
    //        scales: {
    //            x: {
    //                title: {
    //                    display: true, text: 'Dealer Name'
                       
    //                },
    //                ticks: {
    //                    maxRotation: 0,
    //                    minRotation: 0,
    //                    autoSkip: false,
    //                    font: { size: 8 }  // Slightly smaller font to fit better
    //                }
    //            },
    //            y: {
    //                beginAtZero: true,
    //                title: { display: true, text: 'Loan Amount (₹)' }
    //            }
    //        },
    //        animation: { duration: 1500, easing: 'easeInOutQuart' }
    //    }
    //});

    const topLoanDealers = $('#topLoanDealersChart')[0].getContext('2d');

    //const topLoanDealersChart = new Chart(topLoanDealers, {
    //    type: 'doughnut',  // Change chart type to doughnut
    //    data: {
    //        labels: [],   // Dealer names
    //        datasets: [{
    //            label: 'Loan Amount',
    //            data: [],   // Loan amounts
    //            backgroundColor: [], // Array of colors for each section
    //            borderColor: '#ffffff',
    //            borderWidth: 2
    //        }]
    //    },
    //    options: {
    //        responsive: true,
    //        maintainAspectRatio: false,
    //        plugins: {
    //            title: {
    //                display: true,
    //                text: 'Top 10 Dealers by Loan Amount',
    //                font: { size: 16 }
    //            },
    //            legend: {
    //                position: 'top',  // Show legend at the top
    //                labels: {
    //                    boxWidth: 12,  // Set the box width for the legend
    //                    font: {
    //                        size: 10  // Set font size for legend
    //                    }
    //                }
    //            },
    //            tooltip: {
    //                callbacks: {
    //                    label: function (context) {
    //                        return 'Loan Amount: ₹' + context.raw.toLocaleString();  // Display loan amount in tooltip
    //                    }
    //                }
    //            }
    //        },
    //        animation: { duration: 1500, easing: 'easeInOutQuart' },
    //        cutoutPercentage: 60,  // Adjust this for doughnut size (higher = thicker ring)
    //        rotation: -0.5 * Math.PI,  // Rotate the doughnut chart to start from the top
    //    }
    //});

    //const topLoanDealersChart = new Chart(topLoanDealers, {
    //    type: 'pie',  // Change chart type to pie
    //    data: {
    //        labels: [],   // Dealer names
    //        datasets: [{
    //            label: 'Loan Amount',
    //            data: [],   // Loan amounts
    //            backgroundColor: [], // Array of colors for each section
    //            borderColor: '#ffffff',
    //            borderWidth: 2
    //        }]
    //    },
    //    options: {
    //        responsive: true,
    //        maintainAspectRatio: false,
    //        plugins: {
    //            title: {
    //                display: true,
    //                text: 'Top 10 Dealers by Loan Amount',
    //                font: { size: 16 }
    //            },
    //            legend: {
    //                position: 'right',  // Show legend at the top
    //                labels: {
    //                    boxWidth: 12,  // Set the box width for the legend
    //                    font: {
    //                        size: 10  // Set font size for legend
    //                    }
    //                }
    //            },
    //            tooltip: {
    //                callbacks: {
    //                    label: function (context) {
    //                        return 'Loan Amount: ₹' + context.raw.toLocaleString();  // Display loan amount in tooltip
    //                    }
    //                }
    //            }
    //        },
    //        animation: { duration: 1500, easing: 'easeInOutQuart' }
    //    }
    //});
    const topLoanDealersChart = new Chart(topLoanDealers, {
        type: 'pie',
        data: {
            labels: [],
            datasets: [{
                label: 'Loan Amount',
                data: [],
                backgroundColor: [],
                borderColor: '#ffffff',
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Top 10 Dealers by Loan Amount',
                    font: { size: 16 }
                },
                legend: {
                    position: 'right',
                    labels: {
                        boxWidth: 12,
                        font: { size: 12 },
                        generateLabels: function (chart) {
                            const data = chart.data;
                            const total = data.datasets[0].data.reduce((sum, val) => sum + val, 0);

                            return data.labels.map((label, i) => {
                                const value = data.datasets[0].data[i];
                                const percentage = ((value / total) * 100).toFixed(1);

                                // Format amount in Indian format
                                let formattedAmount;
                                if (value >= 10000000) {
                                    formattedAmount = '₹' + (value / 10000000).toFixed(1) + 'Cr';
                                } else if (value >= 100000) {
                                    formattedAmount = '₹' + (value / 100000).toFixed(1) + 'L';
                                } else if (value >= 1000) {
                                    formattedAmount = '₹' + (value / 1000).toFixed(0) + 'K';
                                } else {
                                    formattedAmount = '₹' + value;
                                }

                                return {
                                    text: label + ': ' + formattedAmount + ' (' + percentage + '%)',
                                    fillStyle: data.datasets[0].backgroundColor[i],
                                    hidden: false,
                                    index: i
                                };
                            });
                        }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const total = context.dataset.data.reduce((sum, val) => sum + val, 0);
                            const percentage = ((context.raw / total) * 100).toFixed(1);
                            return [
                                context.label,
                                'Amount: ₹' + context.raw.toLocaleString('en-IN'),
                                'Percentage: ' + percentage + '%'
                            ];
                        }
                    }
                }
            },
            animation: {
                duration: 1500,
                easing: 'easeInOutQuart',
                animateRotate: true,
                animateScale: true
            }
        },
        plugins: [{
            id: 'pieSliceLabels',
            afterDraw: function (chart) {
                const ctx = chart.ctx;
                const dataset = chart.data.datasets[0];
                const total = dataset.data.reduce((sum, val) => sum + val, 0);

                chart.getDatasetMeta(0).data.forEach((datapoint, index) => {
                    const value = dataset.data[index];
                    const percentage = ((value / total) * 100).toFixed(1);

                    // Only show label if percentage > 5% to avoid clutter
                    if (parseFloat(percentage) < 3) return;

                    // Format amount in Indian format
                    let formattedAmount;
                    if (value >= 10000000) {
                        formattedAmount = '₹' + (value / 10000000).toFixed(1) + 'Cr';
                    } else if (value >= 100000) {
                        formattedAmount = '₹' + (value / 100000).toFixed(1) + 'L';
                    } else if (value >= 1000) {
                        formattedAmount = '₹' + (value / 1000).toFixed(0) + 'K';
                    } else {
                        formattedAmount = '₹' + value;
                    }

                    // Get the position of the label
                    const { x, y } = datapoint.tooltipPosition();

                    // Draw text background (optional - for better readability)
                    ctx.fillStyle = 'rgba(255, 255, 255, 0.8)';
                    ctx.font = 'bold 11px Arial';
                    const text1 = formattedAmount;
                    const text2 = '(' + percentage + '%)';
                    const text1Width = ctx.measureText(text1).width;
                    const text2Width = ctx.measureText(text2).width;
                    const maxWidth = Math.max(text1Width, text2Width);

                    ctx.fillRect(x - maxWidth / 2 - 4, y - 18, maxWidth + 8, 32);

                    // Draw amount text
                    ctx.fillStyle = '#000';
                    ctx.font = 'bold 11px Arial';
                    ctx.textAlign = 'center';
                    ctx.textBaseline = 'middle';
                    ctx.fillText(text1, x, y - 6);

                    // Draw percentage text
                    ctx.font = 'normal 10px Arial';
                    ctx.fillText(text2, x, y + 6);
                });
            }
        }]
    });

    //chart 9

    const topPersonnelCtx = $('#topPersonnelChart')[0].getContext('2d');

    const topPersonnelChart = new Chart(topPersonnelCtx, {
        type: 'bar',
        data: {
            labels: [], // Rank + ApplicantName
            datasets: [{
                label: 'Total Loan Amount',
                data: [],   // Total loan amounts
                backgroundColor: [],
                borderColor: '#ffffff',
                borderWidth: 2,
                rawData: [] // Store raw data for tooltip
            }]
        },
        options: {
            indexAxis: 'y', // Horizontal bar
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Top 20 Approved Personnel by Total Loan Amount',
                    font: { size: 16 }
                },
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const item = context.dataset.rawData[context.dataIndex];
                            return [
                                'Total Amount: ₹' + item.totalLoanAmount.toLocaleString('en-IN'),
                                'Car Loans: ₹' + item.totalCarLoan.toLocaleString('en-IN') + ' (' + item.carLoans + ')',
                                'HBA Loans: ₹' + item.totalHbaLoan.toLocaleString('en-IN') + ' (' + item.hbaLoans + ')',
                                'PCA Loans: ₹' + item.totalPcaLoan.toLocaleString('en-IN') + ' (' + item.pcaLoans + ')',
                                'Total Loans: ' + item.totalLoans
                            ];
                        }
                    }
                }
            },
            scales: {
                x: {
                    beginAtZero: true,
                    title: {
                        display: true, text: 'Total Loan Amount (₹)', font: { weight: 'bold', size: 14 }
                    },
                    ticks: {
                        callback: function (value) {
                            if (value >= 10000000) return '₹' + (value / 10000000).toFixed(1) + 'Cr';
                            if (value >= 100000) return '₹' + (value / 100000).toFixed(1) + 'L';
                            if (value >= 1000) return '₹' + (value / 1000).toFixed(0) + 'K';
                            return '₹' + value.toLocaleString('en-IN');
                        }
                    }
                },
                y: {
                    title: { display: true, text: 'Applicant Name', font: { weight: 'bold', size: 14 } }
                }
            },
            animation: { duration: 1500, easing: 'easeInOutQuart' }
        },
        plugins: [{
            id: 'customLabels',
            afterDatasetsDraw: function (chart) {
                const ctx = chart.ctx;
                chart.data.datasets.forEach(function (dataset, i) {
                    const meta = chart.getDatasetMeta(i);
                    if (!meta.hidden) {
                        meta.data.forEach(function (element, index) {
                            ctx.fillStyle = '#000';
                            ctx.font = 'bold 10px Arial';
                            ctx.textAlign = 'left';
                            ctx.textBaseline = 'middle';

                            const value = dataset.data[index];
                            let displayText;
                            if (value >= 10000000) displayText = '₹' + (value / 10000000).toFixed(1) + 'Cr';
                            else if (value >= 100000) displayText = '₹' + (value / 100000).toFixed(1) + 'L';
                            else displayText = '₹' + (value / 1000).toFixed(0) + 'K';

                            ctx.fillText(displayText, element.x + 5, element.y);
                        });
                    }
                });
            }
        }]
    });

    //chart 10
    const statusChartCtx = $('#applicationStatusChart')[0].getContext('2d');
    //const applicationStatusChart = new Chart(statusChartCtx, {
    //    type: 'doughnut',
    //    data: {
    //        labels: ['Pending', 'Approved', 'Rejected'],
    //        datasets: [{
    //            data: [], // Will be filled dynamically
    //            backgroundColor: [
    //                'rgba(255, 193, 7, 0.8)',   // Pending - Yellow
    //                'rgba(0, 123, 255, 0.8)',   // Approved - Blue
    //                'rgba(220, 53, 69, 0.8)'    // Rejected - Red
    //            ],
    //            borderColor: '#ffffff',
    //            borderWidth: 2
    //        }]
    //    },
    //    options: {
    //        responsive: true,
    //        plugins: {
    //            title: { display: true, text: 'Applications by Status', font: { size: 16 } },
    //            legend: { position: 'right' },
    //            tooltip: {
    //                callbacks: {
    //                    label: function (context) {
    //                        return `${context.label}: ${context.raw}`;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //});
    //const applicationStatusChart = new Chart(statusChartCtx, {
    //    type: 'bar',
    //    data: {
    //        labels: ['Applications'], // Single row
    //        datasets: [
    //            { label: 'Pending', data: [0], backgroundColor: 'rgba(255,193,7,0.8)', stack: 'Status' },
    //            { label: 'Approved', data: [0], backgroundColor: 'rgba(0,123,255,0.8)', stack: 'Status' },
    //            { label: 'Rejected', data: [0], backgroundColor: 'rgba(220,53,69,0.8)', stack: 'Status' },
    //            { label: 'Target', data: [0], backgroundColor: 'rgba(200,200,200,0.2)', stack: 'Target', barThickness: 30 }
    //        ]
    //    },
    //    options: {
    //        indexAxis: 'y', // horizontal
    //        responsive: true,
    //        maintainAspectRatio: false,
    //        plugins: {
    //            title: { display: true, text: 'Application Status - Bullet Chart', font: { size: 16 } },
    //            legend: { position: 'top' },
    //            tooltip: {
    //                callbacks: {
    //                    label: function (context) {
    //                        return context.dataset.label + ': ' + context.raw;
    //                    }
    //                }
    //            }
    //        },
    //        scales: {
    //            x: { stacked: true, beginAtZero: true },
    //            y: { stacked: true }
    //        }
    //    }
    //});
    const applicationStatusChart = new Chart(statusChartCtx, {
        type: 'polarArea',  // Polar area chart
        data: {
            labels: ['Pending', 'Approved', 'Rejected'],
            datasets: [{
                data: [0, 0, 0],   // Initial values
                backgroundColor: [
                    'rgba(255,193,7,0.8)', // Pending
                    'rgba(0,123,255,0.8)', // Approved
                    'rgba(220,53,69,0.8)'  // Rejected
                ],
                borderColor: '#ffffff',
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: { display: true, text: 'Applications by Status - Polar Area', font: { size: 16 } },
                legend: { position: 'right' },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return `${context.label}: ${context.raw}`;
                        }
                    }
                }
            },
            scales: {
                r: { beginAtZero: true }  // radial scale
            }
        }
    });

    //chart 11
    const ageGroupsCtx = $('#AgeGroupsChart')[0].getContext('2d');

    const ageGroupsChart = new Chart(ageGroupsCtx, {
        type: 'bar',  // Histogram-style
        data: {
            labels: [],            // Will be updated dynamically
            datasets: [{
                label: 'Applications',
                data: [],
                backgroundColor: [],
                borderColor: '#ffffff',
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Applications by Age Groups',
                    font: { size: 16, weight: 'bold' }
                },
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return `${context.dataset.label}: ${context.raw}`;
                        }
                    }
                }
            },
            scales: {
                x: {
                    title: { display: true, text: 'Age Group', font: { weight:'bold',size:14 } }
                },
                y: {
                    beginAtZero: true,
                    title: { display: true, text: 'Count', font: { weight: 'bold', size: 14 } },
                    ticks: { stepSize: 10 }
                }
            }
        },
        plugins: [{
            id: 'customLabels',
            afterDatasetsDraw: function (chart) {
                const ctx = chart.ctx;
                chart.data.datasets.forEach(function (dataset, i) {
                    const meta = chart.getDatasetMeta(i);
                    if (!meta.hidden) {
                        meta.data.forEach(function (element, index) {
                            // Draw the text
                            ctx.fillStyle = '#000';
                            ctx.font = 'bold 12px Arial';
                            ctx.textAlign = 'center';
                            ctx.textBaseline = 'bottom';

                            const dataString = dataset.data[index].toString();
                            ctx.fillText(dataString, element.x, element.y - 5);
                        });
                    }
                });
            }
        }]
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


            //chart 4
            const rawData = (resp.data.loanStats || [])
                .map(item => ({
                    vehLoanType: item.vehLoanType || 'N/A',
                    loanType: item.loanType || 'N/A',
                    loanCount: Number(item.loanCount || 0)
                }))
                .sort((a, b) => b.loanCount - a.loanCount);

            // Color palette - cycle through colors
            const colors = [
                'rgba(0, 123, 255, 0.8)',    // #007bff
                'rgba(40, 167, 69, 0.8)',    // #28a745
                'rgba(255, 193, 7, 0.8)',    // #ffc107
                'rgba(220, 53, 69, 0.8)',    // #dc3545
                'rgba(23, 162, 184, 0.8)',   // #17a2b8
                'rgba(108, 117, 125, 0.8)'   // #6c757d
            ];

            const backgroundColors = rawData.map((_, index) => colors[index % colors.length]);

            // Update chart
            loanStatisticsChart.data.labels = rawData.map(x => x.loanType);
            loanStatisticsChart.data.datasets[0].data = rawData.map(x => x.loanCount);
            loanStatisticsChart.data.datasets[0].backgroundColor = backgroundColors;
            loanStatisticsChart.data.datasets[0].rawData = rawData; // Store for tooltip
            loanStatisticsChart.update();

            //chart 5
            const UnitrawData = (resp.data.topUnits || [])
                .map(item => ({
                    unitName: item.unitName || 'N/A',
                    totalApplications: Number(item.totalApplications || 0)
                }))
                .sort((a, b) => b.totalApplications - a.totalApplications)
                .slice(0, 10);

            // Color gradient - Top performers get darker/bolder colors
            const Unitcolors = [
                'rgba(0, 123, 255, 0.9)',    // Top 1 - Bold Blue
                'rgba(0, 123, 255, 0.85)',   // Top 2
                'rgba(0, 123, 255, 0.8)',    // Top 3
                'rgba(40, 167, 69, 0.8)',    // 4 - Green
                'rgba(40, 167, 69, 0.75)',   // 5
                'rgba(255, 193, 7, 0.8)',    // 6 - Yellow
                'rgba(255, 193, 7, 0.75)',   // 7
                'rgba(23, 162, 184, 0.8)',   // 8 - Cyan
                'rgba(108, 117, 125, 0.8)',  // 9 - Gray
                'rgba(108, 117, 125, 0.7)'   // 10
            ];

            const UnitbackgroundColors = UnitrawData.map((_, index) => colors[index]);

            // Update chart - Note: for horizontal bars, labels are Y-axis
            topUnitsChart.data.labels = UnitrawData.map(x => x.unitName);
            topUnitsChart.data.datasets[0].data = UnitrawData.map(x => x.totalApplications);
            topUnitsChart.data.datasets[0].backgroundColor = UnitbackgroundColors;
            topUnitsChart.update();


            //chart 6

            const rawLoanAmountData = (resp.data.topUnitsByLoanAmount || [])
                .map(item => ({
                    unitName: item.unitName || 'N/A',
                    totalLoanAmount: Number(item.totalLoanAmount || 0),
                    totalHbaLoan: Number(item.totalHbaLoan || 0),
                    totalCarLoan: Number(item.totalCarLoan || 0),
                    totalPcaLoan: Number(item.totalPcaLoan || 0),
                    totalApplications: Number(item.totalApplications || 0)
                }))
                .sort((a, b) => b.totalLoanAmount - a.totalLoanAmount)
                .slice(0, 10);

            const unitLabelsLoan = rawLoanAmountData.map(x => x.unitName);
            const carLoanData = rawLoanAmountData.map(x => x.totalCarLoan);
            const pcaLoanData = rawLoanAmountData.map(x => x.totalPcaLoan);
            const hbaLoanData = rawLoanAmountData.map(x => x.totalHbaLoan);
            const totalLoanData = rawLoanAmountData.map(x => x.totalLoanAmount);

            // Attach raw data to chart instance for tooltip usage
            topUnitsLoanChart.rawLoanData = rawLoanAmountData;

            // Update chart datasets (grouped)
            topUnitsLoanChart.data.labels = unitLabelsLoan;
            topUnitsLoanChart.data.datasets = [
                { label: 'Car Loan', data: carLoanData, backgroundColor: 'rgba(0,123,255,0.85)' },
                { label: 'PCA/Computer Loan', data: pcaLoanData, backgroundColor: 'rgba(40,167,69,0.85)' },
                { label: 'HBA Loan', data: hbaLoanData, backgroundColor: 'rgba(255,193,7,0.85)' },
                { label: 'Total Loan', data: totalLoanData, backgroundColor: 'rgba(220,53,69,0.85)' }
            ];

            // ensure axis labels and stacked flags retain defaults (grouped)
            topUnitsLoanChart.options.scales.x.title = { display: true, text: 'Unit Name', font: { weight:'bold',size:14 } };
            topUnitsLoanChart.options.scales.y.title = { display: true, text: 'Loan Amount (₹)', font: { weight: 'bold', size: 14 } };
            topUnitsLoanChart.options.scales.x.stacked = false;
            topUnitsLoanChart.options.scales.y.stacked = false;

            topUnitsLoanChart.update();


            //chart 7
            const dealers = (resp.data.topDealers || [])
                .map(d => ({ name: d.dealerName || 'N/A', count: Number(d.totalApplications || 0) }))
                .sort((a, b) => b.count - a.count)
                .slice(0, 10);

            topDealersChart.data.labels = dealers.map(d => d.name);
            topDealersChart.data.datasets[0].data = dealers.map(d => d.count);
            topDealersChart.update();

            //chart 8
            const Loandealers = (resp.data.topLoanDealers || [])
                .map(d => ({
                    name: d.dealerName || 'N/A',
                    loanAmount: Number(d.totalLoanAmount || 0)
                }))
                .sort((a, b) => b.loanAmount - a.loanAmount)
                .slice(0, 10);

            const dealerColors = [
                'rgba(255, 99, 132, 0.8)',   // Top 1 - Light Red
                'rgba(54, 162, 235, 0.8)',   // Top 2 - Blue
                'rgba(255, 159, 64, 0.8)',   // Top 3 - Orange
                'rgba(75, 192, 192, 0.8)',   // Top 4 - Teal
                'rgba(153, 102, 255, 0.8)',  // Top 5 - Purple
                'rgba(255, 205, 86, 0.8)',   // Top 6 - Light Yellow
                'rgba(255, 99, 132, 0.6)',   // Top 7 - Soft Red
                'rgba(255, 159, 64, 0.6)',   // Top 8 - Soft Orange
                'rgba(75, 192, 192, 0.6)',   // Top 9 - Soft Teal
                'rgba(153, 102, 255, 0.6)'   // Top 10 - Soft Purple
            ];

            topLoanDealersChart.data.labels = Loandealers.map(d => d.name);
            topLoanDealersChart.data.datasets[0].data = Loandealers.map(d => d.loanAmount);
            topLoanDealersChart.data.datasets[0].backgroundColor = Loandealers.map((_, index) => dealerColors[index]);;

            topLoanDealersChart.update();

            //chart 9
            const topPersonnel = (resp.data.topPersonnel || [])
                .map(item => ({
                    displayName: item.rank + ' ' + item.applicantName,
                    applicantName: item.applicantName,
                    rankName: item.rankName,
                    totalLoanAmount: Number(item.totalLoanAmount || 0),
                    totalCarLoan: Number(item.totalCarLoan || 0),
                    totalHbaLoan: Number(item.totalHbaLoan || 0),
                    totalPcaLoan: Number(item.totalPcaLoan || 0),
                    totalLoans: Number(item.totalLoans || 0),
                    carLoans: Number(item.carLoans || 0),
                    hbaLoans: Number(item.hbaLoans || 0),
                    pcaLoans: Number(item.pcaLoans || 0)
                }))
                .sort((a, b) => b.totalLoanAmount - a.totalLoanAmount)
                .slice(0, 20);

            const topPersonnelcolors = topPersonnel.map((_, index) => {
                if (index < 3) return 'rgba(220, 53, 69, 0.9)';   // Top 3 - Red
                if (index < 7) return 'rgba(255, 193, 7, 0.8)';   // Next 4 - Yellow
                if (index < 15) return 'rgba(0, 123, 255, 0.8)';  // Next 8 - Blue
                return 'rgba(108, 117, 125, 0.7)';               // Remaining - Gray
            });

            topPersonnelChart.data.labels = topPersonnel.map(x => x.displayName);
            topPersonnelChart.data.datasets[0].data = topPersonnel.map(x => x.totalLoanAmount);
            topPersonnelChart.data.datasets[0].backgroundColor = topPersonnelcolors;
            topPersonnelChart.data.datasets[0].rawData = topPersonnel;
            topPersonnelChart.update();


            //chart 10
            // --- Doughnut Chart: Applications by Status ---


            //const statusData = [
            //    resp.data.statusCounts[0].pendingCount,
            //    resp.data.statusCounts[0].approvedCount,
            //    resp.data.statusCounts[0].rejectedCount
            //];

            //const statusColors = [
            //    'rgba(255, 193, 7, 0.8)',   // Pending - Yellow
            //    'rgba(0, 123, 255, 0.8)',   // Approved - Blue
            //    'rgba(220, 53, 69, 0.8)'    // Rejected - Red
            //];

            //applicationStatusChart.data.labels = ['Pending', 'Approved', 'Rejected'];
            //applicationStatusChart.data.datasets[0].data = statusData;
            //applicationStatusChart.data.datasets[0].backgroundColor = statusColors;
            //applicationStatusChart.update();
            //const statusData = [
            //    resp.data.statusCounts[0].pendingCount,
            //    resp.data.statusCounts[0].approvedCount,
            //    resp.data.statusCounts[0].rejectedCount
            //];

            //const totalApplications = statusData.reduce((a, b) => a + b, 0);

            //// Update datasets dynamically
            //applicationStatusChart.data.datasets[0].data = [statusData[0]]; // Pending
            //applicationStatusChart.data.datasets[1].data = [statusData[1]]; // Approved
            //applicationStatusChart.data.datasets[2].data = [statusData[2]]; // Rejected
            //applicationStatusChart.data.datasets[3].data = [totalApplications]; // Target/Background
            //applicationStatusChart.update();

            if (resp.data.statusCounts != 0) {
                const statusData = [
                    resp.data.statusCounts[0].pendingCount,
                    resp.data.statusCounts[0].approvedCount,
                    resp.data.statusCounts[0].rejectedCount
                ];

                // Update dataset dynamically
                applicationStatusChart.data.datasets[0].data = statusData;
            }
            else {
                applicationStatusChart.data.datasets[0].data = 0;
            }
            applicationStatusChart.update();


            //chart 11
            const ageGroupsData = resp.data.ageGroups;

            // Extract labels and counts dynamically
            const ageGroupsLabels = ageGroupsData.map(x => x.ageGroup);
            const ageGroupsCounts = ageGroupsData.map(x => x.totalApplications);

            // Optional: assign colors dynamically
            const ageGroupsColors = [
                'rgba(75,192,192,0.8)',
                'rgba(54,162,235,0.8)',
                'rgba(255,206,86,0.8)',
                'rgba(255,99,132,0.8)',
                'rgba(153,102,255,0.8)'
            ];
            const agecolors = ageGroupsLabels.map((_, i) => ageGroupsColors[i % ageGroupsColors.length]);

            ageGroupsChart.data.labels = ageGroupsData.map(x => x.ageGroup);
            ageGroupsChart.data.datasets[0].data = ageGroupsCounts;
            ageGroupsChart.data.labels = ageGroupsLabels;
            ageGroupsChart.data.datasets[0].backgroundColor = agecolors;

            ageGroupsChart.update();
        });
    }

    // initial + year change
    loadChartData(currentYear);
    $yearSelect.on('change', function () { loadChartData($(this).val()); });


});