

function GetApplicationList(status) {
    debugger;
    var listitem = "";
    $.ajax({
        url: "/Application/GetUserApplications",
        type: 'GET',
        data: { "status": status },
        success: function (response) {
            //debugger;
            //if (response.length > 0) {
            //    console.log(response);
            //    if ($.fn.dataTable.isDataTable('#tblList')) {
            //        $('#tblList').DataTable().clear().destroy();
            //    }

            //    // Add extra table headers for Rejected & Approved status
            //    var extraHeaders = "";
            //    if (status == 2) { // Rejected
            //        extraHeaders = "<th><h6>Rejected Time</h6></th><th><h6>Remarks</h6></th>";
            //    } else if (status == 3) { // Approved
            //        extraHeaders = "<th>Approved Time</th><th><h6>Remarks</h6></th><th><h6>DownloadXML</h6></th>";
            //    }


            //    $("#tblList thead tr").html(`
            //        <th><h6>Sr.No.</h6></th>
            //        <th><h6>Army No</h6></th>
            //        <th><h6>Applicant Name</h6></th>
            //        <th><h6>Application Type</h6></th>
            //        <th><h6>Date of Birth</h6></th>
            //        <th><h6>Applied Date</h6></th>
            //        <th><h6>Present Status</h6></th>
            //        ${extraHeaders}
            //        <th><h6>Action</h6></th>
            //    `);

            //    //$.each(response, function (index, item) {
            //    //    listitem += "<tr>";
            //    //    listitem += "<td>" + (index + 1) + "</td>";
            //    //    listitem += "<td>" + item.Army_No + "</td>";
            //    //    listitem += "<td>" + item.Loanee_Name + "</td>";
            //    //    listitem += "<td>" + item.ApplicationType + "</td>";
            //    //    listitem += "<td>" + item.Date_Of_BirthString + "</td>";
            //    //    listitem += "<td>" + item.DateTimeUpdatedString + "</td>";
            //    //    //listitem += "<td>" + item.Application_Id + "</td>";

            //    //    if (TypeId == 2) { // Rejected Applications
            //    //        listitem += "<td>Rejected</td>";
            //    //        listitem += "<td>" + (item.RejectedTimeString || "N/A") + "</td>";
            //    //        listitem += "<td>" + (item.RejectedApprovedRemarks || "No Remarks") + "</td>";
            //    //    }
            //    //    else if (TypeId == 3) { // Approved Applications
            //    //        listitem += "<td>Approved</td>";
            //    //        listitem += "<td>" + (item.ApprovedTimeString || "N/A") + "</td>";
            //    //        listitem += "<td>" + (item.RejectedApprovedRemarks || "No Remarks") + "</td>";
            //    //        if (item.IsXMLDownload === true) {
            //    //            //listitem += "<td> <button data-id="@item.Application_Id" class='downloadXML'>DownloadXML</button></td>"
            //    //            if (item.IsXMLDownload === true) {
            //    //                listitem += "<td> <button data-id='" + item.Application_Id + "' class='downloadXML'>Download XML</button></td>";
            //    //            }

            //    //        }
            //    //    }
            //    //    else { // Pending Applications
            //    //        listitem += "<td>Pending</td>";
            //    //    }

            //    //    // Add action column
            //    //    if (item.IsMergePdf == true) {
            //    //        listitem += "<td><a title='View Details' href='/CORole/ViewDetails/" + item.EnApplication_Id + "'><i class='fa fa-eye' style='color: #0080FF; font-size: 30px;'></i></a></td>";
            //    //    } else {
            //    //        listitem += "<td><a title='View Details' id='mergePdf' data-id='" + item.EnApplication_Id + "'><i class='fa fa-eye' style='color: #0080FF; font-size: 30px;'></i></a></td>";
            //    //    }

            //    //    listitem += "</tr>";
            //    //});

            //    /*$("#tableBody").html(listitem);*/

            //    $('#tblList').DataTable({
            //        "paging": true,
            //        "lengthChange": true,
            //        "searching": true,
            //        "info": true,
            //        "autoWidth": false,
            //        "responsive": true

            //    });
            //} else {
            //    $('#tableBody').html("<tr><td colspan='10' style='text-align:center;'>No applications found.</td></tr>");
            //}
            if (response.length > 0) {
                console.log(response);
            }
        },
        error: function (xhr, status, error) {
            console.log("Error: " + error);
            $('#tableBody').html("<tr><td colspan='10' style='text-align:center;'>An Error Occurred.</td></tr>");
        }
    });
}

$(document).ready(function () {
    var status = $('.table-wrapper .d-none').text().trim();
    GetApplicationList(status)
});