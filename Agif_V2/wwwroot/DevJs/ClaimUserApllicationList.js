/*import { forEach } from "angular";*/
let validatedRecords = [];
let rejectedRecords = [];
$(document).ready(function () {
    // Check if DataTables is loaded
    if (typeof $.fn.DataTable === 'undefined') {
        console.error('DataTables library is not loaded!');
        alert('DataTables library is not loaded. Please check your script references.');
        return;
    }

    let rawValue = $("#Status").val();
    let value = (rawValue === "0" || !rawValue) ? 102 : rawValue;
    BindUsersData(value);
});


// Export Validated Records
$('#btnExportOk').on('click', function () {
    if (!validatedRecords.length) {
        alert("No validated records to export.");
        return;
    }

    $.ajax({
        url: '/ApplicationRequest/ExportValidatedExcel',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(validatedRecords),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            const link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "ValidatedRecords.xlsx";
            link.click();
        }
    });
});

$('#btnExportNotOk').on('click', function () {
    if (!rejectedRecords.length) {
        alert("No rejected records to export.");
        return;
    }

    $.ajax({
        url: '/ApplicationRequest/ExportRejectedExcel',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rejectedRecords),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            const link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "RejectedRecords.xlsx";
            link.click();
        }
    });
});


$('#UploadExcel').on('click', function () {
    $("#AddBulkModel").modal('show');
});

$('#btnBulkUpload').on('click', function () {
    $("#postBulk-msg").html('');
    const file = $('#fileExcel')[0].files[0];
    if (!file) {
        Swal.fire({
            title: 'Error',
            text: 'Please select a file to upload.',
            icon: 'error'
        });
        return; // Exit the function if no file is selected
    }
    const formData = new FormData();
    formData.append('file', file);
    $.ajax({
        url: '/ApplicationRequest/ClaimUploadExcelFile',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {

            if (response.success) {
                populateUploadTables(response.data);
                validatedRecords = response.data.dtoApplStatusBulkUploadOK || [];
                rejectedRecords = response.data.dtoApplStatusBulkUploadNotOk || [];
                Swal.fire({
                    title: 'Success',
                    text: response.message,
                    icon: 'success'
                }).then(() => {
                    // Reload the DataTable after successful upload
                    BindUsersData($('#Status').val());
                });
            } else {
                $("#postBulk-msg").html(response.message);
            }
        },
    });
});

function populateUploadTables(data) {
    if ($.fn.DataTable.isDataTable('#tblOk')) {
        $('#tblOk').DataTable().clear().destroy();
    }
    if ($.fn.DataTable.isDataTable('#tblNotOk')) {
        $('#tblNotOk').DataTable().clear().destroy();
    }
    $('#OkBody').empty();
    $('#NotOkBody').empty();
    if (data.dtoApplStatusBulkUploadOK && data.dtoApplStatusBulkUploadOK.length > 0) {
        let okTableBody = '';
        data.dtoApplStatusBulkUploadOK.forEach((item, index) => {
            okTableBody += `
                <tr>
                    <td class="noExport d-none">${item.applId}</td>
                    <td class="wd-30-f">${index + 1}</td>
                    <td>${item.applId}</td>
                    <td>${item.armyNo || ''}</td>
                    <td>${item.name || ''}</td>
                    <td>${item.status_Code}</td>
                </tr>
            `;
        });
        $('#OkBody').html(okTableBody);

        //let val = $('#OkBody').html();
        //if (val) {
        //    // Initialize the DataTable after content is loaded
        //    $('#tblOk').DataTable({
        //        dom: 'Bfrtip', // Place the buttons above the table
        //        buttons: [
        //            {
        //                extend: 'print',
        //                text: 'Print Table',
        //                className: 'btn btn-success', // Customize button styling
        //                title: 'Data Table', // The title of the printed page
        //                messageTop: 'Table Print', // Additional message at the top of the print page
        //            }
        //        ],
        //        initComplete: function (settings, json) {
        //            // This ensures the table and buttons are properly rendered
        //            console.log('DataTable initialized');
        //        }
        //    });
        //} else {
        //    console.log('Table body is empty');
        //}


        $('#lblTotalOk').text(data.dtoApplStatusBulkUploadOK.length);
        $('#tblOk').DataTable({
            paging: true,
            searching: false,
            ordering: true,
            info: true,
            pageLength: 10,
            lengthChange: false
        });
    } else {
        $('#lblTotalOk').text('0');
    }

    if (data.dtoApplStatusBulkUploadNotOk && data.dtoApplStatusBulkUploadNotOk.length > 0) {
        let notOkTableBody = '';
        data.dtoApplStatusBulkUploadNotOk.forEach((item, index) => {
            notOkTableBody += `
                <tr>
                    <td class="noExport d-none">${item.applId}</td>
                    <td class="wd-30-f">${index + 1}</td>
                    <td>${item.applId}</td>
                    <td>${item.armyNo || ''}</td>
                    <td>${item.name || ''}</td>
                    <td>${item.status_Code}</td>
                    <td>${item.reason}</td>
                </tr>
            `;
        });
        $('#NotOkBody').html(notOkTableBody);
        $('#lblTotalNotOk').text(data.dtoApplStatusBulkUploadNotOk.length);
        $('#tblNotOk').DataTable({
            paging: true,
            searching: false,
            ordering: true,
            info: true,
            pageLength: 10,
            lengthChange: false
        });
    } else {
        $('#lblTotalNotOk').text('0');
    }
}

