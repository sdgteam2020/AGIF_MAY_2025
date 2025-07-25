
$(document).ready(function () {
    expandAccordions();
    confirmAccountNo();
    loadDropdown();
    enableDisablePromotionDate();
    Inputmask().mask(document.querySelectorAll("input"));
    EnableDisablePCDA();
    EnableDisableCivilPostalAdd();
    handleSubmitClick();
    ExtensionOfServiceAccess();
    resetCivilPostalAddress();
    resetFieldsOnRankRegtChange();
});

function resetCivilPostalAddress() {
    $('#armyPostOffice').on('change', function () {
        $('#civilPostalAddress').val("");
    });
}


function resetFieldsOnRankRegtChange() {
    $('#ddlrank, #regtCorps,#armyPrefix').on('change', function () {
        $('#dateOfPromotion,#dateOfRetirement, #dateOfBirth,#dateOfCommission, #totalService, #residualService, #totalResidualMonth, #HBA_EMI_Eligible,#HBA_Amt_Eligible_for_loan,#HBA_Amount_Applied_For_Loan,#HBA_EMI_Applied,#HBA_approxEMIAmount,#HBA_approxDisbursementAmt,#propertyCost,#CA_Amt_Eligible_for_loan,#CA_EMI_Eligible,#CA_Amount_Applied_For_Loan,#CA_EMI_Applied,#CA_approxEMIAmount,#CA_approxDisbursementAmt,#vehicleCost,#PCA_Amt_Eligible_for_loan,#PCA_EMI_Eligible,#PCA_Amount_Applied_For_Loan,#PCA_EMI_Applied,#PCA_approxEMIAmount,#PCA_approxDisbursementAmt,#computerCost').val('');
    });
}

$('#loanType').change(function () {
    showLoanTypeFields($(this).val());
});

$('#loanFormContainer').on('input change', 'input, select', function () {
    checkFieldsForLoan();
});
function showLoanForm() {
    $('#addLoanSection').hide();
    $('#loanFormContainer').show();
    resetLoanForm();
}

function removeLoanForm() {
    $('#addLoanSection').show();
    $('#loanFormContainer').hide();
    resetLoanForm();
}

function resetLoanForm() {
    $('#loanType').val('');
    $('.loan-type-section').hide();
    $('#addLoanButton').hide().prop('disabled', true);
}


function isLoanTypeAlreadyAdded(loanType) {
    let isAlreadyAdded = false;

    $('#loanGrid tbody tr').each(function () {
        const rowLoanType = $(this).attr('data-loan-type');
        if (rowLoanType === loanType) {
            isAlreadyAdded = true;
            return false; // Break the loop
        }
    });

    return isAlreadyAdded;
}

// Updated showLoanTypeFields to disable already added loan types
function showLoanTypeFields(loanType) {
    // Hide all sections first
    $('.loan-type-section').hide();

    if (loanType) {
        // Check if this loan type is already added
        if (isLoanTypeAlreadyAdded(loanType)) {
            const loanTypeNames = {
                'hba': 'House Building Advance (HBA)',
                'hra': 'House Repair Advance (HRA)',
                'ca': 'Computer Advance (CA)',
                'pca': 'Personal Computer Advance (PCA)'
            };

            alert(`${loanTypeNames[loanType]} has already been added. Only one entry per loan type is allowed.`);
            $('#loanType').val(''); // Reset selection
            $('#addLoanButton').hide();
            return;
        }

        $('#' + loanType + 'Fields').show();
        $('#addLoanButton').show();
        checkFieldsForLoan();
    } else {
        $('#addLoanButton').hide();
    }
}

function checkFieldsForLoan() {
    const loanType = $('#loanType').val();
    if (!loanType) {
        $('#addLoanButton').prop('disabled', true);
        return;
    }

    let allFieldsFilled = true;
    const requiredFields = [
        loanType + 'Date',
        loanType + 'Duration',
        loanType + 'Amount'
    ];

    requiredFields.forEach(fieldId => {
        if (!$('#' + fieldId).val().trim()) {
            allFieldsFilled = false;
        }
    });

    $('#addLoanButton').prop('disabled', !allFieldsFilled);
}
function removeLoanRow(button) {
    $(button).closest('tr').remove();

    if ($('#loanGrid tbody tr').length === 0) {
        $('#loanGridContainer').hide();
    }

    // Update dropdown options after removing a loan
    updateLoanTypeDropdown();
}
function addLoanToGrid() {
    const loanType = $('#loanType').val();
    
        if (isLoanTypeAlreadyAdded(loanType)) {
            const loanTypeNames = {
                'hba': 'House Building Advance (HBA)',
                'hra': 'House Repair Advance (HRA)',
                'ca': 'Computer Advance (CA)',
                'pca': 'Personal Computer Advance (PCA)'
            };

            showErrorMessage(`${loanTypeNames[loanType]} has already been added. Only one entry per loan type is allowed.`);
            return;
        }

        // Store values before clearing
        const loanDate = $('#' + loanType + 'Date').val();
        const loanDuration = $('#' + loanType + 'Duration').val();
        const loanAmount = $('#' + loanType + 'Amount').val();

        // Validate fields
        if (!validateLoanData(loanType, loanDate, loanDuration, loanAmount)) {
            return;
        }

        // Get loan type display name
        const loanTypeNames = {
            'hba': 'HBA (House Building Advance)',
            'hra': 'HRA (House Repair Advance)',
            'ca': 'CA (Computer Advance)',
            'pca': 'PCA (Personal Computer Advance)'
        };

        // Add to grid
        const newRow = `
        <tr data-loan-type="${loanType}">
            <td>${loanTypeNames[loanType]}</td>
            <td>${formatDateofloan(loanDate)}</td>
            <td>${loanDuration} months</td>
            <td>₹${formatAmount(loanAmount)}</td>
            <td><button type="button" class="btn btn-danger btn-sm" onclick="removeLoanRow(this)">Remove</button></td>
        </tr>
    `;

        $('#loanGrid tbody').append(newRow);
        $('#loanGridContainer, #loanGrid').show();

        // Update dropdown to disable the added loan type
        updateLoanTypeDropdown();

        // Reset form
        resetLoanForm();
    
}

