








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
