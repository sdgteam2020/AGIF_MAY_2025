$(document).ready(function () {
    $('#typeSelect').on('change', function () {
        let selectedType = $(this).val();
        clearAllData();
    });

    function clearAllData() {
        // Hide result table and no results message
        $('#resultsTable').addClass('d-none');
        $('#noResultsMessage').addClass('d-none');

        // Clear the table body
        $('#applicationTableBody').empty();

        // Clear the input field (optional)
        $('#armyNoInput').val('');
        
    }


    $('#searchByArmyNo').on('submit', function (e) {
        e.preventDefault();
        const armyNo = $('#armyNoInput').val().trim();
        if (armyNo === '') return;

        // Clear previous results
        $('#noResultsMessage').addClass('d-none');
        $('#resultsTable').addClass('d-none');

        let selectedType = $('#typeSelect').val();

        // Determine the endpoint based on selected type
        const searchEndpoint = getSearchEndpoint(selectedType);

        $.ajax({
            url: searchEndpoint,
            type: 'GET',
            data: { armyNo: armyNo },
            success: function (data) {
                if (data && data.length > 0) {
                    populateTable(data);
                    $('#resultsTable').removeClass('d-none');
                } else {
                    $('#noResultsMessage').removeClass('d-none');
                }
            },
            error: function (xhr, status, error) {
                alert('Error searching applications. Please try again.');
            }
        });
    });

    // Function to determine the search endpoint based on selected type
    function getSearchEndpoint(type) {
        switch (type) {
            case 'Loan':
                return '/Default/SearchByArmyNo';
            case 'Maturity':
                return '/Default/ClaimSearchByArmyNo';
            default:
                return '/Default/SearchByArmyNo'; // Default to loan endpoint
        }
    }


    function populateTable(applications) {
        const tbody = $('#applicationTableBody');
        tbody.empty(); // Clear old results

        $.each(applications, function (index, app) {
            const safeAppId = (app.applicationId !== undefined && app.applicationId !== null) ? app.applicationId : index;

            // Conditional extra button for statusId = 103
            let extraButtonHtml = '';
            let downloadButtonHtml = '';
            if (app.statusId !== 1 && app.statusId !== 101) {
                downloadButtonHtml = `
                <button class="btn btn-danger ms-2 btn-icon downloadApplication"
        type="button"
        title="Download"
        data-app-id="${safeAppId}">
    <i class="bi bi-download"></i>
</button>
                `
            }
            if (app.statusId === 41 || app.statusId === 161) {
                extraButtonHtml = `
               <button class="btn btn-warning ms-2 btn-icon editapp"
        type="button"
        title="Edit Application"
        data-app-id="${safeAppId}">
    <i class="bi bi-pencil-square"></i>
</button>
            `;
            }

            const rowHtml = `
                <tr>
                <td>${index + 1}.</td>
                    <td class="statusList">${app.applicationType || 'N/A'}</td>
                    <td class="statusList">
                        <span class=" ${getStatusBadgeClass(app.statusId)} statusList">${app.status || 'Unknown'}</span>
                    </td>
                    <td class="d-flex align-items-center">
                        <button class="btn btn-primary timeline-btn " type="button"
                                data-app-id="${safeAppId}" 
                                data-bs-toggle="collapse" 
                                data-bs-target="#timeline-${safeAppId}" 
                                aria-expanded="false" 
                                aria-controls="timeline-${safeAppId}"
                                title="Application Timeline">
                            <i class="bi bi-calendar-week"></i>
                        </button>
                         ${extraButtonHtml}
                         ${downloadButtonHtml}
                         
                    </td>
                    <td class="statusList">${app.remarks || 'N/A'}</td>

                </tr>
                <tr class="collapse" id="timeline-${safeAppId}">
                    <td colspan="3">
                        <div class="timeline-loading" id="loading-${safeAppId}">
                            <i class="fas fa-spinner fa-spin"></i> Loading timeline...
                        </div>
                        <div class="timeline-container" id="timeline-content-${safeAppId}">
                            <!-- Timeline will be populated here -->
                        </div>
                    </td>
                </tr>
            `;
            tbody.append(rowHtml);
        });
    }


    $(document).on('click', '.editapp', function () {
        // Adjust the URL according to your routing
        const type = $('#typeSelect').val();

        const appId = $(this).data('app-id');  // Get application ID from button
        if (!appId) return;

        if (type === 'Loan')
            window.location.href = `/OnlineApplication/OnlineApplication/${appId}`;
        else if (type === 'Maturity')
            window.location.href = `/Claim/OnlineApplication/${appId}`;


        //window.location.href = `/OnlineApplication/OnlineApplication/${appId}`;
    });

    $(document).on('click', '.downloadApplication', function () {

        const appId = $(this).data('app-id');
        const type = $('#typeSelect').val();
        if (!appId) return;

        downloadApplication(appId,type);
    });

    function downloadApplication(applicationId,type) {
        if (!applicationId) {
            alert('Application ID is required for download');
            return;
        }

        if (type === 'Loan')
            window.location.href = `/Default/DownloadApplication?id=${applicationId}`;
        else if (type === 'Maturity')
            window.location.href = `/Default/DownloadClaimApplication?id=${applicationId}`;
        // Direct file download — no AJAX needed
        
    }

    $(document).on('click', '.timeline-btn', function () {
        const appId = $(this).data('app-id');
        const timelineRow = $('#timeline-' + appId);
        const timelineContent = $('#timeline-content-' + appId);
        const loadingDiv = $('#loading-' + appId);
        let selectedType = $('#typeSelect').val();
        let endpoint = '';
        // Check if timeline is already loaded
        if (timelineContent.children().length > 0) {
            return; // Timeline already loaded, just toggle
        }
        

        // Show loading
        loadingDiv.show();
        timelineContent.hide();

        if (selectedType === 'Loan')
            endpoint = '/Default/GetTimeline';
        else if (selectedType === 'Maturity')
            endpoint = '/Default/GetClaimTimeline';

        // Get the appropriate timeline endpoint
        $.ajax({
            url: endpoint,
            type: 'GET',
            data: { applicationId: appId },
            success: function (response) {
                // Hide loading
                loadingDiv.hide();

                if (response && response.length > 0) {
                    // Build timeline HTML
                    const timelineHtml = buildTimelineHtml(response);
                    timelineContent.html(timelineHtml);
                    timelineContent.show();
                } else {
                    timelineContent.html('<div class="alert alert-info">No timeline data available.</div>');
                    timelineContent.show();
                }
            },
            error: function (xhr, status, error) {
                console.error('Error fetching timeline:', error);
                loadingDiv.hide();
                timelineContent.html('<div class="alert alert-danger">Failed to load timeline. Please try again.</div>');
                timelineContent.show();
            }
        });
    });

    function buildTimelineHtml(timelineData) {
        let timelineHtml = '<div class="timeline-vertical">';

        $.each(timelineData, function (index, step) {
            const isLast = index === timelineData.length - 1;
            const stepClass = isLast ? 'timeline-step last' : 'timeline-step';
            let shadow = "";
            if (step.statusId == 1 || step.statusId == 2) {
                shadow = 'green';
            }
            else if (step.statusId == 3) {
                shadow = 'red';
            }
            else if (step.statusId == 5) {
                shadow = 'yellow';
            }
            timelineHtml += `
                <div class="${stepClass}">
                    <div class="timeline-dot ${getStatusBadgeClass(step.statusId)}"></div>
                    ${!isLast ? '<div class="timeline-line"></div>' : ''}
                    <div class="timeline-content ${getStatusBadgeClass(step.statusId)}">
                        <div class="timeline-title text-white">${step.status || step.title || 'Status Update'}</div>
                        <div class="timeline-date text-white">${formatDate(step.timeLine)}</div>
                    </div>
                </div>
            `;
        });

        timelineHtml += '</div>';
        return timelineHtml;
    }

    function formatDate(dateString) {
        if (!dateString) return 'N/A';

        try {
            // Expecting dd-mm-yyyy
            const parts = dateString.split('-');
            if (parts.length === 3) {
                // Rearranged to yyyy-mm-dd (ISO format)
                const isoString = `${parts[2]}-${parts[1]}-${parts[0]}`;
                const date = new Date(isoString);
                return date.toLocaleDateString('en-GB', {
                    year: 'numeric',
                    month: 'short',
                    day: 'numeric'
                });
            }
            return dateString;
        } catch (e) {
            return dateString; // Return original if parsing fails
        }
    }

    function getStatusBadgeClass(statusId) {
        if (!statusId) {
            return 'bg-secondary';
        }
        if (statusId == 1 || statusId == 2) {
            return 'bg-success';
        }
        else if (statusId == 3) {
            return 'bg-danger';
        }
        else if (statusId == 5) {
            return 'bg-warning';
        }
        else {
            return 'bg-primary';
        }
    }
});