function validateLoanData(loanType, date, duration, amount) {
    // Date validation
    const loanDate = new Date(date);
    const currentDate = new Date();
    const minDate = new Date('1990-01-01');

    if (loanDate > currentDate) {
        showErrorMessage('Loan date cannot be in the future.');
        return false;
    }
    // Amount validation
    const amountNum = parseFloat(amount);
    if (isNaN(amountNum) || amountNum <= 0 || amountNum > 10000000) {
        showErrorMessage('Please enter a valid amount (₹1 - ₹1,00,00,000).');
        return false;
    }

    return true;
}

function formatDateofloan(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-IN');
}

function formatAmount(amount) {
    return parseFloat(amount).toLocaleString('en-IN');
}

// Function to update loan type dropdown options based on already added loans
function updateLoanTypeDropdown() {
    const addedLoanTypes = [];

    // Get all added loan types using data-loan-type attribute for accuracy
    $('#loanGrid tbody tr').each(function () {
        const loanTypeAttr = $(this).attr('data-loan-type');
        if (loanTypeAttr) {
            addedLoanTypes.push(loanTypeAttr);
        }
    });

    // Store original option texts to avoid multiple "(Already Added)" appends
    const originalTexts = {
        'hba': 'HBA (House Building Advance)',
        'hra': 'HRA (House Repair Advance)',
        'ca': 'CA (Computer Advance)',
        'pca': 'PCA (Personal Computer Advance)'
    };

    // Update dropdown options
    $('#loanType option').each(function () {
        const optionValue = $(this).val();

        if (optionValue && originalTexts[optionValue]) {
            if (addedLoanTypes.includes(optionValue)) {
                // Disable and mark as already added
                $(this).prop('disabled', true).text(originalTexts[optionValue] + ' (Already Added)');
                $('#' + optionValue + '_Loan').val(true);
            } else {
                // Enable and restore original text
                $(this).prop('disabled', false).text(originalTexts[optionValue]);
                $('#' + optionValue + '_Loan').val(false);
            }
        }
    });
}

function expandAccordions() {

    //$('#dateOfRetirement').val("2044-10-12");
    let $toggleButton = $('#toggleAll');

    $toggleButton.on('click', function () {
        let isExpanding = $toggleButton.text().trim() === 'Expand All';

        $('.accordion-collapse').each(function () {
            const collapse = bootstrap.Collapse.getOrCreateInstance(this);
            isExpanding ? collapse.show() : collapse.hide();
        });
    });
    function updateToggleButtonText() {
        const total = $('.accordion-collapse').length;
        const open = $('.accordion-collapse.show').length;

        if (total === open) {
            $toggleButton.text('Collapse All');
        } else {
            $toggleButton.text('Expand All');
        }
    }

    $('.accordion-collapse').on('shown.bs.collapse hidden.bs.collapse', function () {
        updateToggleButtonText();
    });
    updateToggleButtonText();
}
function formatAadhar(input) {
    let value = input.value.replace(/\D/g, '');
    value = value.substring(0, 12);
    let formattedValue = '';
    for (let i = 0; i < value.length; i++) {
        if (i > 0 && i % 4 === 0) {
            formattedValue += '-';
        }
        formattedValue += value[i];
    }
    input.value = formattedValue;
}
function loadDropdown() {
    const params = new URLSearchParams(window.location.search);

    const loanTypeFromUrl = params.get("Category");
    const loanTypeFromInput = document.getElementById('Category')?.value || null;
    const loanType = loanTypeFromUrl ? loanTypeFromUrl : loanTypeFromInput;

    const applicantCategoryFromUrl = params.get("PurposeOfWithdrwal");
    const applicantCategoryFromInput = document.getElementById('Purpose')?.value || null;
    const applicantCategory = applicantCategoryFromUrl ? applicantCategoryFromUrl : applicantCategoryFromInput;

    const armyPrefixValue = $('#armyPrefix').data('army-prefix');
    const OldArmyPrefixvalue = $('#oldArmyPrefix').data('oldarmy-prefix');
    const Rank = $('#ddlrank').data('rank-prefix');
    const regtcorps = $('#regtCorps').data('regt-prefix');
    const parentunit = $('#parentUnit').data('parent-prefix');
    const presentunit = $('#presentUnit').data('present-prefix');
    const Armypostoffice = $('#armyPostOffice').data('armypost-prefix');

    if (loanType == 1) {
        mMsater(armyPrefixValue, "armyPrefix", 9, 0);
        mMsater(OldArmyPrefixvalue, "oldArmyPrefix", 9, 0);
        mMsater(Rank, "ddlrank", 3, 0);
    }
    else if (loanType == 2) {
        mMsater(armyPrefixValue, "armyPrefix", 10, 0);
        mMsater(OldArmyPrefixvalue, "oldArmyPrefix", 10, 0);
        mMsater(Rank, "ddlrank", 4, 0);
    }
    else if (loanType == 3) {
        mMsater(armyPrefixValue, "armyPrefix", 11, 0);
        mMsater(OldArmyPrefixvalue, "oldArmyPrefix", 11, 0);
        mMsater(Rank, "ddlrank", 13, 0);
    }
    

    mMsater(regtcorps, "regtCorps", 8, 0);
    mMsater(parentunit, "parentUnit", 2, 0);
    mMsater(presentunit, "presentUnit", 2, 0);
    mMsater(Armypostoffice, "armyPostOffice", 14, 0);
    mMsater(OldArmyPrefixvalue, "oldArmyPrefix", 7, 0);
}

