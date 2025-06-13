$(document).ready(function () {
    const params = new URLSearchParams(window.location.search);
    const value = params.get("status");
    GetApplicationList(value);
});

function GetApplicationList(status) {
    // Destroy existing DataTable if it exists
    if ($.fn.DataTable.isDataTable('#tblData')) {
        $('#tblData').DataTable().destroy();
    }

    // Initialize DataTable with server-side processing
    var table = $('#tblApplications').DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        order: [[0, 'desc']], // Default sorting on the first column
        ajax: {
            url: "/ApplicationRequest/GetUsersApplicationList",
            type: "POST",
            contentType: "application/x-www-form-urlencoded",
            data: function (data) {
                console.log(data);
                return {
                    draw: data.draw,
                    start: data.start,
                    length: data.length,
                    searchValue: data.search.value,
                    sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',
                    sortDirection: data.order.length > 0 ? data.order[0].dir : '',
                    status: status // Pass the status parameter
                };

            },
            error: function (xhr, error, code) {
                console.error('Error loading data:', error);
                alert('Error loading data. Please try again.');
            }
        },
        columns: [
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
                    return data ? `<a href='mailto:${data}'>${data}</a>` : 'N/A';
                }
            },
            {
                data: "applicationType",
                name: "ApplicationType",
                render: function (data, type, row) {
                    return data || 'N/A';
                }
            },
            {
                data: "dateOfBirth",
                name: "DateOfBirth",
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
                orderable: false,
                className: 'noExport',
                render: function (data, type, row) {
                    if (row.isMergePdf == false) {
                        return `
                        <div class='action action-container'>
                            <button class='btn btn-sm btn-outline-primary d-flex align-items-center' onclick='mergePdf(${row.applicationId})'>
                                <i class="bi bi-eye"></i>
                                View
                            </button>
                        </div>
                    `;
                    }
                    else {
                        return `
                        <div class='action action-container'>
                            <button class='btn btn-sm btn-outline-primary d-flex align-items-center' onclick='viewDetails(${row.applicationId})'>
                                <i class="bi bi-eye"></i>
                                Views
                            </button>
                        </div>
                    `;
                    }
                    
                }
            }
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
        pageLength: 10,
        lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
        dom: 'lBfrtip',
        //buttons: [
        //    //{
        //    //    extend: 'excel',
        //    //    title: 'Users List',
        //    //    exportOptions: {
        //    //        columns: "thead th:not(.noExport)"
        //    //    }
        //    //}
        //],
        //drawCallback: function (settings) {
        //    // Re-bind toggle switch events after each draw
        //    $('#tblData tbody').off('change', '.cls-toggle-status').on('change', '.cls-toggle-status', function () {
        //        const $toggle = $(this);
        //        const domainId = $toggle.data('domain-id');
        //        const isActive = $toggle.is(':checked');
        //        const statusText = $toggle.closest('.action-container').find('.status-text');

        //        // Revert the toggle immediately; will be set again on confirm
        //        $toggle.prop('checked', !isActive);

        //        Swal.fire({
        //            title: `Are you sure?`,
        //            text: `Do you want to ${isActive ? 'activate' : 'deactivate'} this user?`,
        //            icon: 'warning',
        //            showCancelButton: true,
        //            confirmButtonText: 'Yes',
        //            cancelButtonText: 'No'
        //        }).then((result) => {
        //            if (result.isConfirmed) {
        //                // Set back the correct toggle value
        //                $toggle.prop('checked', isActive);

        //                // Update status text immediately for better UX
        //                if (isActive) {
        //                    statusText.text('Active').removeClass('status-inactive').addClass('status-active');
        //                } else {
        //                    statusText.text('Inactive').removeClass('status-active').addClass('status-inactive');
        //                }

        //                // Call function to update user status
        //                updateUserStatus(domainId, isActive, $toggle);
        //            }
        //        });
        //    });
        //}

    });
}

function mergePdf(applicationId) {
    // Redirect to the merge PDF page with the application ID
    alert(applicationId);
    $.ajax({
        type: "POST",
        url: "/ApplicationRequest/MergePdf",
        data: { applicationId: applicationId},
        success: function (response) {
            if (response.success) {
                window.location.href = "/ApplicationRequest/GetApplicationDetails"; // Redirect to the generated PDF
            } else {
                alert('Error generating PDF: ' + response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
            alert('An error occurred while generating the PDF. Please try again.');
        }
    });
}