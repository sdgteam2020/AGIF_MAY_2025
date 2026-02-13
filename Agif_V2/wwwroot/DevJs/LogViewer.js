//$(document).ready(function () {
//    // Check if DataTables is loaded
//    if (typeof $.fn.DataTable === 'undefined') {
//        console.error('DataTables library is not loaded!');
//        alert('DataTables library is not loaded. Please check your script references.');
//        return;
//    }
//    BindUsersData();
//});


//function BindUsersData() {

//    if (typeof $.fn.DataTable === 'undefined') {
//        console.error('DataTables is not available');
//        return;
//    }

//    // Destroy existing DataTable if it exists
//    if ($.fn.DataTable.isDataTable('#tblApprovedLog')) {
//        $('#tblApprovedLog').DataTable().destroy();
//    }

//    const columns = [
//        {
//            data: null,
//            name: "SerialNumber",
//            orderable: false,
//            render: function (data, type, row, meta) {
//                return meta.row + meta.settings._iDisplayStart + 1;
//            }
//        },
//        {
//            data: "name",
//            name: "Name",
//            render: function (data, type, row) {
//                return data || 'N/A';
//            }
//        },
//        {
//            data: "domainId",
//            name: "DomainId",
//            render: function (data, type, row) {
//                return data || 'N/A';
//            }
//        },
//        {
//            data: "ipAddress",
//            name: "IpAddress",
//            render: function (data, type, row) {
//                return data || 'N/A';
//            }
//        },
//        {
//            data: "coDomainId",
//            name: "CoDomainId",
//            render: function (data, type, row) {
//                return data || 'N/A';
//            }
//        },
//        {
//            data: "unitName",
//            name: "UnitName",
//            render: function (data, type, row) {
//                return data || 'N/A';
//            }
//        },

//        {
//            data: "isApproved",
//            name: "IsApproved",
//            render: function (data, type, row) {
//                if (data === true) {
//                    return '<span class="text-success fw-bold">Active</span>';
//                } else if (data === false) {
//                    return '<span class="text-danger fw-bold">Inactive</span>';
//                } else {
//                    return 'N/A';
//                }
//            }
//        },

//        {
//            data: "updatedOn",
//            name: "UpdatedOn",
//            render: function (data, type, row) {
//                return data || 'N/A';
//            }
//        }
//    ];
//    var token = $('input[name="__RequestVerificationToken"]').val();
//    // Initialize DataTable with server-side processing
//    $('#tblApprovedLog').DataTable({
//        processing: true,
//        serverSide: true,
//        searching : true,
//        filter: true,
//        order: [[0, 'desc']], // Default sorting on the first column
//        ajax: {
//            url: "/Home/GetApprovedLogs",
//            type: "POST",
//            contentType: "application/x-www-form-urlencoded",
//            "headers": {
//                "RequestVerificationToken": token
//            },
//            data: function (data) {
//                return {
//                    'request.Draw': data.draw,
//                    'request.Start': data.start,
//                    'request.Length': data.length,
//                    'request.searchValue': data.search.value,
//                    'request.sortColumn': data.order.length > 0 ? data.columns[data.order[0].column].data : '',
//                    'request.sortDirection': data.order.length > 0 ? data.order[0].dir : ''
//                };
//            },
//            error: function (xhr, error, code) {
//                console.error('Error loading data:', error);
//                console.error('XHR:', xhr);
//                console.error('Code:', code);
//                alert('Error loading data. Please try again.');
//            }
//        },
//        columns: columns,
//        language: {
//            search: "",
//            searchPlaceholder: "Search applications...",
//            processing: "Loading applications...",
//            emptyTable: "No applications found",
//            info: "Showing _START_ to _END_ of _TOTAL_ applications",
//            infoEmpty: "Showing 0 to 0 of 0 applications",
//            infoFiltered: "(filtered from _MAX_ total applications)",
//            lengthMenu: "Show _MENU_ applications per page",
//            paginate: {
//                first: "First",
//                last: "Last",
//                next: "Next",
//                previous: "Previous"
//            }
//        },
//        dom: '<"row"<"col-md-6"l><"col-md-6"f>>rt<"row"<"col-md-6"i><"col-md-6"p>>',
//        buttons: [
//        ],
//        drawCallback: function (settings) {
//            // Add any custom logic here after table is drawn
//        }
//    });
//}