function confirmAccountNo() {
    $('#confirmSalaryAcctNo').change(function () {
        const accountNo = $('#salaryAcctNo').val();
        const reEnterAccountNo = $('#confirmSalaryAcctNo').val();

        if (accountNo !== reEnterAccountNo) {
            $('#confirmSalaryAcctNo').val('').css('border', '2px solid red');

            const $this = $(this); // store jQuery reference to use in arrow functions

            setTimeout(() => {
                $this.focus();
            }, 10);

            setTimeout(() => {
                $this.css('border', '');
            }, 2000);
        }
    });
}

function SetSuffixLetter(obj) {
    const fixed = "98765432";
    let ArmyNumber = $(obj).val();
    let arr1 = ArmyNumber.split('');
    const arr2 = fixed.split('');

    const len = arr2.length - arr1.length;
    if (len === 1) {
        ArmyNumber = "0" + ArmyNumber;
    } else if (len === 2) {
        ArmyNumber = "00" + ArmyNumber;
    } else if (len === 3) {
        ArmyNumber = "000" + ArmyNumber;
    } else if (len === 4) {
        ArmyNumber = "0000" + ArmyNumber;
    } else if (len === 5) {
        ArmyNumber = "00000" + ArmyNumber;
    } else if (len === 6) {
        ArmyNumber = "000000" + ArmyNumber;
    } else if (len === 7) {
        ArmyNumber = "0000000" + ArmyNumber;
    }

    arr1 = ArmyNumber.split('');
    let total = 0;
    for (let i = 0; i < arr1.length; i++) {
        const val1 = arr1[i];
        const val2 = arr2[i];
        total += parseInt(val1) * parseInt(val2);
    }

    const rem = total % 11;
    let Sletter = '';
    switch (rem.toString()) {
        case '0':
            Sletter = 'A'; break;
        case '1':
            Sletter = 'F'; break;
        case '2':
            Sletter = 'H'; break;
        case '3':
            Sletter = 'K'; break;
        case '4':
            Sletter = 'L'; break;
        case '5':
            Sletter = 'M'; break;
        case '6':
            Sletter = 'N'; break;
        case '7':
            Sletter = 'P'; break;
        case '8':
            Sletter = 'W'; break;
        case '9':
            Sletter = 'X'; break;
        case '10':
            Sletter = 'Y'; break;
    }

    const sourceId = $(obj).attr('id');
    let targetSuffixId;

    if (sourceId === 'armyNumber') {
        targetSuffixId = 'txtSuffix';
    } else if (sourceId === 'oldArmyNo') {
        targetSuffixId = 'txtOldSuffix';
    }

    $("#" + targetSuffixId).val(Sletter);
    setOutlineActive(targetSuffixId);
}

function getApplicantDetalis() {
    const armyNumber = $("#armyPrefix").val();
    const Prefix = $("#armyNumber").val();
    const Suffix = $("#txtSuffix").val();
    const appType = parseInt($("#loanType").val(), 10);

    $.ajax({
        type: "get",
        url: "/Claim/CheckExistUser",
        data: { armyNumber: armyNumber, Prefix: Prefix, Suffix: Suffix, appType: appType },
        success: function (data) {
            if (data.exists) {
                Swal.fire({
                    title: "You Have Already applied for Loan.",
                    text: "Would you like to apply for a new loan?",
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#3085d6",
                    cancelButtonColor: "#d33",
                    confirmButtonText: "Yes"
                }).then((result) => {
                    if (result.isConfirmed) {
                        DeleteConfirmation();
                    }
                });
            }
        },
        error: function () {
            alert("Data not loaded!");
        }
    });
}


function DeleteConfirmation() {
    Swal.fire({
        title: "Previous Loan data deleted !",
        text: "your previous Loan data will be deleted permanently !",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes"
    }).then((result) => {
        if (result.isConfirmed) {
            DeleteExistingLoan();
        }
    });
}

function DeleteExistingLoan() {
    const armyNumber = $("#armyPrefix").val();
    const Prefix = $("#armyNumber").val();
    const Suffix = $("#txtSuffix").val();
    const appType = parseInt($("#loanType").val(), 10);
    $.ajax({
        type: "get",
        url: "/Claim/DeleteExistingLoan",
        data: { armyNumber: armyNumber, Prefix: Prefix, Suffix: Suffix, appType: appType },
        success: function (data) {
            if (data.exists) {
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Deleted! Please Apply Again!",
                    showConfirmButton: false,
                    timer: 3000
                });
            }
        },
        error: function () {
            alert("Data Not loaded!")
        }
    });
}

