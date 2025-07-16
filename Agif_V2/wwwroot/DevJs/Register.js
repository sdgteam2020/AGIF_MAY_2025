function loadDropdown() {

    var Rank = $('#rank').data('rank-prefix');
  //  var UnitId = $('#UnitId').val();
    var regtCorps = $('#regtCorps').data('regtcorps-prefix');
    var ApptId = $('#ApptId').data('apptid-prefix');

    mMsater(Rank, "rank", 3, 0);
    mMsater(regtCorps, "regtCorps", 8, 0);
    mMsater(ApptId, "ApptId", 1, 0);
    //if (regtCorps) {
    //    $('#regtCorps').prop('disabled', true); // Disable the select dropdown
    //}

    //if (UnitId) {
    //    $.ajax({
    //        url: '/Account/GetUnitById',
    //        type: 'POST',
    //        data: { UnitId: UnitId },
    //        success: function (data) {
    //            if (data) {
    //                $('#txtUnit').val(data);
    //                $('#txtUnit').prop('readonly', true); // Make input readonly
    //            }
    //        },
    //        error: function (response) {
    //            alert(response.responseText);
    //        }
    //    });
    //}
 
}

$('#DteFmn').on('change', function () {
    if ($(this).is(':checked')) {
        Swal.fire({
            title: 'DTE/FMN Selected',
            text: 'You have checked the DTE/FMN option.',
            icon: 'success',
            confirmButtonText: 'OK'
        });
    }
});

$("#btnsignup").on("click", function (e) {
    e.preventDefault(); // Prevent form submission for now

    // Show the confirmation SweetAlert
    Swal.fire({
        title: 'Do you really want to save?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, save it!',
        cancelButtonText: 'No, cancel!',
    }).then((result) => {
        if (result.isConfirmed) {
            // If the user clicked 'Yes', submit the form
            // Assuming the form has an id of #signupForm
            $("#signupForm").submit();  // Or trigger your form submit action here
        } else {
            // If the user clicked 'No', do nothing or show an alert
            Swal.fire('Cancelled', 'Your Details was not saved.', 'info');
        }
    });
});


$("#txtUnit").autocomplete({
    source: function (request, response) {
        //alert(1);
        $("input[name='UnitId']").val(0);

        if (request.term.length > 2) {
            var param = { "UnitName": request.term };
            $("#UnitId").val(0);
            $.ajax({
                url: '/Account/GetALLByUnitName',
                contentType: 'application/x-www-form-urlencoded',
                data: param,
                type: 'POST',
                success: function (data) {
                    if (data.length != 0) {
                        response($.map(data, function (item) {

                            return { label: item.pcda_Pao + ' ' + item.name, value: item.id };

                        }))
                    }
                    else {
                        $("#UnitId").val(0);
                        $("#txtUnit").val("");

                        showErrorMessage("Unit Not found.")
                    }

                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        }
    },
    select: function (e, i) {
        e.preventDefault();
        $("#txtUnit").val(i.item.label);
        $("#UnitId").val(i.item.value);
        // $("#spnUnitMapId").html(i.item.value);
        //alert(i.item.value)

    },
    appendTo: '#suggesstion-box'
});

$("#btnTokenDetails").on('click', function () {

    GetTokenDetails("ArmyNo", "Name", "errormsg", "btnsignup")
});

