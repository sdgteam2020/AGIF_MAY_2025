$(document).ready(function () {
    let rawValue = $("#Status").val();
    let value = rawValue !== "false"; // true if "true" or blank; false only when "false"

    // Add CSS for toggle slider
    //$('<style>')
    //    .prop('type', 'text/css')
    //    .html(`
    //        .toggle-switch {
    //            position: relative;
    //            display: inline-block;
    //            width: 60px;
    //            height: 28px;
    //        }

    //        .toggle-switch input {
    //            opacity: 0;
    //            width: 0;
    //            height: 0;
    //        }

    //        .slider {
    //            position: absolute;
    //            cursor: pointer;
    //            top: 0;
    //            left: 0;
    //            right: 0;
    //            bottom: 0;
    //            background-color: #dc3545;
    //            transition: .4s;
    //            border-radius: 28px;
    //        }

    //        .slider:before {
    //            position: absolute;
    //            content: "";
    //            height: 20px;
    //            width: 20px;
    //            left: 4px;
    //            bottom: 4px;
    //            background-color: white;
    //            transition: .4s;
    //            border-radius: 50%;
    //        }

    //        input:checked + .slider {
    //            background-color: #198754;
    //        }

    //        input:checked + .slider:before {
    //            transform: translateX(32px);
    //        }

    //        .status-text {
    //            font-size: 0.8rem;
    //            margin-left: 10px;
    //            font-weight: 500;
    //        }

    //        .status-active {
    //            color: #198754;
    //        }

    //        .status-inactive {
    //            color: #dc3545;
    //        }

    //        .action-container {
    //            display: flex;
    //            align-items: center;
    //            gap: 10px;
    //        }
    //    `)
    //    .appendTo('head');

    BindUsersData(value);
});

$('#btnDownloadExcel').on('click', function () {
    Swal.fire({
        title: 'Processing...',
        text: 'Your file is being generated.',
        icon: 'info',
        showConfirmButton: false,
        allowOutsideClick: false,
        willOpen: () => {
            Swal.showLoading();
        }
    });
    
    $.ajax({
        url: '/Account/ExportAllUsersToExcel', 
        type: 'POST',
        xhrFields: {
            responseType: 'blob'  
        },
        success: function (response) {
            Swal.close();
            
            const link = document.createElement('a');
            link.href = URL.createObjectURL(response);
            link.download = "UsersList.xlsx";
            link.click();
        },
        error: function () {
            Swal.close();
            Swal.fire({
                title: 'Error',
                text: 'An error occurred while exporting the data.',
                icon: 'error'
            });
        }
    });
});



