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
        $('#aadharNoInput').val('');
        
    }
    $('#aadharNoInput').on('input', function () {
        formatAadhar(this);
    })
    function formatAadhar(input) {
        let value = input.value.replace(/\D/g, '');
        value = value.substring(0, 12);
        let formattedValue = '';
        for (let i = 0; i < value.length; i++) {
            if (i > 0 && i % 4 === 0) {
                formattedValue += '-';
            }
            formattedValue += value[i];
        }
        input.value = formattedValue;
    }

    $('#searchByArmyNo').on('submit', async function (e) {
        e.preventDefault();

        const armyNo = $('#armyNoInput').val().trim();
        
        const aadharNo = $('#aadharNoInput').val().trim();
        if (armyNo === '') {
            alert('Please enter an Army Number');
            return;
        }
        if (aadharNo === '') {
            alert('Please enter an Aadhar Number');
            return;
        }

        const selectedType = $('#typeSelect').val();
        const searchEndpoint = getSearchEndpoint(selectedType);

        // Clear previous results
        $('#noResultsMessage').addClass('d-none');
        $('#resultsTable').addClass('d-none');

        try {
            // Get CSRF token from page
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

            if (!token) {
                console.error('CSRF token not found on page');
                alert('Security token missing. Please refresh the page.');
                return;
            }


            // Create FormData and add both armyNo AND token
            const params = new URLSearchParams();
            params.append('armyNo', armyNo);
            params.append('aadharNo', aadharNo);


            // Disable submit button to prevent double submission
            const submitButton = $(this).find('button[type="submit"]');
            const originalText = submitButton.text();
            submitButton.prop('disabled', true).text('Searching...');

            const response = await fetch(searchEndpoint, {
                method: 'POST',
                body: params,
                credentials: 'same-origin',
                headers: {
                    'RequestVerificationToken': token,
                    'X-Requested-With': 'XMLHttpRequest' // Optional but good practice
                }
            });


            if (!response.ok) {
                if (response.status === 400) {
                    console.error('CSRF validation failed');
                    alert('Security validation failed. Please refresh the page.');
                    return;
                }
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();

            // Handle response
            if (data && data.length > 0) {
                populateTable(data);
                $('#resultsTable').removeClass('d-none');
            } else {
                $('#noResultsMessage').removeClass('d-none');
            }

        } catch (error) {
            console.error('Search error:', error);
            alert('Search failed. Please try again.');
        } finally {
            // Re-enable submit button
            const submitButton = $(this).find('button[type="submit"]');
            submitButton.prop('disabled', false).text('Search');
        }
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


    //$(document).on('click', '.editapp', function () {
    //    // Adjust the URL according to your routing
    //    const type = $('#typeSelect').val();

    //    const appId = $(this).data('app-id');  // Get application ID from button
    //    if (!appId) return;

    //    if (type === 'Loan')
    //        window.location.href = `/OnlineApplication/OnlineApplication/${appId}`;
    //    else if (type === 'Maturity')
    //        window.location.href = `/Claim/OnlineApplication/${appId}`;


    //    //window.location.href = `/OnlineApplication/OnlineApplication/${appId}`;
    //});
    $(document).on('click', '.editapp', function () {
        // Get the loan type and application ID
        const type = $('#typeSelect').val();
        const appId = $(this).data('app-id');  // Get application ID from button

        if (!appId) return;

        // Prepare the data to send to the server
        const requestData = {
            appId: appId,
            type: type
        };

        // Send AJAX request to the server to process the data
        $.ajax({
            url: '/OnlineApplication/HandleApplicationRedirect', // Your controller and action to handle the logic
            type: 'POST',
            data: requestData,
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success) {
                    // If the server returns a URL to redirect to, perform the redirect
                    window.location.href = response.redirectUrl; // Redirect based on server response
                } else {
                    // Handle any errors if necessary
                    alert('Error: ' + response.message);
                }
            },
            error: function () {
                alert('An error occurred while processing the request');
            }
        });
    });


    $(document).on('click', '.downloadApplication', function () {

        const appId = $(this).data('app-id');
        const type = $('#typeSelect').val();
        if (!appId) return;

        downloadApplication(appId,type);
    });

    function downloadApplication(applicationId, type) {
        if (!applicationId) {
            alert('Application ID is required for download');
            return;
        }

        // Determine the action URL
        let actionUrl = '';
        if (type === 'Loan') {
            actionUrl = '/Default/DownloadApplication';
        } else if (type === 'Maturity') {
            actionUrl = '/Default/DownloadClaimApplication';
        } else {
            alert('Invalid download type');
            return;
        }

        // Create a hidden form
        const form = document.createElement('form');
        form.method = 'POST';
        form.action = actionUrl;
        form.style.display = 'none';

        // Add CSRF token (get from your page)
        const csrfToken = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
      
            const tokenInput = document.createElement('input');
            tokenInput.type = 'hidden';
            tokenInput.name = '__RequestVerificationToken';
            tokenInput.value = csrfToken;
            form.appendChild(tokenInput);
        

        // Add application ID (encrypted on server side)
        const idInput = document.createElement('input');
        idInput.type = 'hidden';
        idInput.name = 'id';
        idInput.value = applicationId;
        form.appendChild(idInput);

        // Add form to body, submit, then remove
        document.body.appendChild(form);
        form.submit();
        document.body.removeChild(form);
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
            type: 'POST',
            data: { applicationId: appId },
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
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