function calculateDifferenceBetweenDOBAndDOC(doc) {
    const dob = $('#dateOfBirth').val();
    if (!dob) {
        alert("Please select a Date of Birth.");
        return;
    }
    const dateOfBirth = new Date(my_date(dob));
    const dateOfCommission = new Date(my_date(doc));
    if (dateOfCommission < dateOfBirth) {
        alert("Date of Commission cannot be earlier than Date of Birth.");
        return;
    }
    const ageInMilliseconds = dateOfCommission - dateOfBirth;
    const ageInYears = Math.floor(ageInMilliseconds / (1000 * 60 * 60 * 24 * 365.25)); // Account for leap years
    if (ageInYears < 15) {
        Swal.fire({
            title: 'Warning!',
            text: 'Atleast 15 years of age is required for commission. Please check the Date of Birth and Date of Commission.',
            icon: 'warning',
            confirmButtonText: 'OK'
        }).then((result) => {
            if (result.isConfirmed) {
                $('#dateOfCommission').val("");
                window.location.href = '/Claim/MaturityLoanType';
            }
        });
    }


}
function calculateYearDifference() {
    const value = $('#dateOfCommission').val();

    if (!value) {
        alert("Please select a Date of Commission.");
        return;
    }
    calculateDifferenceBetweenDOBAndDOC(value);
    const commissionDate = new Date(my_date(value));
    const today = new Date();

    let years = today.getFullYear() - commissionDate.getFullYear();

    const hasNotHadAnniversaryThisYear =
        today.getMonth() < commissionDate.getMonth() ||
        (today.getMonth() === commissionDate.getMonth() && today.getDate() < commissionDate.getDate());

    if (hasNotHadAnniversaryThisYear) {
        years--;
    }
    $('#totalService').val(years);
    setOutlineActive("totalService");
    return years;
}
const globleRetirementDate = {};


$('.monthPicker').datepicker({
    changeMonth: true,
    changeYear: true,
    showButtonPanel: true,
    dateFormat: 'dd/mm/yy',
    maxDate: 0, // Restrict to today and past dates only
    yearRange: "1900:+0", // Years from 1900 to current year
    defaultDate: null,

    onSelect: function (dateText, inst) {
        // Get the selected date from the input field
        const dt = $(this).val();

        // Call custom functions
        formatDate(this);
        validateDateFormat(this);

        // Calculate new date by adding 18 years
        const newdt = new Date(my_date(dt));
        newdt.setFullYear(newdt.getFullYear() + 18);

        // Optional: Use `newdt` as needed
    },

    beforeShowDay: function (date) {
        // Disable future dates
        const today = new Date();
        today.setHours(23, 59, 59, 999);
        return [date <= today];
    }
});


$('.DocPicker').datepicker({
    changeMonth: true,
    changeYear: true,
    showButtonPanel: true,
    dateFormat: 'dd/mm/yy',
    maxDate: 0, // This restricts to today and past dates only (no future dates)
    yearRange: "1900:+0", // Allow years from 1900 to current year only
    defaultDate: null, // Default to today
 

    onSelect: function (dateText, inst) {
        // Get the selected date from the input field
        const dt = $('#dateOfCommission').val();

        // Call the custom functions on select
        formatDate(this); // Ensure the function formats the date
        validateDateFormat(this); // Ensure validation is handled
        calculateYearDifference(); // Calculate the year difference (if needed)

        // Calculate new date by adding 18 years
        const newdt = new Date(my_date(dt));
        newdt.setFullYear(newdt.getFullYear() + 18);
        

    }
    
});
$('.DopPicker').datepicker({
    changeMonth: true,
    changeYear: true,
    showButtonPanel: true,
    dateFormat: 'dd/mm/yy',
    yearRange: "1900:2100", // Allow years from 1900 to current year only
    defaultDate: null, // Default to today
   
    onSelect: function (dateText, inst) {
        // Get the selected date from the input field
        const dt = $('#dateOfPromotion').val();

        // Call the custom functions on select
        formatDate(this); // Ensure the function formats the date
        updateRetDateOnPromotionDateSelection(); // Calculate the year difference (if needed)

        // Calculate new date by adding 18 years
        const newdt = new Date(my_date(dt));
        newdt.setFullYear(newdt.getFullYear() + 18);

        // Optional: Set the calculated date back to the field or use as needed

    }
   
});
$('.DOPartIIPicker').datepicker({
    changeMonth: true,
    changeYear: true,
    showButtonPanel: true,
    dateFormat: 'dd/mm/yy',
    yearRange: "1900:2100", // Allow years from 1900 to current year only
    defaultDate: null, // Default to today

    onSelect: function (dateText, inst) {
        // Get the selected date from the input field
        const dt = $('this').val();

        // Call the custom functions on select
        formatDate(this); // Ensure the function formats the date

        // Calculate new date by adding 18 years
        const newdt = new Date(my_date(dt));
        newdt.setFullYear(newdt.getFullYear() + 18);

        // Optional: Set the calculated date back to the field or use as needed

    }

});

function formatDate(input) {
    // Get the current value and remove any non-numeric characters except existing slashes
    let value = input.value.replace(/[^\d]/g, '');

    // Store cursor position
    let cursorPosition = input.selectionStart;
    let oldLength = input.value.length;

    // Format the value with slashes
    if (value.length >= 2) {
        value = value.substring(0, 2) + '/' + value.substring(2);
    }
    if (value.length >= 5) {
        value = value.substring(0, 5) + '/' + value.substring(5);
    }

    // Limit to 10 characters (dd/mm/yyyy)
    if (value.length > 10) {
        value = value.substring(0, 10);
    }

    // Update the input value
    input.value = value;

    // Adjust cursor position
    let newLength = input.value.length;
    if (newLength > oldLength) {
        cursorPosition++;
    }

    // Set cursor position
    input.setSelectionRange(cursorPosition, cursorPosition);
}

function formatDateToString(date) {
    const day = ("0" + date.getDate()).slice(-2);
    const month = ("0" + (date.getMonth() + 1)).slice(-2);
    const year = date.getFullYear();
    return day + "/" + month + "/" + year;
}

