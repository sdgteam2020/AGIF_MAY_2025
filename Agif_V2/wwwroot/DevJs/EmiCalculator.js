$(document).ready(function () {

    $('#loanAmount, #interestRate, #maxEmi').on('input', function () {
        CheckEmiCalculater();
    });

    checkEMIMonths();
    checkInterestRate(); // New function for Interest Rate validation

    $('#loanAmount').on('input', function () {
        formatIndianNumber(this);
    });

    $('#btnCalculate').on('click', function () {
        CheckEmiCalculater();
    });

});

// 1. Max EMI Validation
function checkEMIMonths() {
    $('#maxEmi').on('input', function () {
        let inputValue = parseInt($(this).val(), 10);

        // Allow user to clear the input, but restrict max to 360 (30 years)
        if ($(this).val() === '') {
            return;
        }

        if (inputValue > 360) {
            $(this).val(360); // Set to max 360 to match your HTML
        } else if (inputValue < 1 || isNaN(inputValue)) {
            $(this).val(''); // Clear invalid input
        }
    });
}

// 2. Interest Rate Validation (New)
function checkInterestRate() {
    $('#interestRate').on('input', function () {
        let val = parseFloat($(this).val());

        if ($(this).val() === '') return;

        // Cap interest rate at 100%
        if (val > 100) {
            $(this).val(100);
        } else if (val < 0) {
            $(this).val(0);
        }

        // Prevent user from typing more than 2 decimal places
        let strVal = $(this).val();
        if (strVal.includes('.')) {
            let parts = strVal.split('.');
            if (parts[1].length > 2) {
                $(this).val(parseFloat(strVal).toFixed(2));
            }
        }
    });
}

// 3. Loan Amount Validation
function formatIndianNumber(input) {
    // Remove commas and non-numeric characters (except decimals)
    let value = input.value.replace(/,/g, '').replace(/[^\d.]/g, '');

    if (isNaN(value) || value === '') {
        input.value = '';
        return;
    }

    // Cap the loan amount at ₹100 Crores (1,00,00,00,000) to prevent overflow
    let numericValue = parseFloat(value);
    if (numericValue > 1000000000) {
        numericValue = 1000000000;
    }

    input.value = numericValue.toLocaleString('en-IN', {
        maximumFractionDigits: 2,
    });
}

// EMI calculation logic remains the same
function CheckEmiCalculater() {
    const loanAmount = parseFloat($("#loanAmount").val().replace(/,/g, ''));
    const interestRate = parseFloat($("#interestRate").val());
    const maxEmi = parseFloat($("#maxEmi").val());

    if (!isNaN(loanAmount) && !isNaN(interestRate) && !isNaN(maxEmi) &&
        loanAmount > 0 && interestRate > 0 && maxEmi > 0) {
        calculateEMI(loanAmount, interestRate, maxEmi);
    } else {
        $('#emiAmount').text("");
        $('#emiResult').addClass('d-none');
    }
}

function calculateEMI(loanAmount, annualRate, maxEmi) {
    const monthlyRate = (annualRate / 100) / 12;

    const emi = (loanAmount * monthlyRate * Math.pow(1 + monthlyRate, maxEmi)) /
        (Math.pow(1 + monthlyRate, maxEmi) - 1);

    const roundedEmi = emi.toFixed(2);

    $('#emiAmount').text(Number(roundedEmi).toLocaleString('en-IN', {
        style: 'currency',
        currency: 'INR',
        maximumFractionDigits: 2
    }));

    $('#emiResult').removeClass('d-none');

    // Note: Scroll animation is optional. If it's annoying while typing, 
    // you might want to move this inside the #btnCalculate click event instead.
    /*
    $('html, body').animate({
        scrollTop: $('#emiResult').offset().top 
    }, 1000);  
    */
}