$(document).ready(function () {

    $('#loanAmount, #interestRate, #maxEmi').on('input', function () {
        CheckEmiCalculater();
    });


});
function CheckEmiCalculater() {

    const loanAmount = parseFloat($("#loanAmount").val().replace(/,/g, '')); // Remove commas and convert to number
    const interestRate = parseFloat($("#interestRate").val()); // Convert to number
    const maxEmi = parseFloat($("#maxEmi").val()); // Convert to number

    if (loanAmount > 0 && interestRate > 0 && maxEmi > 0) {
        calculateEMI(loanAmount, interestRate, maxEmi);
    }
    else {
        $('#emiAmount').text("");
        $('#emiResult').addClass('d-none');  // hide the result
    }
}

function calculateEMI(loanAmount, annualRate, maxEmi) {
   
    if (!loanAmount || !annualRate || !maxEmi) {
        alert('Please fill in all required fields');
        return;
    }

    const monthlyRate = (annualRate / 100) / 12;

    const emi = (loanAmount * monthlyRate * Math.pow(1 + monthlyRate, maxEmi)) /
        (Math.pow(1 + monthlyRate, maxEmi) - 1);

    // Round the EMI to 2 decimal places
    const roundedEmi = emi.toFixed(2);

    // Display result using jQuery
    $('#emiAmount').text(Number(roundedEmi).toLocaleString('en-IN', {
        style: 'currency',
        currency: 'INR',
        maximumFractionDigits: 2
    }));

    $('#emiResult').removeClass('d-none');  // Show the result
    $('html, body').animate({
        scrollTop: $('#emiResult').offset().top  // Scroll to the result
    }, 1000);  // Smooth scrolling with a duration of 1 second

}



function formatIndianNumber(input) {

    let value = input.value.replace(/,/g, ''); // Remove any existing commas
    let formattedValue = parseFloat(value).toLocaleString('en-IN', {
        maximumFractionDigits: 2,
    });
    input.value = formattedValue;
}