function validateDateFormat(input) {
    const value = input.value;
    const datePattern = /^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/(\d{4})$/;

    // Check if the value matches the date format
    if (value && !datePattern.test(value)) {
        Swal.fire({
            icon: "error",
            title: "Invalid date",
            text: "Invalid date format. Please select a valid date.",
        });
        input.focus();
        input.value = ""; // Clear the invalid input
        return;
    }

    // Additional validation to check date validity and reasonable year range
    if (value && datePattern.test(value)) {
        const parts = value.split('/');
        const day = parseInt(parts[0], 10);
        const month = parseInt(parts[1], 10) - 1; // Month is 0-indexed
        const year = parseInt(parts[2], 10);

        // Check for reasonable year range (e.g., 1900 to current year)
        const currentYear = new Date().getFullYear();
        if (year < 1900 || year > currentYear) {
            Swal.fire({
                icon: "error",
                title: "Invalid year",
                text: `Please enter a year between 1900 and ${currentYear}.`,
            });
            input.focus();
            input.value = ""; // Clear the invalid input
            return;
        }

        // Check if it's a valid date

    }

    SetRetDate();
}

function my_date(date_string) {
    const date_components = date_string.split("/");
    const day = date_components[0];
    const month = date_components[1];
    const year = date_components[2];
    return new Date(year, month - 1, day);
}
function SetRetDate() {
    const Prefix = $('#armyPrefix').val();
    const ranks = $('#ddlrank').val();
    const rankId = parseInt(ranks);
    const EnrollDate = $('#dateOfCommission').val();
    const regtCorps = $('#regtCorps').val();
    const regtId = parseInt(regtCorps);

    if (!regtCorps) {
        Swal.fire({
            title: 'Warning!',
            html: '<p style="font-size: 18px;">Regt/Corps value is empty.Please enter a valid regiment.</p>',
            confirmButtonText: 'OK',
            width: '500px'
        });
        $('#dateOfBirth').val('');
        return;
    }
    if (!rankId) {
        Swal.fire({
            title: 'Warning!',
            html: '<p style="font-size: 18px;">Rank value is empty.Please enter a valid rank.</p>',
            confirmButtonText: 'OK',
            width: '500px'
        });
        $('#dateOfBirth').val('');
        return;
    }
    if (Prefix == "0" || Prefix == "") {
        Swal.fire({
            title: 'Warning!',
            html: '<p style="font-size: 18px;">Please select Prefix.</p>',
            confirmButtonText: 'OK',
            width: '500px'
        });
        $('#dateOfBirth').val('');
        return;
    }

    const dateOfBirthString = $('#dateOfBirth').val();
    const dateParts = dateOfBirthString.split('/');
    if (dateParts.length === 3) {
        if (!EnrollDate || !dateOfBirthString) {
            console.log('EnrollDate or dateOfBirthString is empty or undefined.')
        } else {
            $.ajax({
                type: "get",
                url: "/OnlineApplication/GetRetirementDate",
                data: { rankId: rankId, Prefix: Prefix, regtId: regtId },
                success: function (data) {
                    if (data.userTypeId == 1) {
                        const dateParts = $('#dateOfBirth').val().split('/');
                        if (data != 0 && dateParts.length == 3) {
                            const [day, month, year] = dateParts;
                            const dob = new Date(year, month - 1, day);
                            dob.setFullYear(dob.getFullYear() + data.retirementAge);
                            const yyyy = dob.getFullYear();
                            const mm = String(dob.getMonth() + 1).padStart(2, '0');
                            const dd = String(dob.getDate()).padStart(2, '0');
                            const formattedDate = `${yyyy}-${mm}-${dd}`;
                            $('#dateOfRetirement').val(formattedDate);
                            setOutlineActive("dateOfRetirement");
                            globleRetirementDate.value = formattedDate;
                            calculateResidualService();
                        } else {
                            $('#dateOfRetirement').val('');
                            console.warn("Invalid retirement age or date of birth.");
                        }
                    } else if (data.userTypeId == 2) {
                        const dateParts = $('#dateOfCommission').val().split('/');
                        if (data != 0 && dateParts.length == 3) {
                            const [day, month, year] = dateParts;
                            const dob = new Date(year, month - 1, day);
                            dob.setFullYear(dob.getFullYear() + 10);
                            const yyyy = dob.getFullYear();
                            const mm = String(dob.getMonth() + 1).padStart(2, '0');
                            const dd = String(dob.getDate()).padStart(2, '0');
                            const formattedDate = `${yyyy}-${mm}-${dd}`;
                            $('#dateOfRetirement').val(formattedDate);
                            setOutlineActive("dateOfRetirement");
                            globleRetirementDate.value = formattedDate;
                            calculateResidualService();
                        } else {
                            $('#dateOfRetirement').val('');
                            console.warn("Invalid retirement age or date of birth.");
                        }
                    } else if (data.userTypeId == 3 || data.userTypeId == 4) {
                        const rankType = $('#ddlrank').val();
                        if (rankType == 0) {
                            alert("Please select Rank Type.");
                        }

                        if (rankType == 31 || rankType == 32 || rankType == 33) {
                            const dateParts = $('#dateOfBirth').val().split('/');
                            if (data != 0 && dateParts.length == 3) {
                                const [day, month, year] = dateParts;
                                const dob = new Date(year, month - 1, day);
                                dob.setFullYear(dob.getFullYear() + data.retirementAge);
                                const yyyy = dob.getFullYear();
                                const mm = String(dob.getMonth() + 1).padStart(2, '0');
                                const dd = String(dob.getDate()).padStart(2, '0');
                                const formattedDate = `${yyyy}-${mm}-${dd}`;
                                $('#dateOfRetirement').val(formattedDate);
                                setOutlineActive("dateOfRetirement");
                                globleRetirementDate.value = formattedDate;
                                calculateResidualService();
                                ExtensionOfServiceAccess();
                            } else {
                                $('#dateOfRetirement').val('');
                                console.warn("Invalid retirement age or date of birth.");
                            }
                        } else {
                            const dateParts = $('#dateOfCommission').val().split('/');
                            if (data != 0 && dateParts.length == 3) {
                                const [day, month, year] = dateParts;
                                const dob = new Date(year, month - 1, day);
                                dob.setFullYear(dob.getFullYear() + data.retirementAge);
                                const yyyy = dob.getFullYear();
                                const mm = String(dob.getMonth() + 1).padStart(2, '0');
                                const dd = String(dob.getDate()).padStart(2, '0');
                                const formattedDate = `${yyyy}-${mm}-${dd}`;
                                $('#dateOfRetirement').val(formattedDate);
                                setOutlineActive("dateOfRetirement");
                                globleRetirementDate.value = formattedDate;
                                calculateResidualService();
                                ExtensionOfServiceAccess();
                            } else {
                                $('#dateOfRetirement').val('');
                                console.warn("Invalid retirement age or date of birth.");
                            }
                        }
                    }
                },
                error: function () {
                    alert("Data Not loaded!")
                }
            });
        }
    } else {
        console.error('Invalid date string.');
    }
}

