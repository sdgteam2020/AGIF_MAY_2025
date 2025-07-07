$(document).ready(function () {
    $('#searchByArmyNo').on('submit', function (e) {
        e.preventDefault();
        var armyNo = $('#armyNoInput').val().trim();
        if (armyNo === '') return;

        // Clear previous results
        $('#noResultsMessage').addClass('d-none');
        $('#resultsTable').addClass('d-none');

        $.ajax({
            url: '/Default/SearchByArmyNo',
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
                console.error('Error fetching data:', error);
                alert('Error searching applications. Please try again.');
            }
        });
    });

    function populateTable(applications) {
        var tbody = $('#applicationTableBody');
        tbody.empty(); // Clear old results

        $.each(applications, function (index, app) {
            var safeAppId = (app.applicationId !== undefined && app.applicationId !== null) ? app.applicationId : index;
            var rowHtml = `
                <tr>
                    <td>${app.applicationType || 'N/A'}</td>
                    <td>
                        <span class="badge ${getStatusBadgeClass(app.statusId)}">${app.status || 'Unknown'}</span>
                    </td>
                    <td>
                        <button class="btn btn-info timeline-btn" type="button"
                                data-app-id="${safeAppId}" 
                                data-bs-toggle="collapse" 
                                data-bs-target="#timeline-${safeAppId}" 
                                aria-expanded="false" 
                                aria-controls="timeline-${safeAppId}">
                            <i class="fas fa-calendar-check"></i> Show Timeline
                        </button>
                    </td>
                </tr>
                <tr class="collapse" id="timeline-${safeAppId}">
                    <td colspan="3">
                        <div class="timeline-loading" id="loading-${safeAppId}" style="text-align: center; padding: 20px;">
                            <i class="fas fa-spinner fa-spin"></i> Loading timeline...
                        </div>
                        <div class="timeline-container" id="timeline-content-${safeAppId}" style="display: none;">
                            <!-- Timeline will be populated here -->
                        </div>
                    </td>
                </tr>
            `;
            tbody.append(rowHtml);
        });
    }

    $(document).on('click', '.timeline-btn', function () {
        var appId = $(this).data('app-id');
        var timelineRow = $('#timeline-' + appId);
        var timelineContent = $('#timeline-content-' + appId);
        var loadingDiv = $('#loading-' + appId);

        // Check if timeline is already loaded
        if (timelineContent.children().length > 0) {
            return; // Timeline already loaded, just toggle
        }

        console.log("Timeline button clicked for App ID:", appId);

        // Show loading
        loadingDiv.show();
        timelineContent.hide();

        $.ajax({
            url: '/Default/GetTimeline',
            type: 'GET',
            data: { applicationId: appId },
            success: function (response) {
                console.log("Timeline response:", response);

                // Hide loading
                loadingDiv.hide();

                if (response && response.length > 0) {
                    // Build timeline HTML
                    var timelineHtml = buildTimelineHtml(response);
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
        var timelineHtml = '<div class="timeline-vertical">';

        $.each(timelineData, function (index, step) {
            var isLast = index === timelineData.length - 1;
            var stepClass = isLast ? 'timeline-step last' : 'timeline-step';
            let shadow = "";
            if (step.statusId == 1 || step.statusId == 2) {
                shadow= 'green';
            }
            else if (step.statusId == 3) {
                shadow= 'red';
            }
            else if (step.statusId == 5) {
                shadow= 'yellow';
            }
            timelineHtml += `
                <div class="${stepClass}">
                    <div class="timeline-dot ${getStatusBadgeClass(step.statusId)}" style="box-shadow: 0 0 0 2px ${shadow};"></div>
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
            var date = new Date(dateString);
            return date.toLocaleDateString('en-US', {
                year: 'numeric',
                month: 'short',
                day: 'numeric'
            });
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
    }
});