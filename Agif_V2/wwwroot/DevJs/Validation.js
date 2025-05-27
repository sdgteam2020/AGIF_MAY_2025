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
        alert("Please enter a valid 10-digit mobile number");
        input.focus();
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
        alert("Please enter a valid Account No");
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