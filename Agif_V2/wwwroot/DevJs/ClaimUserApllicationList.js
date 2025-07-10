/*import { forEach } from "angular";*/

$(document).ready(function () {
    // Check if DataTables is loaded
    if (typeof $.fn.DataTable === 'undefined') {
        console.error('DataTables library is not loaded!');
        alert('DataTables library is not loaded. Please check your script references.');
        return;
    }
    const params = new URLSearchParams(window.location.search);
    const value = params.get("status");
    BindUsersData(value);
});

$(document).on('click', '.download-btn', function () {
    const applicationId = $(this).data('id');
    const armyNo = $(this).data('army');
    const applicationType = $(this).data('type');
    downloadApplication(applicationId, armyNo, applicationType);
});

function BindUsersData(status) {
    // Check if DataTables is available

    if (typeof $.fn.DataTable === 'undefined') {
        console.error('DataTables is not available');
        return;
    }

    // Destroy existing DataTable if it exists
    if ($.fn.DataTable.isDataTable('#tblReceivedApplications')) {
        $('#tblReceivedApplications').DataTable().destroy();
    }

    var columns = [
        {
            data: null,
            name: "SerialNumber",
            orderable: false,
            render: function (data, type, row, meta) {
                return meta.row + meta.settings._iDisplayStart + 1;
            }
        },
        {
            data: "armyNo",
            name: "ArmyNo",
            render: function (data, type, row) {
                return data || 'N/A';
            }
        },
        {
            data: "name",
            name: "Name",
            render: function (data, type, row) {
                return data || 'N/A';
            }
        },
        {
            data: "regtCorps",
            name: "RegtCorps",
            render: function (data, type, row) {
                return data || 'N/A';
            }
        },
        {
            data: "presentUnit",
            name: "PresentUnit",
            render: function (data, type, row) {
                return data || 'N/A';
            }
        },
        {
            data: "appliedDate",
            name: "AppliedDate",
            render: function (data, type, row) {
                return data || 'N/A';
            }
        }
    ];

    // Add conditional columns for status = 4
    if (status === '4') {
        columns.push(
            {
                data: "downloadCount",
                name: "DownloadCount",
                render: function (data, type, row) {
                    return data || '0';
                }
            },
            {
                data: "downloadedOn",
                name: "DownloadedOn",
                render: function (data, type, row) {
                    if (data) {
                        // Format the date if needed
                        var date = new Date(data);
                        return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
                    }
                    return 'N/A';
                }
            }
        );
    }

    // Add download button column (always last)
    columns.push({
        data: null,
        name: "Download",
        orderable: false,
        render: function (data, type, row) {
            return `<button class="btn btn-primary btn-sm download-btn"
                data-id="${row.applicationId}"
                data-army="${row.armyNo}"
                data-type="${row.applicationType}">
                <i class="bi bi-download"></i>
            </button>`;
        }
    });

    // Initialize DataTable with server-side processing
    var table = $('#tblReceivedApplications').DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        order: [[0, 'desc']], // Default sorting on the first column
        ajax: {
            url: "/ApplicationRequest/GetClaimUsersApplicationListToAdmin",
            type: "POST",
            contentType: "application/x-www-form-urlencoded",
            data: function (data) {
                console.log(data);
                return {
                    'request.Draw': data.draw,
                    'request.Start': data.start,
                    'request.Length': data.length,
                    'request.searchValue': data.search.value,
                    'request.sortColumn': data.order.length > 0 ? data.columns[data.order[0].column].data : '',
                    'request.sortDirection': data.order.length > 0 ? data.order[0].dir : '',
                    'status': status// Pass the status parameter
                };
            },
            error: function (xhr, error, code) {
                console.error('Error loading data:', error);
                console.error('XHR:', xhr);
                console.error('Code:', code);
                alert('Error loading data. Please try again.');
            }
        },
        columns: columns,
        language: {
            search: "",
            searchPlaceholder: "Search applications...",
            processing: "Loading applications...",
            emptyTable: "No applications found",
            info: "Showing _START_ to _END_ of _TOTAL_ applications",
            infoEmpty: "Showing 0 to 0 of 0 applications",
            infoFiltered: "(filtered from _MAX_ total applications)",
            lengthMenu: "Show _MENU_ applications per page",
            paginate: {
                first: "First",
                last: "Last",
                next: "Next",
                previous: "Previous"
            }
        },
        dom: '<"row"<"col-md-6"l><"col-md-6"f>>rt<"row"<"col-md-6"i><"col-md-6"p>>',
        buttons: [
        ],
        drawCallback: function (settings) {
            // Add any custom logic here after table is drawn
        }
    });
}

function downloadApplication(applicationId, armyNo, applicationType) {
    if (!applicationId) {
        alert('Application ID is required for download');
        return;
    }

    var applicationIds = [];
    applicationIds.push(applicationId);

    // Option 2: Using AJAX if you need to handle response differently

    $.ajax({
        url: '/ApplicationRequest/DownloadClaimApplication',
        type: 'GET',
        traditional: true, // VERY IMPORTANT for proper array query string
        data: { id: applicationIds },
        success: function (response) {

            if (response == 0) {
                alert("Error")
            }
            else {
                window.location.href = '/ClaimPdfDownloaded/' + response + ".zip";

            }
            // Handle success
            console.log('Download initiated successfully');
        },
        error: function (xhr, status, error) {
            console.error('Download failed:', error);
            alert('Download failed. Please try again.');
        }
    });

}


function getAllApplicationByDateWise() {
    var date = $('#toDate').val();
    if (!date) {
        alert('Please select a date to filter applications.');
        return;
    }

    $.ajax({
        url: '/ApplicationRequest/GetClaimApplicationByDate',
        type: 'GET',
        data: { date: date },
        success: function (response) {
            if (!response || response.length === 0) {
                alert("No applications found for the selected date.");
                return;
            }

            var applicationIds = [];
            response.forEach(function (item) {
                applicationIds.push(item.applicationId);
            });
            downloadApplications(applicationIds);
            console.log('Applications retrieved:', applicationIds);
        },
        error: function (xhr, status, error) {
            console.error('Request failed:', error);
            alert('Failed to retrieve applications. Please try again.');
        }
    });
}


function downloadApplications(applicationIds) {
    $.ajax({
        url: '/ApplicationRequest/DownloadClaimApplication',
        type: 'GET',
        traditional: true, // ensures array is passed correctly
        data: { id: applicationIds },
        success: function (folderName) {
            if (!folderName || folderName === 0) {
                alert('Download failed or no files found.');
                return;
            }

            // Automatically trigger download of the zip file
            var downloadUrl = `/ClaimPdfDownloaded/${folderName}.zip`;
            window.location.href = downloadUrl; // triggers file download
        },
        error: function (xhr, status, error) {
            console.error('Download failed:', error);
            alert('Download failed. Please try again.');
        }
    });
}
