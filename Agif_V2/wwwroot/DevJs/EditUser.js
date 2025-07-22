$(document).ready(function () {
    populateDropdowns();

    function populateDropdowns() {
        const Rank = $('#rank').data('rank-prefix');
        const regtCorps = $('#regtCorps').data('regtcorps-prefix');
        const ApptId = $('#ApptId').data('apptid-prefix');
        const UnitId = $('#UnitId').val();

        mMsater(regtCorps, "regtCorps", 8, 0);
        mMsater(Rank, "rank", 3, 0);
        mMsater(ApptId, "ApptId", 1, 0);

        if (UnitId) {
            $.ajax({
                url: '/Account/GetUnitById',
                type: 'POST',
                data: { UnitId: UnitId },
                success: function (data) {
                    if (data) {
                        $('#txtUnit').val(data);
                        $('#txtUnit').prop('readonly', true); // Make input readonly
                    }
                },
                error: function (response) {
                    alert(response.responseText);
                }
            });
        }
    }

    $('#editButton').click(function () {
        enableEditMode();
    });

     function enableEditMode() {                   

            // Keep other fields disabled false
         $('#name').prop('disabled', false);       
         $('#EmailId').prop('disabled', false);    
         $('#MobileNo').prop('disabled', false);
         $('#ApptId').prop('disabled', false);   
         $('#rank').prop('disabled', false);       
         $('#DteFmn').prop('disabled', false);       

            // Update page title
            $('#page-title').html('Edit User Profile');
            
            // Show/hide buttons
         $('#editButton').hide();

         //$('#btnsignup').removeClass('d-none');

         $('#btnTokenDetails').show();

        }

    $("#btnsignup").on("click", function (e) {
        e.preventDefault(); // Prevent form submission for now
        const form = document.getElementById("editUserForm");
        if (form.checkValidity()) {
            Swal.fire({
                title: 'Do you really want to Update?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes, Update it!',
                cancelButtonText: 'No, cancel!',
            }).then((result) => {
                if (result.isConfirmed) {
                    // If the user clicked 'Yes', submit the form
                    // Assuming the form has an id of #signupForm
                    $("#editUserForm").submit();  // Or trigger your form submit action here
                } else {
                    // If the user clicked 'No', do nothing or show an alert
                    Swal.fire('Cancelled', 'Your Details was not Update.', 'info');
                }
            });
        } else {
            form.reportValidity(); 
        }
        
    });

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


    $("#btnTokenDetails").on('click', function () {

        GetTokenDetails("ArmyNo", "name", "errormsg", "btnsignup")
    });
});