function calculateResidualService() {
    var retirementDateStr = $('#dateOfRetirement').val(); // Expected format: 'YYYY-MM-DD'
    var purposetype = $('#Purpose').val();

    if (!retirementDateStr) {
        return;
    }

    var retirementDate = new Date(retirementDateStr);
    var currentDate = new Date();

    // Normalize both dates to remove time differences
    retirementDate.setHours(0, 0, 0, 0);
    currentDate.setHours(0, 0, 0, 0);

    if (retirementDate < currentDate) {
        return;
    }

    var years = retirementDate.getFullYear() - currentDate.getFullYear();
    var months = retirementDate.getMonth() - currentDate.getMonth();
    var days = retirementDate.getDate() - currentDate.getDate();

    if (days < 0) {
        months -= 1;
        var prevMonth = new Date(retirementDate.getFullYear(), retirementDate.getMonth(), 0);
        days += prevMonth.getDate(); // Add days of the previous month
    }

    if (months < 0) {
        years -= 1;
        months += 12;
    }
    var totalmonths = years * 12 + months;
    $("#totalResidualMonth").val(totalmonths);
    $("#residualService").val(years);

    if (purposetype == "3") {

        if (years > 2) {
            Swal.fire({
                title: 'Residual Service Calculated',
                text: 'Your residual service is not valid',
                icon: 'info',
                confirmButtonText: 'OK'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Redirect to claim/online application page
                    window.location.href = '/Claim/MaturityLoanType';  // Adjust URL if necessary
                }
            });
        }
        // Show SweetAlert and redirect on "OK"
      
    }
        
    
    setOutlineActive("residualService");

}
function enableDisablePromotionDate() {
    $('#ddlrank').on('change', function () {
        togglePromotionDate($(this).val());
    });

    // Run once on load
    togglePromotionDate($('#ddlrank').val());
}

function togglePromotionDate(rankValue) {
    if (rankValue === '31' || rankValue === '1') {
        $('#dateOfPromotion').prop('disabled', false).addClass('bg-white text-dark');
    } else {
        $('#dateOfPromotion').prop('disabled', true).removeClass('bg-white text-dark');
    }
}
function updateRetDateOnPromotionDateSelection() {
    var promotionDate = $('#dateOfPromotion').val();
    if (!promotionDate) {
        alert("Please select the Date of Promotion.");
        return;
    }
    var dateParts = promotionDate.split('/');
    if (dateParts.length === 3) {
        var year = dateParts[2];
        var month = dateParts[1] - 1;
        var day = dateParts[0];

        var dob = new Date(year, month, day);
        dob.setFullYear(dob.getFullYear() + 4);
        var yyyy = dob.getFullYear();
        var mm = String(dob.getMonth() + 1).padStart(2, '0');
        var dd = String(dob.getDate()).padStart(2, '0');
        var formattedDate = `${yyyy}-${mm}-${dd}`;
        $('#dateOfRetirement').val(formattedDate);
        calculateResidualService();
    } else {
        console.error('Invalid date string.');
    }
}
function extensionOfService() {
    const prefix = $('#armyPrefix').val();
    const extension = $('#ExtnOfService').val();

    if (!prefix) {
        alert("Please select Prefix.");
        return;
    }

    if (prefix == 13 || prefix == 14) {
        if (extension === "Yes") {
            const currentRetDate = $('#dateOfRetirement').val();
            const currentResidualService = parseInt($('#residualService').val());

            const dateParts = currentRetDate.split('-');
            if (dateParts.length === 3) {
                const year = parseInt(dateParts[0]);
                const month = parseInt(dateParts[1]) - 1;
                const day = parseInt(dateParts[2]);

                const dateOfRetirement = new Date(year, month, day);
                dateOfRetirement.setFullYear(dateOfRetirement.getFullYear() + 2);

                const yyyy = dateOfRetirement.getFullYear();
                const mm = String(dateOfRetirement.getMonth() + 1).padStart(2, '0');
                const dd = String(dateOfRetirement.getDate()).padStart(2, '0');
                const formattedDate = `${yyyy}-${mm}-${dd}`;

                $('#dateOfRetirement').val(formattedDate);
                calculateResidualService();
            } else {
                $('#dateOfRetirement').val('');
                console.warn("Invalid retirement age or date of birth.");
            }
        } else {
            $('#dateOfRetirement').val(globleRetirementDate.value);
            calculateResidualService();
        }
    }
}

