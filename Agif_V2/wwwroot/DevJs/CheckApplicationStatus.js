$(document).ready(function () {

    // =========================================================
    // 1. ENCRYPTION HELPER FUNCTION
    // =========================================================
    function encryptPayload(plainText, keyBase64) {
        if (!keyBase64) {
            console.error("Encryption key is missing!");
            return "";
        }
        var key = CryptoJS.enc.Base64.parse(keyBase64);
        var iv = CryptoJS.lib.WordArray.random(16);

        var encrypted = CryptoJS.AES.encrypt(plainText, key, {
            iv: iv,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });

        var ivAndCipher = iv.clone().concat(encrypted.ciphertext);
        var hmac = CryptoJS.HmacSHA256(ivAndCipher, key);
        var finalData = ivAndCipher.clone().concat(hmac);

        return CryptoJS.enc.Base64.stringify(finalData);
    }

    // =========================================================
    // 2. UI INITIALIZATION & INPUT FORMATTERS
    // =========================================================
    $('#typeSelect').on('change', function () {
        clearAllData();
    });

    $('#armyNoInput').on('input', function () {
        let sanitizedValue = $(this).val().replace(/[^a-zA-Z0-9]/g, '');
        sanitizedValue = sanitizedValue.substring(0, 14);
        $(this).val(sanitizedValue.toUpperCase());
    });

    $('#aadharNoInput').on('input', function () {
        formatAadhar(this);
    });

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

    function clearAllData() {
        $('#resultsTable').addClass('d-none');
        $('#noResultsMessage').addClass('d-none');
        $('#applicationTableBody').empty();
        $('#armyNoInput').val('');
        $('#aadharNoInput').val('');
    }

    // =========================================================
    // 3. MAIN SEARCH (ENCRYPTED)
    // =========================================================
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

        $('#noResultsMessage').addClass('d-none');
        $('#resultsTable').addClass('d-none');

        try {
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            const serverKey = $('#serverCryptoKey').val();

            if (!token) {
                console.error('CSRF token not found on page');
                alert('Security token missing. Please refresh the page.');
                return;
            }
            if (!serverKey) {
                alert('Security key missing. Please refresh the page.');
                return;
            }

            const submitButton = $(this).find('button[type="submit"]');
            submitButton.prop('disabled', true).text('Searching...');

            // Encrypt the payload
            const searchData = { armyNo: armyNo, aadharNo: aadharNo };
            const encryptedString = encryptPayload(JSON.stringify(searchData), serverKey);

            // Append encrypted string to params
            const params = new URLSearchParams();
            params.append('EncryptedData', encryptedString);

            const response = await fetch(searchEndpoint, {
                method: 'POST',
                body: params,
                credentials: 'same-origin',
                headers: {
                    'RequestVerificationToken': token,
                    'X-Requested-With': 'XMLHttpRequest'
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
            const submitButton = $(this).find('button[type="submit"]');
            submitButton.prop('disabled', false).text('Search');
        }
    });

    function getSearchEndpoint(type) {
        switch (type) {
            case 'Loan':
                return '/Default/SearchByArmyNo';
            case 'Maturity':
                return '/Default/ClaimSearchByArmyNo';
            default:
                return '/Default/SearchByArmyNo';
        }
    }

    // =========================================================
    // 4. EDIT APPLICATION (ENCRYPTED)
    // =========================================================
    $(document).on('click', '.editapp', function () {
        const type = $('#typeSelect').val();
        const appId = $(this).data('app-id');

        if (!appId) return;

        const requestData = { appId: appId.toString(), type: type };
        const serverKey = $('#serverCryptoKey').val();
        const encryptedString = encryptPayload(JSON.stringify(requestData), serverKey);

        $.ajax({
            url: '/OnlineApplication/HandleApplicationRedirect',
            type: 'POST',
            data: { EncryptedData: encryptedString },
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success) {
                    window.location.href = response.redirectUrl;
                } else {
                    alert('Error: ' + response.message);
                }
            },
            error: function () {
                alert('An error occurred while processing the request');
            }
        });
    });

    // =========================================================
    // 5. DOWNLOAD APPLICATION (ENCRYPTED)
    // =========================================================
    $(document).on('click', '.downloadApplication', function () {
        const appId = $(this).data('app-id');
        const type = $('#typeSelect').val();

        if (!appId) return;
        downloadApplication(appId, type);
    });

    function downloadApplication(applicationId, type) {
        if (!applicationId) {
            alert('Application ID is required for download');
            return;
        }

        let actionUrl = '';
        if (type === 'Loan') {
            actionUrl = '/Default/DownloadApplication';
        } else if (type === 'Maturity') {
            actionUrl = '/Default/DownloadClaimApplication';
        } else {
            alert('Invalid download type');
            return;
        }

        const form = document.createElement('form');
        form.method = 'POST';
        form.action = actionUrl;
        form.style.display = 'none';

        const csrfToken = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        const tokenInput = document.createElement('input');
        tokenInput.type = 'hidden';
        tokenInput.name = '__RequestVerificationToken';
        tokenInput.value = csrfToken;
        form.appendChild(tokenInput);

        // Encrypt the download payload
        const requestData = { id: applicationId.toString() };
        const serverKey = $('#serverCryptoKey').val();
        const encryptedString = encryptPayload(JSON.stringify(requestData), serverKey);

        const encryptedInput = document.createElement('input');
        encryptedInput.type = 'hidden';
        encryptedInput.name = 'EncryptedData';
        encryptedInput.value = encryptedString;
        form.appendChild(encryptedInput);

        document.body.appendChild(form);
        form.submit();
        document.body.removeChild(form);
    }

    // =========================================================
    // 6. GET TIMELINE (ENCRYPTED)
    // =========================================================
    $(document).on('click', '.timeline-btn', function () {
        const appId = $(this).data('app-id');
        const timelineContent = $('#timeline-content-' + appId);
        const loadingDiv = $('#loading-' + appId);
        let selectedType = $('#typeSelect').val();
        let endpoint = '';

        if (timelineContent.children().length > 0) {
            return; // Timeline already loaded, just toggle
        }

        loadingDiv.show();
        timelineContent.hide();

        if (selectedType === 'Loan')
            endpoint = '/Default/GetTimeline';
        else if (selectedType === 'Maturity')
            endpoint = '/Default/GetClaimTimeline';

        // Encrypt the timeline request
        const requestData = { applicationId: appId.toString() };
        const serverKey = $('#serverCryptoKey').val();
        const encryptedString = encryptPayload(JSON.stringify(requestData), serverKey);

        $.ajax({
            url: endpoint,
            type: 'POST',
            data: { EncryptedData: encryptedString },
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                loadingDiv.hide();

                if (response && response.length > 0) {
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

    // =========================================================
    // 7. UI RENDER HELPERS
    // =========================================================
    function populateTable(applications) {
        const tbody = $('#applicationTableBody');
        tbody.empty(); // Clear old results

        $.each(applications, function (index, app) {
            const safeAppId = (app.applicationId !== undefined && app.applicationId !== null) ? app.applicationId : index;

            let extraButtonHtml = '';
            let downloadButtonHtml = '';

            if (app.statusId !== 1 && app.statusId !== 101) {
                downloadButtonHtml = `
                <button class="btn btn-danger ms-2 btn-icon downloadApplication"
                    type="button"
                    title="Download"
                    data-app-id="${safeAppId}">
                    <i class="bi bi-download"></i>
                </button>`;
            }
            if (app.statusId === 41 || app.statusId === 161) {
                extraButtonHtml = `
                <button class="btn btn-warning ms-2 btn-icon editapp"
                    type="button"
                    title="Edit Application"
                    data-app-id="${safeAppId}">
                    <i class="bi bi-pencil-square"></i>
                </button>`;
            }

            const rowHtml = `
                <tr>
                    <td>${index + 1}.</td>
                    <td class="statusList">${app.applicationType || 'N/A'}</td>
                    <td class="statusList">
                        <span class="${getStatusBadgeClass(app.statusId)} statusList">${app.status || 'Unknown'}</span>
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
                    <td colspan="5">
                        <div class="timeline-loading" id="loading-${safeAppId}">
                            <i class="fas fa-spinner fa-spin"></i> Loading timeline...
                        </div>
                        <div class="timeline-container" id="timeline-content-${safeAppId}">
                            </div>
                    </td>
                </tr>
            `;
            // Note: Updated colspan to 5 above to match your 5 columns (index, type, status, actions, remarks)
            tbody.append(rowHtml);
        });
    }

    function buildTimelineHtml(timelineData) {
        let timelineHtml = '<div class="timeline-vertical">';

        $.each(timelineData, function (index, step) {
            const isLast = index === timelineData.length - 1;
            const stepClass = isLast ? 'timeline-step last' : 'timeline-step';

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
            const parts = dateString.split('-');
            if (parts.length === 3) {
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