$(function () {
    // ---- DOM ----
    const $yearSelect = $('#yearSelect');
    const $btnLoan = $('#btnLoan');
    const $btnMaturity = $('#btnMaturity');
    const $loanCanvas = $('#applicationByMonth');
    const $matCanvas = $('#maturityChartCanvas');
    const currentYear = new Date().getFullYear();

    // ---- state ----
    let activeMode = 'loan';   // 'loan' | 'maturity'
    let loanChart = null;
    let maturityChart = null;

    // ---- utils ----
    const monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    function showLoan() { $loanCanvas.removeClass('d-none'); $matCanvas.addClass('d-none'); }
    function showMaturity() { $matCanvas.removeClass('d-none'); $loanCanvas.addClass('d-none'); }

    // years
    for (let y = currentYear; y >= 2000; y--) $yearSelect.append(new Option(y, y));
    $yearSelect.val(currentYear);

    // ===================== LOAN CHART =====================
    function initLoanChart() {
        if (loanChart) return;
        const ctx = $loanCanvas[0].getContext('2d');
        loanChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: [],
                datasets: [
                    { label: 'Car Applications', data: [], backgroundColor: '#007bff', borderColor: '#ffffff', borderWidth: 2 },
                    { label: 'PC Applications', data: [], backgroundColor: '#28a745', borderColor: '#ffffff', borderWidth: 2 },
                    { label: 'HBA Applications', data: [], backgroundColor: '#ffc107', borderColor: '#ffffff', borderWidth: 2 },
                    { label: 'Total Applications', data: [], backgroundColor: '#dc3545', borderColor: '#ffffff', borderWidth: 2 }
                ]
            },
            options: {
                responsive: true, maintainAspectRatio: false,
                plugins: { title: { display: true, text: 'Applications by Month', font: { size: 16 } }, legend: { position: 'top' } },
                scales: {
                    y: { beginAtZero: true, title: { display: true, text: 'Count', font: { weight: 'bold', size: 14 } } },
                    x: { title: { display: true, text: 'Month', font: { weight: 'bold', size: 14 } } }
                }
            },
            plugins: [valueLabelsPlugin()]
        });

        const topCtx = $('#topRanksChart')[0].getContext('2d');
         topRanksChart = new Chart(topCtx, {
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
         topRegtChart = new Chart(topregtCtx, {
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

         loanStatisticsChart = new Chart(loanCtx, {
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
         topUnitsChart = new Chart(topUnitsCtx, {
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
                            stepSize: 2,
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

         topUnitsLoanChart = new Chart(toploanUnitsCtx, {
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
        
         topDealersChart = new Chart(topDealersCtx, {
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

        const topLoanDealers = $('#topLoanDealersChart')[0].getContext('2d');

   
         topLoanDealersChart = new Chart(topLoanDealers, {
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

         topPersonnelChart = new Chart(topPersonnelCtx, {
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
    
         applicationStatusChart = new Chart(statusChartCtx, {
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

         ageGroupsChart = new Chart(ageGroupsCtx, {
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
                        title: { display: true, text: 'Age Group', font: { weight: 'bold', size: 14 } }
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


        //chart 12
        const MultipleLoansChartCtx = $('#TopApplicantsMultipleLoansChart')[0].getContext('2d');

         topApplicantsChart = new Chart(MultipleLoansChartCtx, {
            type: 'line',
            data: { datasets: [] },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    title: {
                        display: true,
                        text: 'Top 20 Applicants with Multiple Loans Over Time',
                        font: { size: 16 }
                    },
                    legend: {
                        position: 'right',
                        labels: {
                            boxWidth: 12,
                            font: { size: 12 }
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                const point = context.raw;
                                return `${context.dataset.label}: Loan #${point.y} on ${point.dateString}`;
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        type: 'linear',
                        title: {
                            display: true,
                            text: 'Year',
                            font: { weight: 'bold', size: 14 }
                        },
                        ticks: {
                            stepSize: 1,
                            callback: function (value) {
                                return Math.floor(value);
                            }
                        }
                    },
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Cumulative Loan Count',
                            font: { weight: 'bold', size: 14 }
                        },
                        ticks: { stepSize: 1 }
                    }
                },
                animation: {
                    duration: 1500,
                    easing: 'easeInOutQuart'
                }
            }
        });

        //chart 13
        const comparisonChartCtx = $('#LoancomparisonChart')[0].getContext('2d');

         comparisonChart = new Chart(comparisonChartCtx, {
            type: 'bar',
            data: { datasets: [] },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    title: {
                        display: true,
                        text: 'Comparison of Loan Types by Unit',
                        font: { size: 16 }
                    },
                    legend: {
                        position: 'top',
                        labels: {
                            boxWidth: 12,
                            font: { size: 12 }
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                return `${context.dataset.label}: ${context.parsed.y}`;
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        stacked: true,
                        title: {
                            display: true,
                            text: 'Unit Name',
                            font: { weight: 'bold', size: 14 }
                        },
                        //ticks: {
                        //    maxRotation: 45,
                        //    minRotation: 45,
                        //    font: { size: 11 }
                        //}
                    },
                    y: {
                        stacked: true,
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Loan Count',
                            font: { weight: 'bold', size: 14 }
                        },
                        ticks: {
                            stepSize: 0
                        }
                    }
                },
                animation: { duration: 1500, easing: 'easeInOutQuart' }
            },
            plugins: [{
                id: 'stackedBarLabels',
                afterDatasetsDraw: function (chart) {
                    const ctx = chart.ctx;

                    chart.data.datasets.forEach((dataset, datasetIndex) => {
                        const meta = chart.getDatasetMeta(datasetIndex);

                        if (!meta.hidden) {
                            meta.data.forEach((bar, index) => {
                                const value = dataset.data[index];

                                // Only show label if value > 0
                                if (value > 0) {
                                    // Calculate position
                                    const x = bar.x;
                                    const y = bar.y + (bar.height / 2); // Center of the bar segment

                                    // Set text style
                                    ctx.fillStyle = '#000'; // White text
                                    ctx.font = 'bold 11px Arial';
                                    ctx.textAlign = 'center';
                                    ctx.textBaseline = 'middle';

                                    // Draw text
                                    ctx.fillText(value.toString(), x, y);
                                }
                            });
                        }
                    });
                }
            }]
        });

    }

    function loadLoanData(year) {
        $.getJSON(`/Home/GetApplicationAnalytics?year=${year}`, function (resp) {
            if (!resp || resp.success === false) return alert(resp?.message || 'Failed to load loan data');

            // Expected: data.monthlyApplications[]: { month, caCount, pcaCount, hbaCount, totalApplications }
            const rows = (resp.data?.monthlyApplications || [])
                .map(m => ({
                    month: Number(m.month),
                    ca: Number(m.caCount || 0),
                    pca: Number(m.pcaCount || 0),
                    hba: Number(m.hbaCount || 0),
                    total: Number(m.totalApplications || 0)
                }))
                .sort((a, b) => a.month - b.month);

            loanChart.data.labels = rows.map(r => monthNames[(r.month || 1) - 1] || '');
            loanChart.data.datasets[0].data = rows.map(r => r.ca);
            loanChart.data.datasets[1].data = rows.map(r => r.pca);
            loanChart.data.datasets[2].data = rows.map(r => r.hba);
            loanChart.data.datasets[3].data = rows.map(r => r.total || (r.ca + r.pca + r.hba));
            loanChart.update();

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
            topUnitsLoanChart.options.scales.x.title = { display: true, text: 'Unit Name', font: { weight: 'bold', size: 14 } };
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


            //chart 12
            const multipleLoans = resp.data.multipleLoans;
            const datasets = multipleLoans.map((app, idx) => {
                console.log(`Processing applicant ${idx}:`, app);
                console.log('Loan Dates:', app.loanDates);

                // Filter and parse valid dates only
                const points = (app.loanDates || [])
                    .filter(dateStr => dateStr != null && dateStr !== '')  // Remove null/empty
                    .map((dateStr, i) => {
                        // Try to parse the date
                        let date;

                        // Handle different date formats
                        if (typeof dateStr === 'string') {
                            // ISO format: "2023-05-15T10:30:00"
                            date = new Date(dateStr);
                        } else if (typeof dateStr === 'object' && dateStr !== null) {
                            // Already a date object
                            date = dateStr;
                        } else {
                            console.warn('Invalid date format:', dateStr);
                            return null;
                        }

                        // Validate the date
                        if (isNaN(date.getTime())) {
                            console.warn('Invalid date:', dateStr);
                            return null;
                        }

                        const year = date.getFullYear() + (date.getMonth() / 12);
                        const dateString = date.toLocaleDateString('en-IN', {
                            day: '2-digit',
                            month: 'short',
                            year: 'numeric'
                        });

                        console.log(`Date ${i}: ${dateStr} -> ${dateString} (year: ${year.toFixed(2)})`);

                        return {
                            x: year,
                            y: i + 1,
                            dateString: dateString
                        };
                    })
                    .filter(point => point !== null);  // Remove invalid dates

                console.log(`Valid points for ${app.applicantName}:`, points);

                if (points.length === 0) {
                    console.warn(`No valid dates for applicant: ${app.applicantName}`);
                }

                return {
                    label: `${app.rank || ''} ${app.applicantName}`,
                    data: points,
                    fill: false,
                    borderColor: `hsl(${idx * 18}, 70%, 50%)`,
                    backgroundColor: `hsl(${idx * 18}, 70%, 50%)`,
                    borderWidth: 2,
                    tension: 0.3,
                    pointRadius: 4,
                    pointHoverRadius: 6
                };
            }).filter(dataset => dataset.data.length > 0);  // Only include datasets with data

            console.log('Final Datasets:', datasets);


            topApplicantsChart.data.datasets = datasets;
            topApplicantsChart.update();


            //chart 13
            const loanDataByUnit = resp.data.loanTypes;  // Assuming `resp.data` contains the loan data

            // Prepare the datasets
            const Compdatasets = [
                {
                    label: 'Car Loans',
                    data: loanDataByUnit.map(unit => unit.caCount),
                    backgroundColor: 'rgba(0, 123, 255, 0.8)',
                    borderColor: '#ffffff',
                    borderWidth: 2,
                    stack: 'LoanTypes'
                },
                {
                    label: 'PCA Loans',
                    data: loanDataByUnit.map(unit => unit.pcaCount),
                    backgroundColor: 'rgba(40, 167, 69, 0.8)',
                    borderColor: '#ffffff',
                    borderWidth: 2,
                    stack: 'LoanTypes'
                },
                {
                    label: 'HBA Loans',
                    data: loanDataByUnit.map(unit => unit.hbaCount),
                    backgroundColor: 'rgba(255, 193, 7, 0.8)',
                    borderColor: '#ffffff',
                    borderWidth: 2,
                    stack: 'LoanTypes'
                }
            ];

            // Prepare labels (unit names)
            const labels = loanDataByUnit.map(unit => unit.unitName);

            // Update chart data
            comparisonChart.data.labels = labels;


            comparisonChart.data.datasets = Compdatasets;

            // Update the chart
            comparisonChart.update();

        }).fail(() => alert('Failed to load loan data'));
    }

    // ===================== MATURITY CHART =====================
    function initMaturityChart() {
        if (maturityChart) return;
        const ctx = $matCanvas[0].getContext('2d');
        maturityChart = new Chart(ctx, {
            type: 'bar', // same type, but its own chart instance
            data: {
                labels: [],
                datasets: [
                    { label: 'Education Details', data: [], backgroundColor: '#6f42c1', borderColor: '#ffffff', borderWidth: 2 },
                    { label: 'Marriage ward', data: [], backgroundColor: '#20c997', borderColor: '#ffffff', borderWidth: 2 },
                    { label: 'Property Renovation', data: [], backgroundColor: '#fd7e14', borderColor: '#ffffff', borderWidth: 2 },
                    { label: 'Total Maturity', data: [], backgroundColor: '#17a2b8', borderColor: '#ffffff', borderWidth: 2 }
                ]
            },
            options: {
                responsive: true, maintainAspectRatio: false,
                plugins: { title: { display: true, text: 'Applications by Month', font: { size: 16 } }, legend: { position: 'top' } },
                scales: {
                    y: { beginAtZero: true, title: { display: true, text: 'Count', font: { weight: 'bold', size: 14 } } },
                    x: { title: { display: true, text: 'Month', font: { weight: 'bold', size: 14 } } }
                }
            },
            plugins: [valueLabelsPlugin()]
        });

        const topCtx = $('#topRanksChart')[0].getContext('2d');
        window.topRanksChart.destroy();
         topRanksChart = new Chart(topCtx, {
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
        window.topRegtChart.destroy();
         topRegtChart = new Chart(topregtCtx, {
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


        // ---- Chart 5=: Top 10 Units by Number of Applications ----
        const topUnitsCtx = $('#topUnitsChart')[0].getContext('2d');

        // ---- Chart: Top 10 Units (Horizontal Bar) ----
        window.topUnitsChart.destroy();
         topUnitsChart = new Chart(topUnitsCtx, {
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
                            stepSize: 3,
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
    }

    function loadMaturityData(year) {
        $.getJSON(`/Home/GetClaimApplicationAnalytics?year=${year}`, function (resp) {
            if (!resp || resp.success === false) return alert(resp?.message || 'Failed to load maturity data');

            // Flexible mapping: accept several possible field names
            // Prefer: monthlyMaturity[] but fallback to monthlyApplications[] if that’s your shape
            const raw = resp.data?.monthlyMaturity || resp.data?.monthlyApplications || [];
            const rows = raw
                .map(m => ({
                    month: Number(m.month),
                    car: Number(m.carMaturity ?? m.caMaturity ?? m.caCount ?? 0),
                    pc: Number(m.pcMaturity ?? m.pcaMaturity ?? m.pcaCount ?? 0),
                    hba: Number(m.hbaMaturity ?? m.hbaCount ?? 0),
                    total: Number(m.totalMaturity ?? m.totalApplications ?? 0)
                }))
                .sort((a, b) => a.month - b.month);

            maturityChart.data.labels = rows.map(r => monthNames[(r.month || 1) - 1] || '');
            maturityChart.data.datasets[0].data = rows.map(r => r.car);
            maturityChart.data.datasets[1].data = rows.map(r => r.pc);
            maturityChart.data.datasets[2].data = rows.map(r => r.hba);
            maturityChart.data.datasets[3].data = rows.map(r => r.total || (r.car + r.pc + r.hba));

            // Auto-hide empty series for maturity if endpoint doesn’t provide per-scheme
            maturityChart.getDatasetMeta(0).hidden = maturityChart.data.datasets[0].data.every(v => !v);
            maturityChart.getDatasetMeta(1).hidden = maturityChart.data.datasets[1].data.every(v => !v);
            maturityChart.getDatasetMeta(2).hidden = maturityChart.data.datasets[2].data.every(v => !v);

            maturityChart.update();

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

            //chart 5
            const UnitrawData = (resp.data.topUnits || [])
                .map(item => ({
                    unitName: item.unitName || 'N/A',
                    totalApplications: Number(item.totalApplications || 0)
                }))
                .sort((a, b) => b.totalApplications - a.totalApplications)
                .slice(0, 10);

            // Color gradient - Top performers get darker/bolder colors
            // Color palette - cycle through colors
            const colors = [
                'rgba(0, 123, 255, 0.8)',    // #007bff
                'rgba(40, 167, 69, 0.8)',    // #28a745
                'rgba(255, 193, 7, 0.8)',    // #ffc107
                'rgba(220, 53, 69, 0.8)',    // #dc3545
                'rgba(23, 162, 184, 0.8)',   // #17a2b8
                'rgba(108, 117, 125, 0.8)'   // #6c757d
            ];

            const UnitbackgroundColors = UnitrawData.map((_, index) => colors[index]);

            // Update chart - Note: for horizontal bars, labels are Y-axis
            topUnitsChart.data.labels = UnitrawData.map(x => x.unitName);
            topUnitsChart.data.datasets[0].data = UnitrawData.map(x => x.totalApplications);
            topUnitsChart.data.datasets[0].backgroundColor = UnitbackgroundColors;
            topUnitsChart.update();

        }).fail(() => alert('Failed to load maturity data'));
    }

    // ===================== COMMON: value labels plugin =====================
    function valueLabelsPlugin() {
        return {
            id: 'valueLabels',
            afterDatasetsDraw(chart) {
                const ctx = chart.ctx;
                chart.data.datasets.forEach((ds, di) => {
                    const meta = chart.getDatasetMeta(di);
                    if (meta.hidden) return;
                    meta.data.forEach((elem, i) => {
                        const v = ds.data[i] ?? 0;
                        if (!isFinite(v) || v === 0) return;
                        ctx.fillStyle = '#000';
                        ctx.font = 'bold 12px Arial';
                        ctx.textAlign = 'center';
                        ctx.textBaseline = 'bottom';
                        ctx.fillText(String(v), elem.x, elem.y - 5);
                    });
                });
            }
        };
    }

    // ===================== EVENTS =====================
    function refreshActive(year) {
        if (activeMode === 'loan') {
            initLoanChart();     // create if missing
            showLoan();
            loadLoanData(year);
        } else {
            initMaturityChart(); // create if missing
            showMaturity();
            loadMaturityData(year);
        }
    }

    // initial load
    refreshActive(currentYear);

    // year change
    $yearSelect.on('change', function () {
        refreshActive($(this).val());
    });

    // button toggles
    $btnLoan.on('click', function () {
        if (activeMode === 'loan') return;
        activeMode = 'loan';
        $btnLoan.addClass('btn-primary active').removeClass('btn-outline-primary');
        $btnMaturity.addClass('btn-outline-primary').removeClass('btn-primary active');
        refreshActive($yearSelect.val());
    });

    $btnMaturity.on('click', function () {
        if (activeMode === 'maturity') return;
        activeMode = 'maturity';
        $btnMaturity.addClass('btn-primary active').removeClass('btn-outline-primary');
        $btnLoan.addClass('btn-outline-primary').removeClass('btn-primary active');
        refreshActive($yearSelect.val());
    });
});
