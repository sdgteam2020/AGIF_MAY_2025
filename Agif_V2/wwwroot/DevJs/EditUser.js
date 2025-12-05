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

    $('#editButton').on('click',function () {
        enableEditMode();
    });
    $('#UnitId').on('onkeyup', function () {
        ValInData(this);
    })

     function enableEditMode() {                   

            // Keep other fields disabled false
         $('#name').prop('disabled', false);       
         $('#EmailId').prop('disabled', false);    
         $('#MobileNo').prop('disabled', false);
         $('#ApptId').prop('disabled', false);   
         $('#rank').prop('disabled', false);       
         $('#DteFmn').prop('disabled', false);       
         $('#regtCorps').prop('disabled', false);       

            // Update page title
            $('#page-title').html('Edit User Profile');
            
            // Show/hide buttons
         $('#editButton').hide();
         $("#btnTokenDetails").removeClass("d-none");
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

    $('.form-control-Alphabets').on("keypress", function (e) {

        // Get the key code of the pressed key
        const keyCode = e.which;

        // Allow only alphabets (A-Z, a-z) and numbers (0-9)
        if ((keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122) || (keyCode == 32)) {
            return true; // Allow the keypress
        } else {
            showErrorMessage('Only Alphabets allowed');
            return false; // Block the keypress
        }
    });

    $('.isNumerictxt').on("keypress", function (e) {

        if (isNumeric(e.key)) {

            return true;

        }
        else {
            $(this).val($(this).val().replace(e.key, ""));
            showErrorMessage('Only Numbers allowed');
            return false;

        }
    });

    function isNumeric(key) {
        // Allow only digits 0–9
        return /^[0-9]$/.test(key);
    }

    $('.form-email').on("keypress", function (e) {
        const keyCode = e.which;

        // Allow only alphabets (A-Z, a-z) and numbers (0-9)
        if ((keyCode >= 65 && keyCode <= 90) ||  // A-Z
            (keyCode >= 97 && keyCode <= 122) ||  // a-z
            (keyCode >= 48 && keyCode <= 57) ||   // 0-9
            (keyCode == 64) ||                    // '@' symbol (keyCode 64)
            (keyCode == 46) ||                    // '.' symbol (keyCode 46)
            (keyCode == 95)) {                   // '_' symbol (keyCode 95)
            return true;
        } else {
            showErrorMessage('Only Alphabets, Numbers, @, . and _ are allowed');
            return false; // Block the keypress
        }
    });
});

$("input, textarea").on("paste", function (e) {
    e.preventDefault();
});
$(document).ready(function () {
    const message = $('#profileMessage').val();

    if (message) {
        Swal.fire({
            html: `<span class="msg-title">${message}</span>`,
            icon: 'success',
            confirmButtonText: 'OK'
        });
    }
});