function ExtensionOfServiceAccess() {
    const prefix = $('#armyPrefix').val();
    const yearOfService = parseFloat($('#residualService').val());
    const extensionDropdown = $('#ExtnOfService');
    // Enable only if Year of Service < 2 and Prefix is JC or OR
    if ((prefix == 13 || prefix == 14) && yearOfService < 2 && yearOfService >= 0) {
        extensionDropdown.prop('disabled', false);
    } else {
        extensionDropdown.prop('disabled', true);
        extensionDropdown.val('');
    }
}
function fetchPCDA_PAO() {
    const regt = $('#regtCorps').val();
    if (!regt) {
        alert("Please select Regt/Corps.");
        return;
    }
    fetch(`/OnlineApplication/GetPCDA_PAO?regt=${encodeURIComponent(regt)}`, {
        method: 'GET'
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json(); // assuming the server returns JSON
        })
        .then(data => {
            if (data != null) {
                $('#pcda_pao').val(data.pcdaPao);
                setOutlineActive("pcda_pao");
            }
        })
        .catch(error => {
            alert("Data Not loaded!");
            console.error('Fetch error:', error);
        });


}
function setOutlineActive(id) {
    $("#" + id).closest(".form-outline").addClass("active");
    if (typeof mdb !== 'undefined') {
        $("#" + id).closest(".form-outline").each(function () {
            new mdb.Input(this).init();
        });
    }
}
function EnableDisablePCDA() {
    $("#armyPrefix").on("change", function () {
        const Prefix = $('#armyPrefix').val();

        if (Prefix == "13" || Prefix == "14") {
            $('#pcda_AcctNo').val("00/000/000000A");
            $('#pcda_AcctNo').prop('readonly', true);
        } else {
            $('#pcda_AcctNo').prop('readonly', false);
        }
    });
}
function EnableDisableCivilPostalAdd() {
    $("#armyPostOffice").on("change", function () {
        const apo = $('#armyPostOffice').val();

        if (apo == "3") {
            $('#civilPostalAddress')
                .prop('readonly', false)
                .removeClass('readonly-blocked');
        } else {
            $('#civilPostalAddress')
                .prop('readonly', true)
                .addClass('readonly-blocked');
        }
    });
}
let formSubmitting = false;
let formCancelled = false;
function filterAmountText(loanType) {
    if (loanType == 2) {
        const VehicleCost = $('#vehicleCost');
        const cleanedValue = VehicleCost.val().replace(/,/g, '');
        VehicleCost.val(cleanedValue);
        //alert(cleanedValue);
    }
}

function handleSubmitClick() {
    document.getElementById("btn-save").addEventListener("click", function (event) {
        event.preventDefault(); // Prevent form submission
        const form = document.getElementById("myMaturityForm");
        const inputs = form.querySelectorAll("input, select");
        // Clear previous error messages
        form.querySelectorAll(".error").forEach(span => span.textContent = "");

        // let errorlist = "";
        let errorlist = []; // Use an array to store individual error messages
        let hasError = false;

        const params = new URLSearchParams(window.location.search);

        const loanTypeFromUrl = params.get("loanType");

        const loanTypeFromInput = document.getElementById('loanType')?.value || null;

        const loanType = loanTypeFromUrl ? loanTypeFromUrl : loanTypeFromInput;
        filterAmountText(loanType);




        inputs.forEach(input => {

            if (loanType === "1" && (document.getElementById("pcaAccordianWrapper")?.contains(input) || document.getElementById("caAccordianWrapper")?.contains(input))) {
                return;
            }
            else if (loanType === "2" && (document.getElementById("pcaAccordianWrapper")?.contains(input) || document.getElementById("hbaAccordianWrapper")?.contains(input))) {
                return;
            }
            else if (loanType === "3" && (document.getElementById("caAccordianWrapper")?.contains(input) || document.getElementById("hbaAccordianWrapper")?.contains(input))) {
                return;
            }

            if (!input.checkValidity()) {
                const errorSpan = input.parentElement.querySelector(".error");
                if (errorSpan) {
                    errorSpan.textContent = input.validationMessage;
                }
                // errorlist += input.name + ", ";
                let errorText = input.name;
                const prefixes = ["ClaimCommonData.", "Marriageward.", "EducationDetails.", "PropertyRenovation.","SplWaiver."];
                prefixes.forEach(prefix => {
                    if (errorText.includes(prefix)) {
                        errorText = errorText.replace(prefix, "");
                    }
                });
                errorlist.push(errorText);
                hasError = true;
                if (input.value.trim() !== "") {
                    input.removeAttribute("required");
                }

            }
        });

        

        var errors = hasError ? "Error in: " + errorlist.join(", ") : "";
        $("#msgerror").html('<div class="alert alert-danger" role="alert">⚠️' + errors + ' </div>')

        if (hasError) {
            return false;
        }
        else {
            //form.submit();
            $("#msgerror").html(''); // Clear error message
            if (formSubmitting) return; // Allow submission after confirmation
            if (formCancelled) {
                formCancelled = false; // Reset flag
                e.preventDefault();
                return;
            }

            let unitVal = $('#PresenttxtUnit').val().trim();
            if (unitVal != '') {
                event.preventDefault(); // Stop form submission
                checkCORegistration(); // First check CO registration
            }
        }
        

    });

}

function checkCORegistration() {
    // Get Prefix, Number, and Suffix
    var armyNumber = $("#armyPrefix option:selected").text();
    var Prefix = $("#armyNumber").val();
    var Suffix = $("#txtSuffix").val();

    const ArmyNo = `${armyNumber}${Prefix}${Suffix}`.toUpperCase();

    const unitValidation = document.querySelector("span[data-valmsg-for='Unit']");

    if (Prefix === "0" || armyNumber === "" || Suffix === "") {
        // Warn if Army No is incomplete
        console.warn("Incomplete Army No");
        return;
    }

    try {
        $.ajax({
            url: '/OnlineApplication/CheckForCoRegister',
            type: 'POST',
            data: { ArmyNo: ArmyNo },
            success: function (result) {
                if (result === true) {
                    $('#unitSearchDialog').show();
                } else if (result === false) {
                    // If not registered, set unit input back to required
                    formSubmitting = true;
                    $('#myMaturityForm').submit();
                }
            },
            error: function () {
                console.error("Failed to check CO registration");
            }
        });
    } catch (err) {
        console.error("AJAX error", err);
    }
}

