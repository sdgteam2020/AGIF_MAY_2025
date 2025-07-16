$(document).ready(function () {

    $('#loanAmount, #interestRate, #maxEmi').on('input', function () {
        CheckEmiCalculater();
    });
    checkEMIMonths() 


});

function checkEMIMonths() {
    $('#maxEmi').on('input', function () {
        let inputValue = parseInt($(this).val(), 10);

        if (inputValue > 240) {
            $(this).val(240); // Set to max of 240
        } else if (inputValue < 1 || isNaN(inputValue)) {
            $(this).val(''); // Clear invalid input
        }
    });
}


function CheckEmiCalculater() {

    const loanAmount = parseFloat($("#loanAmount").val().replace(/,/g, '')); // Remove commas and convert to number
    const interestRate = parseFloat($("#interestRate").val()); // Convert to number
    const maxEmi = parseFloat($("#maxEmi").val()); // Convert to number

    if (!isNaN(loanAmount) && !isNaN(interestRate) && !isNaN(maxEmi) &&
        loanAmount > 0 && interestRate > 0 && maxEmi > 0) {
        calculateEMI(loanAmount, interestRate, maxEmi);
    }

    else {
        $('#emiAmount').text("");
        $('#emiResult').addClass('d-none');  // hide the result
    }
}

function calculateEMI(loanAmount, annualRate, maxEmi) {
    const monthlyRate = (annualRate / 100) / 12;

    const emi = (loanAmount * monthlyRate * Math.pow(1 + monthlyRate, maxEmi)) /
        (Math.pow(1 + monthlyRate, maxEmi) - 1);
        
    const roundedEmi = emi.toFixed(2);

    // Display result using jQuery
    $('#emiAmount').text(Number(roundedEmi).toLocaleString('en-IN', {
        style: 'currency',
        currency: 'INR',
        maximumFractionDigits: 2
    }));

    $('#emiResult').removeClass('d-none');  
    $('html, body').animate({
        scrollTop: $('#emiResult').offset().top 
    }, 1000);  

}



function formatIndianNumber(input) {
    let value = input.value.replace(/,/g, '');
    if (isNaN(value) || value === '') {
        input.value = '';
        return;
    }
    input.value = parseFloat(value).toLocaleString('en-IN', {
        maximumFractionDigits: 2,
    });
}