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
    var table = $('#tblData').DataTable({
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
        //columns: [
        //    {
        //        data: null,
        //        name: "SerialNumber",
        //        orderable: false,
        //        render: function (data, type, row, meta) {
        //            return meta.row + meta.settings._iDisplayStart + 1;
        //        }
        //    },
        //    {
        //        data: "profileName",
        //        name: "ProfileName",
        //        render: function (data, type, row) {
        //            return data || 'N/A';
        //        }
        //    },
        //    {
        //        data: "emailId",
        //        name: "EmailId",
        //        render: function (data, type, row) {
        //            return data ? `<a href='mailto:${data}'>${data}</a>` : 'N/A';
        //        }
        //    },
        //    {
        //        data: "mobileNo",
        //        name: "MobileNo",
        //        render: function (data, type, row) {
        //            return data || 'N/A';
        //        }
        //    },
        //    {
        //        data: "armyNo",
        //        name: "ArmyNo",
        //        render: function (data, type, row) {
        //            return data || 'N/A';
        //        }
        //    },
        //    {
        //        data: "unitName",
        //        name: "UnitName",
        //        render: function (data, type, row) {
        //            return data || 'N/A';
        //        }
        //    },
        //    {
        //        data: "appointmentName",
        //        name: "AppointmentName",
        //        render: function (data, type, row) {
        //            return data || 'N/A';
        //        }
        //    },
        //    {
        //        data: "regtName",
        //        name: "RegtName",
        //        render: function (data, type, row) {
        //            return data || 'N/A';
        //        }
        //    },
        //    {
        //        data: "isPrimary",
        //        name: "IsPrimary",
        //        render: function (data, type, row) {
        //            return data ? 'Primary' : 'Secondary';
        //        }
        //    },
        //    {
        //        data: null,
        //        orderable: false,
        //        className: 'noExport',
        //        render: function (data, type, row) {
        //            const isActive = row.isActive || false; // Assuming your data has isActive field
        //            const statusText = isActive ? 'Active' : 'Inactive';
        //            const statusClass = isActive ? 'status-active' : 'status-inactive';

        //            return `
        //                <div class='action action-container'>
        //                    <label class="toggle-switch">
        //                        <input type="checkbox" class="cls-toggle-status" data-domain-id='${row.domainId || ''}' ${isActive ? 'checked' : ''}>
        //                        <span class="slider"></span>
        //                    </label>
        //                    <span class="status-text ${statusClass}">${statusText}</span>
        //                </div>
        //            `;
        //        }
        //    }
        //],
        //language: {
        //    search: "",
        //    searchPlaceholder: "Search users...",
        //    processing: "Loading users...",
        //    emptyTable: "No users found",
        //    info: "Showing _START_ to _END_ of _TOTAL_ users",
        //    infoEmpty: "Showing 0 to 0 of 0 users",
        //    infoFiltered: "(filtered from _MAX_ total users)",
        //    lengthMenu: "Show _MENU_ users per page",
        //    paginate: {
        //        first: "First",
        //        last: "Last",
        //        next: "Next",
        //        previous: "Previous"
        //    }
        //},
        //dom: 'lBfrtip',
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


//async function GetApplicationList(TypeId) {
//        var listitem = "";
//        debugger;
//        try {
//            const response = await fetch(`/CORole/GetAllUSerApplicationlist?TypeId=${TypeId}`, {
//                method: 'GET',
//                headers: {
//                    'Content-Type': 'application/json'
//                }
//            });

//            if (!response.ok) {
//                throw new Error(`HTTP error! status: ${response.status}`);
//            }

//            const data = await response.json();

//            if (data.length > 0) {
//                console.log(data);

//                // Clear existing DataTable if it exists
//                if ($.fn.dataTable.isDataTable('#tblList')) {
//                    $('#tblList').DataTable().clear().destroy();
//                }

//                // Add extra table headers for Rejected & Approved status
//                var extraHeaders = "";
//                if (TypeId == 2) { // Rejected
//                    extraHeaders = "<th>Rejected Time</th><th>Remarks</th>";
//                } else if (TypeId == 3) { // Approved
//                    extraHeaders = "<th>Approved Time</th><th>Remarks</th><th>DownloadXML</th>";
//                }

//                $("#tblList thead tr").html(`
//                <th>Sr.No.</th>
//                <th>Army No</th>
//                <th>Applicant Name</th>
//                <th>Application Type</th>
//                <th>Date of Birth</th>
//                <th>Applied Date</th>
//                <th>Present Status</th>
//                ${extraHeaders} 
//                <th>Action</th>
//            `);

//                data.forEach((item, index) => {
//                    listitem += "<tr>";
//                    listitem += "<td>" + (index + 1) + "</td>";
//                    listitem += "<td>" + item.Army_No + "</td>";
//                    listitem += "<td>" + item.Loanee_Name + "</td>";
//                    listitem += "<td>" + item.ApplicationType + "</td>";
//                    listitem += "<td>" + item.Date_Of_BirthString + "</td>";
//                    listitem += "<td>" + item.DateTimeUpdatedString + "</td>";

//                    if (TypeId == 2) { // Rejected Applications
//                        listitem += "<td>Rejected</td>";
//                        listitem += "<td>" + (item.RejectedTimeString || "N/A") + "</td>";
//                        listitem += "<td>" + (item.RejectedApprovedRemarks || "No Remarks") + "</td>";
//                    }
//                    else if (TypeId == 3) { // Approved Applications
//                        listitem += "<td>Approved</td>";
//                        listitem += "<td>" + (item.ApprovedTimeString || "N/A") + "</td>";
//                        listitem += "<td>" + (item.RejectedApprovedRemarks || "No Remarks") + "</td>";
//                        if (item.IsXMLDownload === true) {
//                            listitem += "<td> <button data-id='" + item.Application_Id + "' class='downloadXML'>Download XML</button></td>";
//                        } else {
//                            listitem += "<td></td>"; // Empty cell if no download available
//                        }
//                    }
//                    else { // Pending Applications
//                        listitem += "<td>Pending</td>";
//                    }

//                    // Add action column
//                    if (item.IsMergePdf == true) {
//                        listitem += "<td><a title='View Details' href='/CORole/ViewDetails/" + item.EnApplication_Id + "'><i class='fa fa-eye' style='color: #0080FF; font-size: 30px;'></i></a></td>";
//                    } else {
//                        listitem += "<td><a title='View Details' id='mergePdf' data-id='" + item.EnApplication_Id + "'><i class='fa fa-eye' style='color: #0080FF; font-size: 30px;'></i></a></td>";
//                    }

//                    listitem += "</tr>";
//                });

//                $("#DetailBody").html(listitem);

//                $('#tblList').DataTable({
//                    "paging": true,
//                    "lengthChange": true,
//                    "searching": true,
//                    "info": true,
//                    "autoWidth": false,
//                    "responsive": true
//                });
//            } else {
//                $('#DetailBody').html("<tr><td colspan='10' style='text-align:center;'>No applications found.</td></tr>");
//            }
//        } catch (error) {
//            console.log("Error: " + error.message);
//            $('#DetailBody').html("<tr><td colspan='10' style='text-align:center;'>An Error Occurred.</td></tr>");
//        }
//}
    