$('#unitSearchInput').on('input', function () {
    const inputValue = $(this).val().trim();
    $('#unitSearchConfirmBtn').prop('disabled', inputValue === '');
});

$("#unitSearchConfirmBtn").click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    var value = $("#unitSearchInput").val()

    if (value != 0) {
        Swal.fire({
            title: "Are you sure?",
            text: "Do You want to Submit!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, Submit it!"
        }).then((result) => {
            if (result.isConfirmed) {
                checkUnitSameOrNot(value)
            }
        });
    }
    else {
        alert("Please select unit");
    }
});

$("#unitSearchCancelBtn").click(function (e) {

    e.preventDefault();
    e.stopPropagation();

    $('#unitSearchDialog').hide();

    formSubmitting = false;
    formCancelled = true;

    // Hide the message when Cancel is clicked
    const $message = $('#unitSearchMessage');
    if ($message.length) {
        $message.text('');
    }

    $(unitSearchInput).val('');

    // Also clear any previous search results and reset form stat

    $('#unitSearchConfirmBtn').prop('disabled', true);
});


function checkUnitSameOrNot(ArmyNo) {
    var armyNumber = $("#armyPrefix option:selected").text();
    var Prefix = $("#armyNumber").val();
    var Suffix = $("#txtSuffix").val();

    var Value = armyNumber + Prefix + Suffix;
    if (ArmyNo == Value.toUpperCase()) {
        //console.log("Unit is same as Army No");
        $('#unitSearchMessage').text("Army Number Already Registered.\nYou are already registered as CO for this unit. Please select another Army Number.");
    }

    else {
        try {
            $.ajax({
                url: '/OnlineApplication/CheckForCoRegister',
                type: 'POST',
                data: { ArmyNo: ArmyNo },
                success: function (result) {
                    if (result === true) {
                        $('#COArmyNo').val(ArmyNo);
                        formSubmitting = true;
                        $('#myMaturityForm').submit();
                    } else if (result === false) {
                        // If not registered, set unit input back to required
                        $('#unitSearchMessage').text();
                        formSubmitting = false;
                        //$('#myMaturityForm').submit();
                        Swal.fire({
                            icon: 'info',
                            title: '<span style="font-size: 20px;">Unit Registration Pending/Not Activated</span>',
                            html: '<span style="font-size: 18px;">Please approach your Unit IO to register/contact to Agif.</span>',
                            confirmButtonText: 'OK',
                            cancelButtonText: 'Cancel', // Cancel button text
                            showCancelButton: true, // Enable cancel button
                            reverseButtons: true,  // Make Cancel button appear on the left
                        }).then((result) => {
                            if (result.isConfirmed) {
                                $('#unitSearchDialog').hide();
                            } else if (result.isDismissed) {
                                $("unitSearchDialog").show();
                            }
                        });
                    }
                },
                error: function () {
                    console.error("Failed to check CO registration");
                }
            });
        } catch (err) {
            console.error("AJAX error", err);
        }
    }


}
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


$("#ParenttxtUnit").autocomplete({
    source: function (request, response) {
        //alert(1);
        $("input[name='ParentUnit']").val(0);

        if (request.term.length > 2) {
            var param = { "UnitName": request.term };
            $("#ParentUnitId").val(0);
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
                        $("#ParentUnitId").val(0);
                        $("#ParenttxtUnit").val("");

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
        $("#ParenttxtUnit").val(i.item.label);
        $("#ParentUnitId").val(i.item.value);
        $("input[name='ClaimCommonData.ParentUnit']").val(i.item.value);
        // $("#spnUnitMapId").html(i.item.value);
        //alert(i.item.value)

    },
    appendTo: '#suggesstion-box'
});

$("#PresenttxtUnit").autocomplete({
    source: function (request, response) {
        //alert(1);
        $("input[name='PresentUnit']").val(0);

        if (request.term.length > 2) {
            var param = { "UnitName": request.term };
            $("#ParentUnitId").val(0);
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
                        $("#PresentUnitId").val(0);
                        $("#PresenttxtUnit").val("");

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
        $("#PresenttxtUnit").val(i.item.label);
        $("#PresentUnitId").val(i.item.value);
        $("input[name='ClaimCommonData.PresentUnit']").val(i.item.value);
        

    },
    appendTo: '#suggesstion-box'
});

$('#oldArmyNo').on('focus', function () {
    $(this).off('focus');
    Swal.fire({
        title: "Enter Present 'Army No'",
        text: "If old 'Army No' is not applicable for you.",
        icon: "warning",
        confirmButtonText: "OK"
    });
});

$("#OtherReasonPdf").on("click", function () {
    // Trigger SweetAlert with two messages
    Swal.fire({
        title: 'Important Information',
        html: '<p>Application Dully Recommended by Commanding Officer/IO</p><p>Otherwise DO Letter Of Commanding Officer</p>', // Two paragraphs
        icon: 'info',
        confirmButtonText: 'Okay',
          customClass: {
            popup: 'swal-popup-custom'  // Custom class for the popup
        }
    });
});

$('#OtherReasons').on('input', function () {
    var maxWords = 50;
    var currentValue = $(this).val();

    // Split the value into words by whitespace
    var words = currentValue.trim().split(/\s+/);

    // If the number of words exceeds the limit, truncate the string
    if (words.length > maxWords) {
        words = words.slice(0, maxWords); // Take only the first 50 words
        $(this).val(words.join(' ')); // Join them back into a string and update the input
    }
});