$('#btnProcessBulk').on('click', function () {
    $.ajax({
        url: '/ApplicationRequest/ClaimProcessBulkApplications',
        type: 'Get',
        contentType: 'application/json',
        data: {},
        success: function (response) {
            $('#fileExcel').val(''); // Clear the file input
            $('#postBulk-msg').empty(); // Clear any previous messages

            $('#OkBody').empty();
            $('#NotOkBody').empty();
            $('#lblTotalOk').text('0');
            $('#lblTotalNotOk').text('0');
            $('#AddBulkModel').modal('hide');

            if (response.success) {
                Swal.fire({
                    title: 'Success',
                    text: response.message,
                    icon: 'success'
                }).then(() => {
                    // Reload the DataTable after processing
                    BindUsersData($('#Status').val());
                });
            } else {
                Swal.fire({
                    title: 'Error',
                    text: response.message,
                    icon: 'error'
                });
            }
        },
    });
});
$('#UploadExcel1').on('click', function () {
    Swal.fire({
        title: "Upload Excel File",
        text: "Please select an Excel file to upload.",
        input: 'file',
        inputAttributes: {
            accept: '.xlsx,.xls',
            'aria-label': 'Upload your Excel file'
        },
        showCancelButton: true,
        confirmButtonText: 'Upload',
        cancelButtonText: 'Cancel',
        showLoaderOnConfirm: true,
        allowOutsideClick: () => !Swal.isLoading(),
        preConfirm: (file) => {
            if (!file) {
                Swal.showValidationMessage('No file selected. Please select an Excel file to upload.');
                return false;
            }
            const fileName = file.name.toLowerCase();
            if (!fileName.endsWith('.xlsx') && !fileName.endsWith('.xls')) {
                Swal.showValidationMessage('Invalid file type. Please upload an Excel file (.xlsx or .xls).');
                return false;
            }
            return file;
        }
    }).then((result) => {
        if (result.isConfirmed && result.value) {
            const file = result.value;
            const formData = new FormData();
            formData.append('file', file);
            $.ajax({
                url: '/ApplicationRequest/ClaimUploadExcelFile',
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            title: 'Success',
                            text: response.message,
                            icon: 'success'
                        }).then(() => {
                            // Reload the DataTable after successful upload
                            BindUsersData($('#Status').val());
                        });
                    } else {
                        Swal.fire({
                            title: 'Error',
                            text: response.message,
                            icon: 'error'
                        });
                    }
                },
            });
        }
    });
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

    let columns = [
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

    // Add conditional columns for status = 104
    
    if (status === '104') {
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
                        const date = new Date(data);
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
    const table = $('#tblReceivedApplications').DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        order: [[0, 'desc']], // Default sorting on the first column
        ajax: {
            url: "/ApplicationRequest/GetClaimUsersApplicationListToAdmin",
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

    let applicationIds = [];
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
    const date = $('#toDate').val();
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

            let applicationIds = [];
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
            const downloadUrl = `/ClaimPdfDownloaded/${folderName}.zip`;
            window.location.href = downloadUrl; // triggers file download
        },
        error: function (xhr, status, error) {
            console.error('Download failed:', error);
            alert('Download failed. Please try again.');
        }
    });
}
