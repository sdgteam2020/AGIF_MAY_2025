$(document).ready(function () {
    // Check if DataTables is loaded
    if (typeof $.fn.DataTable === 'undefined') {
        console.error('DataTables library is not loaded!');
        alert('DataTables library is not loaded. Please check your script references.');
        return;
    }



    BindUsersData(2);
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

    // Initialize DataTable with server-side processing
    var table = $('#tblReceivedApplications').DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        order: [[0, 'desc']], // Default sorting on the first column
        ajax: {
            url: "/ApplicationRequest/GetUsersApplicationListToAdmin",
            type: "POST",
            contentType: "application/x-www-form-urlencoded",
            data: function (data) {
                return {
                    'request.Draw': data.draw,
                    'request.Start': data.start,
                    'request.Length': data.length,
                    'request.searchValue': data.search.value,
                    'request.sortColumn': data.order.length > 0 ? data.columns[data.order[0].column].data : '',
                    'request.sortDirection': data.order.length > 0 ? data.order[0].dir : '',
                    'status': status // Pass the status parameter
                };
            },
            error: function (xhr, error, code) {
                console.error('Error loading data:', error);
                console.error('XHR:', xhr);
                console.error('Code:', code);
                alert('Error loading data. Please try again.');
            }
        },
        columns: [
            {
                data: 1,
                name: "SerialNumber",
                orderable: false,
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                data: "presentStatus",
                name: "PresentStatus",
                render: function (data, type, row) {
                    return data || 'N/A';
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
                data: "oldArmyNo",
                name: "OldArmyNo",
                render: function (data, type, row) {
                    return data || 'N/A';
                }
            },
            {
                data: "regtCorps", // Fixed: Changed from "regtCorps" to match your DTO
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
                data: "pcdaPao",
                name: "PcdaPao",
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
            },
            {
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

            }


            // Removed duplicate "presentUnit" column
        ],
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
            // Uncomment and configure as needed
            //{
            //    extend: 'excel',
            //    title: 'Users List',
            //    exportOptions: {
            //        columns: "thead th:not(.noExport)"
            //    }
            //}
        ],
        drawCallback: function (settings) {
            // Add any custom logic here after table is drawn
        }
    });
}

// Function to handle user status update
//function updateUserStatus(domainId, isActive, toggleElement) {
//    $.ajax({
//        url: "/Account/UpdateUserStatus",
//        type: "POST",
//        data: {
//            domainId: domainId,
//            isActive: isActive
//        },
//        success: function (response) {
//            if (response.success) {
//                $('#tblData').DataTable().ajax.reload(null, false);
//                showSuccessMessage(`User status updated to: ${isActive ? 'Active' : 'Inactive'}`);
//            } else {
//                revertToggle(toggleElement, !isActive);
//                console.error('Failed to update user status:', response.message);
//                showErrorMessage('Failed to update user status: ' + response.message);
//            }
//        },
//        error: function (xhr, status, error) {
//            revertToggle(toggleElement, !isActive);
//            console.error('Error updating user status:', error);
//        }
//    });
//}

function downloadApplication(applicationId, armyNo, applicationType) {
    if (!applicationId) {
        alert('Application ID is required for download');
        return;
    }

    var applicationIds = [];
    applicationIds.push(applicationId);

    alert(applicationIds[0])
    //console.log(applicationId + armyNo + applicationType);

    // Option 2: Using AJAX if you need to handle response differently

    $.ajax({
        url: '/ApplicationRequest/DownloadApplication',
        type: 'GET',
        traditional: true, // VERY IMPORTANT for proper array query string
        data: { id: applicationIds },
        success: function (response) {

            if (response == 0) {
                alert("Error")
            }
            else {
                window.location.href = '/TempUploads/' + response+".zip";
             
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


