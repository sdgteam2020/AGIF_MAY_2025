var errormsg002 = "Someting Went Wrong";
function mMsater(sectid , ddl, TableId, ParentId) {

  
    var userdata =
    {
        "id": TableId,
        "ParentId": ParentId,

    };
    $.ajax({
        url: '/Master/GetAllMMaster',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == 0) {
                    Swal.fire({
                        text: errormsg002
                    });
                } else if (response == -1) {
                    Swal.fire({
                        text: errormsg002
                    });
                }

                else  {

                    var listItemddl = "";
                 
                    listItemddl += '<option value="">Please Select</option>';
                   
                    for (var i = 0; i < response.length; i++) {
                      
                          listItemddl += '<option value=' + response[i].id + '>' + response[i].name +'</option>';
                    }
                    $("#" + ddl + "").html(listItemddl);
              
                    if (sectid != '') {
                        $("#" + ddl + "").val(sectid);

                    }

                }
            }
            else {
                //Swal.fire({
                //    text: "No data found Offrs"
                //});
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}