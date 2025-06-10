

function GetApplicationList(status) {
    debugger;
    var listitem = "";
    $.ajax({
        url: "/Application/GetUserApplications",
        type: 'GET',
        data: { "status": status },
        success: function (response) {
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