$(document).ready(function () {
    if (!isDataTableLoaded()) {
        showDataTableError();
        return;
    }

    initializeApprovedLogTable();
});

/* ===========================
   Common Utility Functions
=========================== */

function isDataTableLoaded() {
    return typeof $.fn.DataTable !== 'undefined';
}

function showDataTableError() {
    console.error('DataTables library is not loaded!');
    alert('DataTables library is not loaded. Please check your script references.');
}

function destroyIfExists(tableSelector) {
    if ($.fn.DataTable.isDataTable(tableSelector)) {
        $(tableSelector).DataTable().destroy();
    }
}

function defaultValueRenderer(data) {
    return data || 'N/A';
}

function statusRenderer(data) {
    if (data === true) {
        return '<span class="text-success fw-bold">Active</span>';
    }
    if (data === false) {
        return '<span class="text-danger fw-bold">Inactive</span>';
    }
    return 'N/A';
}

function serialNumberRenderer(data, type, row, meta) {
    return meta.row + meta.settings._iDisplayStart + 1;
}

/* ===========================
   DataTable Initialization
=========================== */

function initializeApprovedLogTable() {

    const TABLE_ID = '#tblApprovedLog';
    const API_URL = '/Home/GetApprovedLogs';
    const token = $('input[name="__RequestVerificationToken"]').val();

    destroyIfExists(TABLE_ID);

    const columns = [
        {
            data: null,
            name: "SerialNumber",
            orderable: false,
            render: serialNumberRenderer
        },
        { data: "name", name: "Name", render: defaultValueRenderer },
        { data: "domainId", name: "DomainId", render: defaultValueRenderer },
        { data: "ipAddress", name: "IpAddress", render: defaultValueRenderer },
        { data: "coDomainId", name: "CoDomainId", render: defaultValueRenderer },
        { data: "unitName", name: "UnitName", render: defaultValueRenderer },
        { data: "isApproved", name: "IsApproved", render: statusRenderer },
        { data: "updatedOn", name: "UpdatedOn", render: defaultValueRenderer }
    ];

    $(TABLE_ID).DataTable({
        processing: true,
        serverSide: true,
        searching: true,
        filter: true,
        order: [[0, 'desc']],
        ajax: createAjaxConfig(API_URL, token),
        columns: columns,
        language: getLanguageConfig(),
        dom: '<"row"<"col-md-6"l><"col-md-6"f>>rt<"row"<"col-md-6"i><"col-md-6"p>>',
        drawCallback: function () {
            // optional post-draw logic
        }
    });
}

/* ===========================
   Ajax & Language Config
=========================== */

function createAjaxConfig(url, token) {
    return {
        url: url,
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        headers: {
            "RequestVerificationToken": token
        },
        data: function (data) {
            return {
                'request.Draw': data.draw,
                'request.Start': data.start,
                'request.Length': data.length,
                'request.searchValue': data.search.value,
                'request.sortColumn': data.order.length
                    ? data.columns[data.order[0].column].data
                    : '',
                'request.sortDirection': data.order.length
                    ? data.order[0].dir
                    : ''
            };
        },
        error: handleAjaxError
    };
}

function handleAjaxError(xhr, error, code) {
    console.error('Error loading data:', error);
    console.error('XHR:', xhr);
    console.error('Code:', code);
    alert('Error loading data. Please try again.');
}

function getLanguageConfig() {
    return {
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
    };
}
