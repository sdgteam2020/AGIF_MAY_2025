$(document).ready(function () {
    GetApplicationList(1);
});



async function GetApplicationList(TypeId) {
        var listitem = "";
        debugger;
        try {
            const response = await fetch(`/CORole/GetAllUSerApplicationlist?TypeId=${TypeId}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();

            if (data.length > 0) {
                console.log(data);

                // Clear existing DataTable if it exists
                if ($.fn.dataTable.isDataTable('#tblList')) {
                    $('#tblList').DataTable().clear().destroy();
                }

                // Add extra table headers for Rejected & Approved status
                var extraHeaders = "";
                if (TypeId == 2) { // Rejected
                    extraHeaders = "<th>Rejected Time</th><th>Remarks</th>";
                } else if (TypeId == 3) { // Approved
                    extraHeaders = "<th>Approved Time</th><th>Remarks</th><th>DownloadXML</th>";
                }

                $("#tblList thead tr").html(`
                <th>Sr.No.</th>
                <th>Army No</th>
                <th>Applicant Name</th>
                <th>Application Type</th>
                <th>Date of Birth</th>
                <th>Applied Date</th>
                <th>Present Status</th>
                ${extraHeaders} 
                <th>Action</th>
            `);

                data.forEach((item, index) => {
                    listitem += "<tr>";
                    listitem += "<td>" + (index + 1) + "</td>";
                    listitem += "<td>" + item.Army_No + "</td>";
                    listitem += "<td>" + item.Loanee_Name + "</td>";
                    listitem += "<td>" + item.ApplicationType + "</td>";
                    listitem += "<td>" + item.Date_Of_BirthString + "</td>";
                    listitem += "<td>" + item.DateTimeUpdatedString + "</td>";

                    if (TypeId == 2) { // Rejected Applications
                        listitem += "<td>Rejected</td>";
                        listitem += "<td>" + (item.RejectedTimeString || "N/A") + "</td>";
                        listitem += "<td>" + (item.RejectedApprovedRemarks || "No Remarks") + "</td>";
                    }
                    else if (TypeId == 3) { // Approved Applications
                        listitem += "<td>Approved</td>";
                        listitem += "<td>" + (item.ApprovedTimeString || "N/A") + "</td>";
                        listitem += "<td>" + (item.RejectedApprovedRemarks || "No Remarks") + "</td>";
                        if (item.IsXMLDownload === true) {
                            listitem += "<td> <button data-id='" + item.Application_Id + "' class='downloadXML'>Download XML</button></td>";
                        } else {
                            listitem += "<td></td>"; // Empty cell if no download available
                        }
                    }
                    else { // Pending Applications
                        listitem += "<td>Pending</td>";
                    }

                    // Add action column
                    if (item.IsMergePdf == true) {
                        listitem += "<td><a title='View Details' href='/CORole/ViewDetails/" + item.EnApplication_Id + "'><i class='fa fa-eye' style='color: #0080FF; font-size: 30px;'></i></a></td>";
                    } else {
                        listitem += "<td><a title='View Details' id='mergePdf' data-id='" + item.EnApplication_Id + "'><i class='fa fa-eye' style='color: #0080FF; font-size: 30px;'></i></a></td>";
                    }

                    listitem += "</tr>";
                });

                $("#DetailBody").html(listitem);

                $('#tblList').DataTable({
                    "paging": true,
                    "lengthChange": true,
                    "searching": true,
                    "info": true,
                    "autoWidth": false,
                    "responsive": true
                });
            } else {
                $('#DetailBody').html("<tr><td colspan='10' style='text-align:center;'>No applications found.</td></tr>");
            }
        } catch (error) {
            console.log("Error: " + error.message);
            $('#DetailBody').html("<tr><td colspan='10' style='text-align:center;'>An Error Occurred.</td></tr>");
        }
}
    