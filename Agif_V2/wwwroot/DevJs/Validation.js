function ValInDataAddress(input) {
    var regex = /[^a-zA-Z0-9 ()/ ]/g;
    input.value = input.value.replace(regex, "");
}
function ValInData(input) {
    var regex = /[^a-zA-Z0-9 ]/g;
    input.value = input.value.replace(regex, "");
}
function ValInDataLetter(input) {
    var regex = /[^a-zA-Z ]/g;
    input.value = input.value.replace(regex, "");
}
function ValInDataNo(input) {
    var regex = /[^0-9]/g;
    input.value = input.value.replace(regex, "");
}
function valInDataRupee(input) {
    var regex = /[^0-9,]/g;
    input.value = input.value.replace(regex, "");
}
function ValInAadhar(input) {
    var regex = /[^0-9-]/g;
    input.value = input.value.replace(regex, "");
}
function verifyMobileNo(input) {
    var inputValue = input.value.trim();
    if (inputValue.length !== 10 || !/^\d{10}$/.test(inputValue)) {
       // alert("Please enter a valid 10-digit mobile number");
        setTimeout(function () {
            input.focus();
        }, 0);
    }
}
function validateUnitPin(input) {

    var inputValue = input.value.trim();
    if (inputValue.length !== 6 || !/^\d{6}$/.test(inputValue)) {
        alert("Please enter a valid 6-digit PIN code");
        setTimeout(function () { input.focus(); }, 100);

    }
}
function validateAccountNo(input) {

    var inputValue = input.value.trim();
    if (inputValue.length < 11) {
        setTimeout(function () { input.focus(); }, 100);

    }
}
function validateDateFormat(input) {
    debugger;
    const value = input.value;
    const datePattern = /^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/(\d{4})$/;
    if (value && !datePattern.test(value)) {
        Swal.fire({
            icon: "error",
            title: "Invalid date",
            text: "Invalid date format. Please select a valid date.",
        });
        input.focus();
        input.value = "";
    }
}

$('.form-space').on("keypress", function (e) {
    // Get the key code of the pressed key
    var keyCode = e.which;

    // Allow only alphabets (A-Z, a-z) and numbers (0-9)
    if (keyCode == 32) {
        showErrorMessage('Only Alphabets and Numbers allowed');
        return false; // Block the keypress
    } else {
        return true; // Allow the keypress
    }
});
$('.form-control-Alphabets').on("keypress", function (e) {

    // Get the key code of the pressed key
    var keyCode = e.which;

    // Allow only alphabets (A-Z, a-z) and numbers (0-9)
    if ((keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122) || (keyCode == 32)) {
        return true; // Allow the keypress
    } else {
        showErrorMessage('Only Alphabets allowed');
        return false; // Block the keypress
    }
});
$('.form-control').on("keypress", function (e) {

    // Get the key code of the pressed key
    var keyCode = e.which;
   
    // Allow only alphabets (A-Z, a-z) and numbers (0-9)
    if ((keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122) || (keyCode >= 48 && keyCode <= 57) || (keyCode == 32)) {
        return true; // Allow the keypress
    } else {
        showErrorMessage('Only Alphabets and Numbers allowed');
        return false; // Block the keypress
    }
});

$('.form-email').on("keypress", function (e) {
    var keyCode = e.which;

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

$('.form-control-domainId').on("keypress", function (e) {
    // Get the key code of the pressed key
    var keyCode = e.which;

    // Allow only alphabets (A-Z, a-z) and numbers (0-9)
    if ((keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122) || (keyCode >= 48 && keyCode <= 57) || (keyCode == 32) || keyCode == 95) {
        return true; // Allow the keypress
    } else {
        showErrorMessage('Only Alphabets, Underscore and Numbers allowed');
        return false; // Block the keypress
    }
});
$('.Alphanumeric').on('change', function () {

    if ($('.Alphanumeric').val().match("^[a-zA-Z0-9 ]*$")) {
        return true

    }
    else {
        showErrorMessage('Only Alphabets and Numbers allowed');

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

function showErrorMessage(message) {
    // Using Bootstrap alert (if you have Bootstrap)
    const alertHtml = `
        <div class="alert alert-danger alert-dismissible fade show" role="alert" style="position: fixed; top: 20px; right: 20px; z-index: 9999; min-width: 300px;">
            <i class="lni lni-cross-circle"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    $('body').append(alertHtml);

    // Auto remove after 5 seconds
    setTimeout(function () {
        $('.alert-danger').fadeOut(300, function () {
            $(this).remove();
        });
    }, 2000);
}