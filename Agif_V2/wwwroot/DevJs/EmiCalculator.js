function calculateEMI() {
    const loanAmount = parseFloat(document.getElementById('loanAmount').value.replace(/[^0-9.]/g, ''));
    const annualRate = parseFloat(document.getElementById('interestRate').value);
    const maxEmi = parseInt(document.getElementById('maxEmi').value);

    if (!loanAmount || !annualRate || !maxEmi) {
        alert('Please fill in all required fields');
        return;
    }
    
    const monthlyRate = (annualRate / 100) / 12;
    
    const emi = (loanAmount * monthlyRate * Math.pow(1 + monthlyRate, maxEmi)) /
        (Math.pow(1 + monthlyRate, maxEmi) - 1);

    // Display result
    const emiResult = document.getElementById('emiResult');
    const emiAmount = document.getElementById('emiAmount');

    emiAmount.textContent = '₹ ' + emi.toLocaleString('en-IN', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });

    emiResult.style.display = 'block';
    emiResult.scrollIntoView({ behavior: 'smooth' });
}


// Allow Enter key to calculate EMI
document.addEventListener('keypress', function (e) {
    if (e.key === 'Enter') {
        calculateEMI();
    }
});

function formatIndianNumber(input) {

    let num = input.value.replace(/[^0-9]/g, '');

    if (num === "") {
        input.value = "";
        return;
    }

    let parsedInteger = parseInt(num, 10);
    if (isNaN(parsedInteger)) {
        input.value = "";
        return;
    }

    let integerPart = parsedInteger.toString();

    let lastThree = integerPart.slice(-3);
    let otherNumbers = integerPart.slice(0, -3);

    if (otherNumbers !== '') {
        otherNumbers = otherNumbers.replace(/\B(?=(\d{2})+(?!\d))/g, ",");
    }

    input.value = (otherNumbers ? otherNumbers + "," : "") + lastThree;
}