function BindUsersData(status) {
    // Destroy existing DataTable if it exists
    if ($.fn.DataTable.isDataTable('#tblData')) {
        $('#tblData').DataTable().destroy();
    }

    // Initialize DataTable with server-side processing
    let table = $('#tblData').DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        order: [[0, 'desc']], // Default sorting on the first column
        ajax: {
            url: "/Account/GetAllUsersListPaginated",
            type: "POST",
            contentType: "application/x-www-form-urlencoded",
            data: function (data) {
                // Set default values for sortColumn and sortDirection if no sorting is applied
                const sortColumn = data.order.length > 0 ? data.columns[data.order[0].column].data : 'profileName'; // Default column if none selected
                const sortDirection = data.order.length > 0 ? data.order[0].dir : 'asc'; // Default direction if none selected

                return {
                    draw: data.draw,
                    start: data.start,
                    length: data.length,
                    searchValue: data.search.value,
                    sortColumn: sortColumn, // Ensure it has a default
                    sortDirection: sortDirection, // Ensure it has a default
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
                data: "domainId",
                name: "domainId",
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
                data: "profileName",
                name: "ProfileName",
                render: function (data, type, row) {
                    return data || 'N/A';
                }
            },
            {
                data: "appointmentName",
                name: "AppointmentName",
                render: function (data, type, row) {
                    return data || 'N/A';
                }
            },
            {
                data: "unitName",
                name: "UnitName",
                render: function (data, type, row) {
                    return data || 'N/A';
                }
            },
            {
                data: "mobileNo",
                name: "MobileNo",
                render: function (data, type, row) {
                    return data || 'N/A';
                }
            },
            {
                data: "emailId",
                name: "EmailId",
                render: function (data, type, row) {
                    return data ? `<a href='mailto:${data}'>${data}</a>` : 'N/A';
                }
            },
         
          
            //{
            //    data: "regtName",
            //    name: "RegtName",
            //    render: function (data, type, row) {
            //        return data || 'N/A';
            //    }
            //},
            {
                data: "isPrimary",
                name: "IsPrimary",
                orderable: false,
                className: 'noExport',
                render: function (data, type, row) {
                    const isPrimary = row.isPrimary || false;
                    const statusText = isPrimary ? 'Primary' : 'Secondary';
                    const statusClass = isPrimary ? 'status-active' : 'status-inactive';

                    return `
                        <div class='action action-container'>
                            <label class="toggle-switch">
                                <input type="checkbox" class="cls-toggle-primary" data-domain-id='${row.domainId || ''}' ${isPrimary ? 'checked' : ''}>
                                <span class="slider"></span>
                            </label>
                            <span class="status-text ${statusClass}">${statusText}</span>
                        </div>
                    `;
                }
            },
            {
                data: null,
                orderable: false,
                className: 'noExport',
                render: function (data, type, row) {
                    const isActive = row.isActive || false; // Assuming your data has isActive field
                    const statusText = isActive ? 'Active' : 'Inactive';
                    const statusClass = isActive ? 'status-active' : 'status-inactive';

                    return `
                        <div class='action action-container'>
                            <label class="toggle-switch">
                                <input type="checkbox" class="cls-toggle-status" data-domain-id='${row.domainId || ''}' ${isActive ? 'checked' : ''}>
                                <span class="slider"></span>
                            </label>
                            <span class="status-text ${statusClass}">${statusText}</span>
                        </div>
                    `;
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
        dom: '<"row"<"col-md-6"l><"col-md-6"f>>rt<"row"<"col-md-6"i><"col-md-6"p>>',
        drawCallback: function (settings) {
            // Re-bind toggle switch events after each draw
            $('#tblData tbody').off('change', '.cls-toggle-status').on('change', '.cls-toggle-status', function () {
                const $toggle = $(this);
                const domainId = $toggle.data('domain-id');
                const isActive = $toggle.is(':checked');
                const statusText = $toggle.closest('.action-container').find('.status-text');

                // Revert the toggle immediately; will be set again on confirm
                $toggle.prop('checked', !isActive);

                Swal.fire({
                    title: `Are you sure?`,
                    text: `Do you want to ${isActive ? 'activate' : 'deactivate'} this user?`,
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        // Set back the correct toggle value
                        $toggle.prop('checked', isActive);

                        // Update status text immediately for better UX
                        if (isActive) {
                            statusText.text('Active').removeClass('status-inactive').addClass('status-active');
                        } else {
                            statusText.text('Inactive').removeClass('status-active').addClass('status-inactive');
                        }

                        // Call function to update user status
                        updateUserStatus(domainId, isActive, $toggle);
                    }
                });
            });
            $('#tblData tbody').off('change', '.cls-toggle-primary').on('change', '.cls-toggle-primary', function () {
                const $toggle = $(this);
                const domainId = $toggle.data('domain-id');
                const isPrimary = $toggle.is(':checked');
                const statusText = $toggle.closest('.action-container').find('.status-text');

                // Revert the toggle immediately; will be set again on confirm
                $toggle.prop('checked', !isPrimary);

                Swal.fire({
                    title: `Are you sure?`,
                    text: `Do you want to set this user as ${isPrimary ? 'Primary' : 'Secondary'}?`,
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        // Set back the correct toggle value
                        $toggle.prop('checked', isPrimary);

                        // Update status text immediately for better UX
                        if (isPrimary) {
                            statusText.text('Primary').removeClass('status-inactive').addClass('status-active');
                        } else {
                            statusText.text('Secondary').removeClass('status-active').addClass('status-inactive');
                        }

                        // Call function to update user primary status
                        updateUserPrimary(domainId, isPrimary, $toggle);
                    }
                });
            });
        }

    });
}

// Function to handle user status update
function updateUserStatus(domainId, isActive, toggleElement) {
    $.ajax({
        url: "/Account/UpdateUserStatus", 
        type: "POST",
        data: {
            domainId: domainId,
            isActive: isActive
        },
        success: function (response) {
            if (response.success) {
                $('#tblData').DataTable().ajax.reload(null, false);
                showSuccessMessage(`User status updated to: ${isActive ? 'Active' : 'Inactive'}`);
            }
            else {
                revertToggle(toggleElement, !isActive);
                console.error('Failed to update user status:', response.message);
                showErrorMessage('Failed to update user status: ' + response.message);
            }
        },
        error: function (xhr, status, error) {
            revertToggle(toggleElement, !isActive);
            console.error('Error updating user status:', error);
        }
    });
}

function updateUserPrimary(domainId, isPrimary, toggleElement) {
    $.ajax({
        url: "/Account/UpdateUserPrimary", // You'll need to create this endpoint
        type: "POST",
        data: {
            domainId: domainId,
            isPrimary: isPrimary
        },
        success: function (response) {
            if (response.success && isPrimary) {
                $('#tblData').DataTable().ajax.reload(null, false);
                showSuccessMessage(`User role updated to: Primary'}`);
            } else if (response.success && !isPrimary) {
                $('#tblData').DataTable().ajax.reload(null, false);
                showErrorMessage(`You cannot change primary to secondary`);
            }
            else {
                revertToggle(toggleElement, !isPrimary, 'primary');
                console.error('Failed to update user primary status:', response.message);
                showErrorMessage('Failed to update user role: ' + response.message);
            }
        },
        error: function (xhr, status, error) {
            revertToggle(toggleElement, !isPrimary, 'primary');
            console.error('Error updating user primary status:', error);
            showErrorMessage('Error updating user role. Please try again.');
        }
    });
}

function revertToggle(toggleElement, originalState, toggleType) {
    toggleElement.prop('checked', originalState);
    const statusText = toggleElement.closest('.action-container').find('.status-text');

    if (toggleType === 'status') {
        if (originalState) {
            statusText.text('Active').removeClass('status-inactive').addClass('status-active');
        } else {
            statusText.text('Inactive').removeClass('status-active').addClass('status-inactive');
        }
    } else if (toggleType === 'primary') {
        if (originalState) {
            statusText.text('Primary').removeClass('status-inactive').addClass('status-active');
        } else {
            statusText.text('Secondary').removeClass('status-active').addClass('status-inactive');
        }
    }
}

