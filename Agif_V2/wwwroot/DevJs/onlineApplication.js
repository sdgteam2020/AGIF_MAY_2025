
$(document).ready(function () {
    expandAccordions();
    confirmAccountNo();
    loadDropdown();
    enableDisablePromotionDate();
    Inputmask().mask(document.querySelectorAll("input"));
    EnableDisablePCDA();
    EnableDisableCivilPostalAdd();
    RefreshMaxAmt_PCA();
    RefreshMaxAmt_CA();
    RefreshMaxAmt_HBA();
    handleSubmitClick();
    ExtensionOfServiceAccess();
    resetCivilPostalAddress();
    resetFieldsOnRankRegtChange();
    bindMinAmountValidation();
    resetFieldsOnChange();
    filterPCALoanFreqOptions();
    filterHBALoanFreqOptions();
    filterCALoanFreqOptions();
});
function filterPCALoanFreqOptions() {
    $('#computer_Loan_Type').on('change', function () {
        const $dropdown = $('#PCA_LoanFreq');

        $dropdown.find('option').each(function () {
            const val = $(this).val();
            if (val != '1' && val != '2' && val !== '') {
                $(this).remove(); // Instead of hide
            }
        });


    });
}

function filterHBALoanFreqOptions() {
    $('#propertyType').on('change', function () {
        const $dropdown = $('#HBA_LoanFreq');
        const propType = $('#propertyType').val();

        $dropdown.find('option').prop('disabled', false);

        if (propType == 5) {
            $dropdown.find('option').each(function () {
                const val = $(this).val();
                if (val != '1' && val !== '') {
                    $(this).prop('disabled', true);
                }
            });
        } else {
            $dropdown.find('option').each(function () {
                const val = $(this).val();
                if (val != '1' && val != '2' && val !== '') {
                    $(this).prop('disabled', true);
                }
            });
        }
    });
}

function filterCALoanFreqOptions() {
    $('#veh_Loan_Type').on('change', function () {
        const $dropdown = $('#CA_LoanFreq');
        const propType = $('#veh_Loan_Type').val();

        $dropdown.find('option').prop('disabled', false);

        if (propType == 4) {
            $dropdown.find('option').each(function () {
                const val = $(this).val();
                if (val != '1' && val != '2' && val != '3' && val !== '') {
                    $(this).prop('disabled', true);
                }
            });
        } else {
            $dropdown.find('option').each(function () {
                const val = $(this).val();
                if (val != '1' && val != '2' && val !== '') {
                    $(this).prop('disabled', true);
                }
            });
        }
    });
}

resetFieldsOnChange('#propertyCost,#vehicleCost,#computerCost', [
    '#HBA_Amount_Applied_For_Loan', '#HBA_EMI_Applied', '#CA_Amount_Applied_For_Loan',
    '#CA_EMI_Applied', '#PCA_Amount_Applied_For_Loan',
    '#PCA_EMI_Applied'
]);

function resetFieldsOnChange(dropdownSelector, fieldSelectors) {
    $(dropdownSelector).on('change', function () {
        $(fieldSelectors.join(',')).val('');
    });
}
resetFieldsOnChange('#HBA_LoanFreq,#PCA_LoanFreq,#CA_LoanFreq', [
    '#HBA_EMI_Eligible', '#CA_EMI_Eligible', '#PCA_EMI_Eligible', '#vehicleCost', '#propertyCost', '#computerCost'
]);

// Usage
resetFieldsOnChange('#veh_Loan_Type,#VehTypeId', [
    '#CA_LoanFreq', '#vehicleCost', '#CA_Amt_Eligible_for_loan',
    '#CA_EMI_Eligible', '#CA_repayingCapacity', '#CA_Amount_Applied_For_Loan',
    '#CA_EMI_Applied', '#CA_approxEMIAmount', '#CA_approxDisbursementAmt'
]);

resetFieldsOnChange('#propertyType', [
    '#propertyCost', '#HBA_LoanFreq', '#HBA_repayingCapacity',
    '#HBA_Amt_Eligible_for_loan', '#HBA_EMI_Eligible', '#HBA_Amount_Applied_For_Loan',
    '#HBA_EMI_Applied', '#HBA_approxEMIAmount', '#HBA_approxDisbursementAmt'
]);

resetFieldsOnChange('#dateOfBirth,#dateOfCommission', [
    '#CA_LoanFreq', '#vehicleCost', '#CA_Amt_Eligible_for_loan',
    '#CA_EMI_Eligible', '#CA_repayingCapacity', '#CA_Amount_Applied_For_Loan',
    '#CA_EMI_Applied', '#CA_approxEMIAmount', '#CA_approxDisbursementAmt',
    '#propertyCost', '#HBA_LoanFreq', '#HBA_repayingCapacity',
    '#HBA_Amt_Eligible_for_loan', '#HBA_EMI_Eligible', '#HBA_Amount_Applied_For_Loan',
    '#HBA_EMI_Applied', '#HBA_approxEMIAmount', '#HBA_approxDisbursementAmt',
    '#PCA_LoanFreq', '#computerCost', '#PCA_Amt_Eligible_for_loan',
    '#PCA_EMI_Eligible', '#PCA_repayingCapacity', '#PCA_Amount_Applied_For_Loan',
    '#PCA_EMI_Applied', '#PCA_approxEMIAmount', '#PCA_approxDisbursementAmt'
]);

resetFieldsOnChange('#dateOfBirth', [
    '#dateOfCommission'
]);
// Bind once when DOM is ready


function bindMinAmountValidation() {
    const selectors = [
        '#HBA_Amount_Applied_For_Loan',
        '#PCA_Amount_Applied_For_Loan',
        '#CA_Amount_Applied_For_Loan'
    ];

    selectors.forEach(selector => {
        $(selector).on('change', function () {
            const rawValue = $(this).val();
            const numericValue = parseFloat(rawValue.replace(/,/g, '')) || 0;
            const vehType = $('#veh_Loan_Type').val();

            let minAmount = 0;

            if (selector === '#HBA_Amount_Applied_For_Loan') {
                minAmount = 500000;
            } else if (selector === '#PCA_Amount_Applied_For_Loan') {
                minAmount = 25000;
            } else if (selector === '#CA_Amount_Applied_For_Loan') {
                minAmount = (vehType == 4) ? 50000 : 200000;
            }

            if (numericValue < minAmount) {
                Swal.fire({
                    title: 'Warning!',
                    text: `Minimum amount is ₹${minAmount.toLocaleString('en-IN')}. Please enter a valid amount.`,
                    icon: 'warning',
                    confirmButtonText: 'OK'
                });

                $(this).val('');
                $(this).focus();
            }
        });
    });
}


function resetFieldsOnRankRegtChange() {
    $('#ddlrank, #regtCorps,#armyPrefix').on('change', function () {
        $('#dateOfPromotion,#dateOfRetirement, #dateOfBirth,#dateOfCommission, #totalService, #residualService, #totalResidualMonth, #HBA_EMI_Eligible,#HBA_Amt_Eligible_for_loan,#HBA_Amount_Applied_For_Loan,#HBA_EMI_Applied,#HBA_approxEMIAmount,#HBA_approxDisbursementAmt,#propertyCost,#CA_Amt_Eligible_for_loan,#CA_EMI_Eligible,#CA_Amount_Applied_For_Loan,#CA_EMI_Applied,#CA_approxEMIAmount,#CA_approxDisbursementAmt,#vehicleCost,#PCA_Amt_Eligible_for_loan,#PCA_EMI_Eligible,#PCA_Amount_Applied_For_Loan,#PCA_EMI_Applied,#PCA_approxEMIAmount,#PCA_approxDisbursementAmt,#computerCost').val('');
    });
}
function resetCivilPostalAddress() {
    $('#armyPostOffice').on('change', function () {
        $('#civilPostalAddress').val("");
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

    const loanTypeFromUrl = params.get("loanType");

    const loanTypeFromInput = document.getElementById('loanType')?.value || null;

    const loanType = loanTypeFromUrl ? loanTypeFromUrl : loanTypeFromInput;

    //const applicantCategory = params.get("applicantCategory");

    const applicantCategoryFromUrl = params.get("applicantCategory");

    const applicantCategoryFromInput = document.getElementById('applicantCategory')?.value || null;

    const applicantCategory = applicantCategoryFromUrl ? applicantCategoryFromUrl : applicantCategoryFromInput;

    var armyPrefixValue = $('#armyPrefix').data('army-prefix');  // Access the data-attribute value
    var OldArmyPrefixvalue = $('#oldArmyPrefix').data('oldarmy-prefix');
    var Rank = $('#ddlrank').data('rank-prefix');
    var regtcorps = $('#regtCorps').data('regt-prefix');
    var parentunit = $('#parentUnit').data('parent-prefix');
    var presentunit = $('#presentUnit').data('present-prefix');
    var Armypostoffice = $('#armyPostOffice').data('armypost-prefix');
    var propertytype = $('#propertyType').data('propertytype-prefix');
    var hbaloanfreq = $('#HBA_LoanFreq').data('hbaloanfreq-prefix');
    var vehicletype = $('#veh_Loan_Type').data('vehicletype-prefix');
    var Cavehicleloanfreq = $('#CA_LoanFreq').data('vehicleloanfreq-prefix');
    var computer_Loan_Type = $('#computer_Loan_Type').data('pcaloantype-prefix');
    var Pcaloanfreq = $('#PCA_LoanFreq').data('pcaloanfreq-prefix');
    var VehType = $('#VehTypeId').data('vehicletypeId-prefix');

    if (applicantCategory == 1) {
        mMsater(armyPrefixValue, "armyPrefix", 9, 0);
        mMsater(Rank, "ddlrank", 3, 0);
    }
    else if (applicantCategory == 2) {
        mMsater(armyPrefixValue, "armyPrefix", 10, 0);
        mMsater(Rank, "ddlrank", 4, 0);
    }
    else if (applicantCategory == 3) {
        mMsater(armyPrefixValue, "armyPrefix", 11, 0);
        mMsater(Rank, "ddlrank", 13, 0);
    }
    if (loanType == 1) {
        mMsater(propertytype, "propertyType", 16, 0);
    }
    else if (loanType == 2) {
        mMsater(vehicletype, "veh_Loan_Type", 17, 0);
    }
    else if (loanType == 3) {
        mMsater(computer_Loan_Type, "computer_Loan_Type", 18, 0);
    }
    mMsater(regtcorps, "regtCorps", 8, 0);
    mMsater(parentunit, "parentUnit", 2, 0);
    mMsater(presentunit, "presentUnit", 2, 0);
    mMsater(Armypostoffice, "armyPostOffice", 14, 0);
    mMsater(Cavehicleloanfreq, "CA_LoanFreq", 15, 0);
    mMsater(Pcaloanfreq, "PCA_LoanFreq", 15, 0);
    mMsater(hbaloanfreq, "HBA_LoanFreq", 15, 0);
    mMsater(OldArmyPrefixvalue, "oldArmyPrefix", 7, 0);
    mMsater(VehType, "VehTypeId", 20, 0);
}
function confirmAccountNo() {
    $('#confirmSalaryAcctNo').change(function () {
        var accountNo = $('#salaryAcctNo').val();
        var reEnterAccountNo = $('#confirmSalaryAcctNo').val();
        if (accountNo !== reEnterAccountNo) {
            Swal.fire({
                title: "Not Matched",
                text: "Salary Account No. not matched",
                icon: "warning"
            }).then(() => {

                $('#confirmSalaryAcctNo').val("");
                $('#confirmSalaryAcctNo').focus();
            });
        }

    });
}
function SetSuffixLetter(obj) {
    var arr1 = [], arr2 = [];
    var ArmyNumber = $(obj).val();
    var fixed = "98765432";
    arr1 = ArmyNumber.split('');
    arr2 = fixed.split('');
    var len = arr2.length - arr1.length;
    if (len == 1) {
        ArmyNumber = "0" + ArmyNumber;
    }
    if (len == 2) {
        ArmyNumber = "00" + ArmyNumber;
    }
    if (len == 3) {
        ArmyNumber = "000" + ArmyNumber;
    }
    if (len == 4) {
        ArmyNumber = "0000" + ArmyNumber;
    }
    if (len == 5) {
        ArmyNumber = "00000" + ArmyNumber;
    }
    if (len == 6) {
        ArmyNumber = "000000" + ArmyNumber;
    }
    if (len == 7) {
        ArmyNumber = "0000000" + ArmyNumber;
    }

    var total = 0;
    arr1 = ArmyNumber.split('');
    for (var i = 0; i < arr1.length; i++) {
        var val1 = arr1[i];
        var val2 = arr2[i];
        total += parseInt(val1) * parseInt(val2);
    }
    var rem = total % 11;
    var Sletter = '';
    switch (rem.toString()) {
        case '0':
            Sletter = 'A'
            break;
        case '1':
            Sletter = 'F'
            break;
        case '2':
            Sletter = 'H'
            break;
        case '3':
            Sletter = 'K'
            break;
        case '4':
            Sletter = 'L'
            break;
        case '5':
            Sletter = 'M'
            break;
        case '6':
            Sletter = 'N'
            break;
        case '7':
            Sletter = 'P'
            break;
        case '8':
            Sletter = 'W'
            break;
        case '9':
            Sletter = 'X'
            break;
        case '10':
            Sletter = 'Y'
            break;
    }
    var sourceId = $(obj).attr('id');
    var targetSuffixId;

    if (sourceId === 'armyNumber') {
        targetSuffixId = 'txtSuffix';
    } else if (sourceId === 'oldArmyNo') {
        targetSuffixId = 'txtOldSuffix';
    }
    $("#" + targetSuffixId).val(Sletter);
    setOutlineActive(targetSuffixId);

    //var oldArmyNo = $('#oldArmyNo').val();
    //if (oldArmyNo =="")
    //    getApplicantDetalis();
}


function getApplicantDetalis() {

    var armyNumber = $("#armyPrefix").val();
    var Prefix = $("#armyNumber").val();
    var Suffix = $("#txtSuffix").val();
    var appType = parseInt($("#loanType").val(), 10);

    $.ajax({
        type: "get",
        url: "/OnlineApplication/CheckExistUser",
        data: { armyNumber: armyNumber, Prefix: Prefix, Suffix: Suffix, appType: appType },
        success: function (data) {
            if (data.exists) {
                Swal.fire({
                    title: "You Have Already applied for  Loan.",
                    text: "Would you like to apply for a new loan !",
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
            alert("Data Not loaded!")
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
    var armyNumber = $("#armyPrefix").val();
    var Prefix = $("#armyNumber").val();
    var Suffix = $("#txtSuffix").val();
    var appType = parseInt($("#loanType").val(), 10);
    $.ajax({
        type: "get",
        url: "/OnlineApplication/DeleteExistingLoan",
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
    const dateOfBirth = new Date(dob);
    const dateOfCommission = new Date(doc);
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
            icon: 'warning'
        }).then(() => {
            $('#dateOfCommission').val("");
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
    const commissionDate = new Date(value);
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
    maxDate: 0, // This restricts to today and past dates only (no future dates)
    yearRange: "1900:+0", // Allow years from 1900 to current year only
    defaultDate: null, // Default to today
    //onClose: function (dateText, inst) {
    //    // Check if the element and required properties exist before proceeding
    //    if (inst && inst.selectedYear && inst.selectedMonth !== undefined && inst.selectedDay) {
    //        var formattedDate = formatDateToString(new Date(inst.selectedYear, inst.selectedMonth, inst.selectedDay));
    //        $(this).val(formattedDate);
    //    }
    //},
    //onSelect: function (dateText, inst) {
    //    // Format the selected date properly
    //    var formattedDate = formatDateToString(new Date(inst.selectedYear, inst.selectedMonth, inst.selectedDay));
    //    $(this).val(formattedDate);


    //},
    //onSelect: function () {
    //    var dt = $('#dateOfBirth').val();
    //    var newdt = new Date(my_date(dt));
    //    newdt.setFullYear(newdt.getFullYear() + 18);
    //},
    onSelect: function (dateText, inst) {
        // Get the selected date from the input field
        var dt = $('#dateOfBirth').val();

        // Call the custom functions on select
        formatDate(this); // Ensure the function formats the date
        validateDateFormat(this); // Ensure validation is handled 

        // Calculate new date by adding 18 years
        var newdt = new Date(my_date(dt));
        newdt.setFullYear(newdt.getFullYear() + 18);

        // Optional: Set the calculated date back to the field or use as needed

    },
    beforeShowDay: function (date) {
        // Additional check to disable future dates
        var today = new Date();
        today.setHours(23, 59, 59, 999); // Set to end of today
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
    //onClose: function (dateText, inst) {
    //    // Check if the element and required properties exist before proceeding
    //    if (inst && inst.selectedYear && inst.selectedMonth !== undefined && inst.selectedDay) {
    //        var formattedDate = formatDateToString(new Date(inst.selectedYear, inst.selectedMonth, inst.selectedDay));
    //        $(this).val(formattedDate);
    //    }
    //},
    //onSelect: function (dateText, inst) {
    //    // Format the selected date properly
    //    var formattedDate = formatDateToString(new Date(inst.selectedYear, inst.selectedMonth, inst.selectedDay));
    //    $(this).val(formattedDate);


    //},
    //onSelect: function () {
    //    var dt = $('#dateOfCommission').val();
    //    var newdt = new Date(my_date(dt));
    //    newdt.setFullYear(newdt.getFullYear() + 18);
    //}


    onSelect: function (dateText, inst) {
        // Get the selected date from the input field
        var dt = $('#dateOfCommission').val();

        // Call the custom functions on select
        formatDate(this); // Ensure the function formats the date
        validateDateFormat(this); // Ensure validation is handled
        calculateYearDifference(); // Calculate the year difference (if needed)

        // Calculate new date by adding 18 years
        var newdt = new Date(my_date(dt));
        newdt.setFullYear(newdt.getFullYear() + 18);

        // Optional: Set the calculated date back to the field or use as needed

    }
    //beforeShowDay: function (date) {
    //    // Additional check to disable future dates
    //    var today = new Date();
    //    today.setHours(23, 59, 59, 999); // Set to end of today
    //    return [date <= today];
    //}
});

$('.DopPicker').datepicker({
    changeMonth: true,
    changeYear: true,
    showButtonPanel: true,
    dateFormat: 'dd/mm/yy',
    yearRange: "1900:2100", // Allow years from 1900 to current year only
    defaultDate: null, // Default to today
    //onClose: function (dateText, inst) {
    //    // Check if the element and required properties exist before proceeding
    //    if (inst && inst.selectedYear && inst.selectedMonth !== undefined && inst.selectedDay) {
    //        var formattedDate = formatDateToString(new Date(inst.selectedYear, inst.selectedMonth, inst.selectedDay));
    //        $(this).val(formattedDate);
    //    }
    //},
    //onSelect: function (dateText, inst) {
    //    // Format the selected date properly
    //    var formattedDate = formatDateToString(new Date(inst.selectedYear, inst.selectedMonth, inst.selectedDay));
    //    $(this).val(formattedDate);


    //},
    //onSelect: function () {
    //    var dt = $('#dateOfPromotion').val();
    //    var newdt = new Date(my_date(dt));
    //    newdt.setFullYear(newdt.getFullYear() + 18);
    //}
    onSelect: function (dateText, inst) {
        // Get the selected date from the input field
        var dt = $('#dateOfPromotion').val();

        // Call the custom functions on select
        formatDate(this); // Ensure the function formats the date
        updateRetDateOnPromotionDateSelection(); // Calculate the year difference (if needed)

        // Calculate new date by adding 18 years
        var newdt = new Date(my_date(dt));
        newdt.setFullYear(newdt.getFullYear() + 18);

        // Optional: Set the calculated date back to the field or use as needed

    }
    //beforeShowDay: function (date) {
    //    // Additional check to disable future dates
    //    var today = new Date();
    //    today.setHours(23, 59, 59, 999); // Set to end of today
    //    return [date <= today];
    //}
});

$('.Payslippicker').datepicker({
    changeMonth: true,
    changeYear: true,
    showButtonPanel: true,
    dateFormat: 'dd/mm/yy',
    yearRange: "1900:2100", // Allow years from 1900 to current year only
    defaultDate: null, // Default to today
    //onClose: function (dateText, inst) {
    //    // Check if the element and required properties exist before proceeding
    //    if (inst && inst.selectedYear && inst.selectedMonth !== undefined && inst.selectedDay) {
    //        var formattedDate = formatDateToString(new Date(inst.selectedYear, inst.selectedMonth, inst.selectedDay));
    //        $(this).val(formattedDate);
    //    }
    //},
    //onSelect: function (dateText, inst) {
    //    // Format the selected date properly
    //    var formattedDate = formatDateToString(new Date(inst.selectedYear, inst.selectedMonth, inst.selectedDay));
    //    $(this).val(formattedDate);


    //},
    //onSelect: function () {
    //    var dt = $('#dateOfPromotion').val();
    //    var newdt = new Date(my_date(dt));
    //    newdt.setFullYear(newdt.getFullYear() + 18);
    //}
    onSelect: function (dateText, inst) {
        // Get the selected date from the input field
        var dt = $('#monthlyPaySlip').val();

        // Call the custom functions on select
        formatDate(this); // Ensure the function formats the date
        Validate_Salary_Slip_date(this); // Calculate the year difference (if needed)

        // Calculate new date by adding 18 years
        var newdt = new Date(my_date(dt));
        newdt.setFullYear(newdt.getFullYear() + 18);

        // Optional: Set the calculated date back to the field or use as needed

    }
    //beforeShowDay: function (date) {
    //    // Additional check to disable future dates
    //    var today = new Date();
    //    today.setHours(23, 59, 59, 999); // Set to end of today
    //    return [date <= today];
    //}
});

$('.LicencePicker').datepicker({
    changeMonth: true,
    changeYear: true,
    showButtonPanel: true,
    dateFormat: 'dd/mm/yy',
    yearRange: "1900:2100",
    defaultDate: null, // Default to today

    onSelect: function (dateText, inst) {
        // Get the selected date from the input field
        var dt = $('#validity_Date_DL').val();

        // Call the custom functions on select
        formatDate(this); // Ensure the function formats the date

        // Calculate new date by adding 18 years
        var newdt = new Date(my_date(dt));
        newdt.setFullYear(newdt.getFullYear() + 18);

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
    var day = ("0" + date.getDate()).slice(-2);
    var month = ("0" + (date.getMonth() + 1)).slice(-2);
    var year = date.getFullYear();
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
    var date_components = date_string.split("/");
    var day = date_components[0];
    var month = date_components[1];
    var year = date_components[2];
    return new Date(year, month - 1, day);
}

function SetRetDate() {
    var Prefix = $('#armyPrefix').val();
    var ranks = $('#ddlrank').val();
    var rankId = parseInt(ranks);
    var EnrollDate = $('#dateOfCommission').val();
    var regtCorps = $('#regtCorps').val();
    var regtId = parseInt(regtCorps);
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

    var dateOfBirthString = $('#dateOfBirth').val();
    var dateParts = dateOfBirthString.split('/');
    if (dateParts.length === 3) {
        if (EnrollDate == "" || EnrollDate == undefined || dateOfBirthString == "" || dateOfBirthString == undefined) {
            console.log('EnrollDate or dateOfBirthString is empty or undefined.')
        }
        else {
            $.ajax({
                type: "get",
                url: "/OnlineApplication/GetRetirementDate",
                data: { rankId: rankId, Prefix: Prefix, regtId: regtId },
                success: function (data) {
                    if (data.userTypeId == 1) {
                        //userTypeId == 1 => Officers
                        var dateOfBirth = $('#dateOfBirth').val();
                        var dateParts = dateOfBirth.split('/');
                        if (data != 0 && dateParts.length == 3) {
                            var year = dateParts[2];
                            var month = dateParts[1] - 1;
                            var day = dateParts[0];

                            var dob = new Date(year, month, day);
                            dob.setFullYear(dob.getFullYear() + data.retirementAge);
                            var yyyy = dob.getFullYear();
                            var mm = String(dob.getMonth() + 1).padStart(2, '0');
                            var dd = String(dob.getDate()).padStart(2, '0');
                            var formattedDate = `${yyyy}-${mm}-${dd}`;
                            $('#dateOfRetirement').val(formattedDate);
                            setOutlineActive("dateOfRetirement");
                            globleRetirementDate.value = formattedDate;
                            calculateResidualService();
                        } else {
                            $('#dateOfRetirement').val('');
                            console.warn("Invalid retirement age or date of birth.");
                        }
                    }
                    else if (data.userTypeId == 2) {
                        //userTypeId == 2 => Short Service Officers
                        var dateOfCommission = $('#dateOfCommission').val();
                        var dateParts = dateOfCommission.split('/');
                        if (data != 0 && dateParts.length == 3) {
                            var year = dateParts[2];
                            var month = dateParts[1] - 1;
                            var day = dateParts[0];

                            var dob = new Date(year, month, day);
                            dob.setFullYear(dob.getFullYear() + 10);
                            var yyyy = dob.getFullYear();
                            var mm = String(dob.getMonth() + 1).padStart(2, '0');
                            var dd = String(dob.getDate()).padStart(2, '0');
                            var formattedDate = `${yyyy}-${mm}-${dd}`;
                            $('#dateOfRetirement').val(formattedDate);
                            setOutlineActive("dateOfRetirement");
                            globleRetirementDate.value = formattedDate;
                            calculateResidualService();
                        } else {
                            $('#dateOfRetirement').val('');
                            console.warn("Invalid retirement age or date of birth.");
                        }
                    }
                    else if (data.userTypeId == 3 || data.userTypeId == 4) {
                        //userTypeId == 3 => JCOs     userTypeId == 4 => ORs
                        var rankType = $('#ddlrank').val();
                        if (rankType == 0) {
                            alert("Please select Rank Type.");
                        }
                        if (rankType == 31 || rankType == 32 || rankType == 33) {
                            var dateOfBirth = $('#dateOfBirth').val();
                            var dateParts = dateOfBirth.split('/');
                            if (data != 0 && dateParts.length == 3) {
                                var year = dateParts[2];
                                var month = dateParts[1] - 1;
                                var day = dateParts[0];

                                var dob = new Date(year, month, day);
                                dob.setFullYear(dob.getFullYear() + data.retirementAge);
                                var yyyy = dob.getFullYear();
                                var mm = String(dob.getMonth() + 1).padStart(2, '0');
                                var dd = String(dob.getDate()).padStart(2, '0');
                                var formattedDate = `${yyyy}-${mm}-${dd}`;
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
                        else {
                            var dateOfCommission = $('#dateOfCommission').val();
                            var dateParts = dateOfCommission.split('/');
                            if (data != 0 && dateParts.length == 3) {
                                var year = dateParts[2];
                                var month = dateParts[1] - 1;
                                var day = dateParts[0];

                                var dob = new Date(year, month, day);
                                dob.setFullYear(dob.getFullYear() + data.retirementAge);
                                var yyyy = dob.getFullYear();
                                var mm = String(dob.getMonth() + 1).padStart(2, '0');
                                var dd = String(dob.getDate()).padStart(2, '0');
                                var formattedDate = `${yyyy}-${mm}-${dd}`;
                                $('#dateOfRetirement').val(formattedDate);
                                setOutlineActive("dateOfRetirement")
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
    }
    else {
        console.error('Invalid date string.');
    }
}
function calculateResidualService() {
    var retirementDateStr = $('#dateOfRetirement').val(); // Expected format: 'YYYY-MM-DD'
    if (!retirementDateStr) {
        return;
    }

    var retirementDate = new Date(retirementDateStr);
    var currentDate = new Date();

    // Normalize both dates to remove time differences
    retirementDate.setHours(0, 0, 0, 0);
    currentDate.setHours(0, 0, 0, 0);

    var years = retirementDate.getFullYear() - currentDate.getFullYear();
    var months = retirementDate.getMonth() - currentDate.getMonth();
    var days = retirementDate.getDate() - currentDate.getDate();

    if (days < 0) {
        months -= 1;
        var prevMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 0);
        days += prevMonth.getDate(); // Add days of the previous month
    }

    if (months < 0) {
        years -= 1;
        months += 12;
    }

    // Calculate total months (can be negative)
    var totalMonths = years * 12 + months;
    var totalYears = years;

    // If retirement is before current date, show negative values
    if (retirementDate < currentDate) {
        // Recalculate in reverse to maintain negative residual
        var diffInMs = retirementDate - currentDate;
        var diffDate = new Date(Math.abs(diffInMs));
        totalYears = -Math.abs(years);
        totalMonths = -Math.abs(totalMonths);
    }

    $("#totalResidualMonth").val(totalMonths);
    $("#residualService").val(totalYears);
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
    var prefix = $('#armyPrefix').val();
    var extension = $('#ExtnOfService').val();
    var dop = $('#dateOfPromotion').val();

    // Debug log
    console.log('DoP value:', dop);

    // Validate prefix selection
    if (!prefix) {
        Swal.fire({
            title: 'Warning!',
            html: '<p style="font-size: 18px;">Please select Prefix.</p>',
            icon: 'warning',
        });
        return; // Exit early if no prefix selected
    }

    // Check if prefix qualifies for extension (13 or 14)
    if (prefix == 13 || prefix == 14) {
        if (extension == "Yes") {
            // If DoP is not set (empty, null, or undefined), calculate extension
            if (!dop || dop.trim() === "") {
                console.log('DoP not set, calculating service extension...');

                var currentRetDate = $('#dateOfRetirement').val();
                var currentResidualService = parseInt($('#residualService').val());

                // Validate current retirement date exists
                if (!currentRetDate) {
                    Swal.fire({
                        title: 'Error!',
                        html: '<p style="font-size: 18px;">Current retirement date is required for extension calculation.</p>',
                        icon: 'error',
                    });
                    return;
                }

                var dateParts = currentRetDate.split('-');
                if (dateParts.length === 3) {
                    var year = parseInt(dateParts[0]);
                    var month = parseInt(dateParts[1]) - 1; // JavaScript months are 0-indexed
                    var day = parseInt(dateParts[2]);

                    // Create date object and add 2 years for extension
                    var dateOfRetirement = new Date(year, month, day);
                    dateOfRetirement.setFullYear(dateOfRetirement.getFullYear() + 2);

                    // Format the new retirement date
                    var yyyy = dateOfRetirement.getFullYear();
                    var mm = String(dateOfRetirement.getMonth() + 1).padStart(2, '0');
                    var dd = String(dateOfRetirement.getDate()).padStart(2, '0');
                    var formattedDate = `${yyyy}-${mm}-${dd}`;

                    // Update the retirement date field
                    $('#dateOfRetirement').val(formattedDate);

                    // Recalculate residual service
                    calculateResidualService();

                    console.log('Service extended by 2 years. New retirement date:', formattedDate);
                } else {
                    $('#dateOfRetirement').val('');
                    console.error("Invalid retirement date format. Expected YYYY-MM-DD format.");

                    Swal.fire({
                        title: 'Error!',
                        html: '<p style="font-size: 18px;">Invalid retirement date format.</p>',
                        icon: 'error',
                    });
                }
            } else {
                // DoP is set, handle accordingly
                console.log('DoP is set:', dop);
                // Add logic here for when DoP is available
                // This might involve different calculation based on promotion date
            }
        } else {
            if (!dop || dop.trim() === "") {
                // Extension not selected, revert to original retirement date
                if (typeof globleRetirementDate !== 'undefined' && globleRetirementDate.value) {
                    $('#dateOfRetirement').val(globleRetirementDate.value);
                    calculateResidualService();
                    console.log('Extension not selected, reverted to original retirement date');
                } else {
                    console.error('Global retirement date not available');
                }
            } else {
                // DoP is set, handle accordingly
                console.log('DoP is set:', dop);
                // Add logic here for when DoP is available
                // This might involve different calculation based on promotion date
            }


        }
    } else {
        console.log('Prefix does not qualify for extension (not 13 or 14)');
    }
}

// Helper function to validate date format
function isValidDate(dateString) {
    if (!dateString) return false;
    var regex = /^\d{4}-\d{2}-\d{2}$/;
    if (!regex.test(dateString)) return false;

    var date = new Date(dateString);
    var timestamp = date.getTime();

    if (typeof timestamp !== 'number' || Number.isNaN(timestamp)) {
        return false;
    }

    return dateString === date.toISOString().split('T')[0];
}
function ExtensionOfServiceAccess() {
    var prefix = $('#armyPrefix').val();
    var yearOfService = parseFloat($('#residualService').val());
    var extensionDropdown = $('#ExtnOfService');
    // Enable only if Year of Service < 2 and Prefix is JC or OR
    if ((prefix == 13 || prefix == 14) && yearOfService < 2 && yearOfService >= 0) {
        extensionDropdown.prop('disabled', false);
    } else {
        extensionDropdown.prop('disabled', true);
        extensionDropdown.val('');
    }
}
function fetchPCDA_PAO() {
    var regt = $('#regtCorps').val();
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
        var Prefix = $('#armyPrefix').val();

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
        var apo = $('#armyPostOffice').val();

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
function Validate_Salary_Slip_date(inputElement) {
    const value = inputElement.value;

    if (!value) {
        Swal.fire({
            icon: "error",
            title: "Date Required",
            text: "Please select a date.",
        });
        return;
    }

    const selectedDate = new Date(value);
    const currentDate = new Date();
    const threeMonthsAgo = new Date(currentDate.getFullYear(), currentDate.getMonth() - 3, 1);

    // Normalize selectedDate to the 1st of its month
    const normalizedSelectedDate = new Date(selectedDate.getFullYear(), selectedDate.getMonth(), 1);
    const normalizedCurrentMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);

    // Validate range
    if (normalizedSelectedDate < threeMonthsAgo || normalizedSelectedDate > normalizedCurrentMonth) {
        const minAllowed = threeMonthsAgo.toLocaleString("default", { month: "long", year: "numeric" });
        const maxAllowed = normalizedCurrentMonth.toLocaleString("default", { month: "long", year: "numeric" });

        Swal.fire({
            icon: "error",
            title: "Invalid Date",
            text: `Please select a date between ${minAllowed} and ${maxAllowed}.`,
        }).then(() => {
            inputElement.value = "";
            inputElement.focus();
        });
    } else {
        console.log("Valid date:", normalizedSelectedDate);
    }
}
function textChange() {
    function parseValue(value) {
        return value === "" ? 0 : parseInt(value);
    }
    var cr = parseValue($('#basicPay').val().replace(/,/g, ''))
        + parseValue($('#rank_gradePay').val().replace(/,/g, ''))
        + parseValue($('#msp').val().replace(/,/g, ''))
        + parseValue($('#CI_Pay').val().replace(/,/g, ''))
        + parseValue($('#npax_Pay').val().replace(/,/g, ''))
        + parseValue($('#techPay').val().replace(/,/g, ''))
        + parseValue($('#da').val().replace(/,/g, ''))
        + parseValue($('#pmha').val().replace(/,/g, ''))
        + parseValue($('#lra').val().replace(/,/g, ''))
        + parseValue($('#miscPay').val().replace(/,/g, ''));

    var debt = parseValue($('#dsop_afpp').val().replace(/,/g, ''))
        + parseValue($('#agif_Subs').val().replace(/,/g, ''))
        + parseValue($('#incomeTaxMonthly').val().replace(/,/g, ''))
        + parseValue($('#educationCess').val().replace(/,/g, ''))
        + parseValue($('#pli').val().replace(/,/g, ''))
        + parseValue($('#misc_Deduction').val().replace(/,/g, ''))
        + parseValue($('#loanEMI_Outside').val().replace(/,/g, ''))
        + parseValue($('#loanEMI').val().replace(/,/g, ''));
    var totalDebit = debt.toLocaleString('en-IN');
    $('#totalDeductions').val(totalDebit);
    $('#totalCredit').val(cr.toLocaleString('en-IN'));
    $('#salary_After_Deductions').val((cr - debt).toLocaleString('en-IN'));
    setOutlineActive("totalDeductions");
    setOutlineActive("totalCredit");
    setOutlineActive("salary_After_Deductions");
    calculateEMIRepayingCapacity_CA();
    calculateEMIRepayingCapacity_HBA();
    calculateEMIRepayingCapacity_PCA();
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
    $("#btn-save").on("click", function (event) {
        event.preventDefault(); // Prevent form submission
        const form = $("#myForm");
        const inputs = form.find("input, select");
        // Clear previous error messages
        form.find(".error").each(function () {
            $(this).text("");
        });

        // let errorlist = "";
        let errorlist = []; // Use an array to store individual error messages
        let hasError = false;

        const params = new URLSearchParams(window.location.search);

        const loanTypeFromUrl = params.get("loanType");

        const loanTypeFromInput = $('#loanType').val() || null;

        const loanType = loanTypeFromUrl ? loanTypeFromUrl : loanTypeFromInput;
        const applicantCategory = $('#applicantCategory').val() || null;

        filterAmountText(loanType);
        const serviceYearInput = $("#totalService");
        if (serviceYearInput.length) {
            const serviceYearValue = serviceYearInput.val();
            if (serviceYearValue && !isNaN(parseFloat(serviceYearValue.trim())) && parseFloat(serviceYearValue.trim()) < 1 && loanType == 1) {
                const errorSpan = serviceYearInput.parent().find(".error");
                if (errorSpan.length) {
                    errorSpan.text("Total service must be at least 1 yrs");
                }
                errorlist.push("Total Service");
                hasError = true;
            }
            else if (serviceYearValue && !isNaN(parseFloat(serviceYearValue.trim())) && parseFloat(serviceYearValue.trim()) < 2 && (loanType == 2 || loanType == 3)) {
                if (applicantCategory == 2 || applicantCategory == 3) {
                    const errorSpan = serviceYearInput.parent().find(".error");
                    if (errorSpan.length) {
                        errorSpan.text("Total service must be at least 1 yrs");
                    }
                    errorlist.push("Total Service");
                    hasError = true;
                }

            }
        }

        const residualServiceInput = $("#residualService");
        if (residualServiceInput.length) {
            const residualServiceValue = residualServiceInput.val();
            if (residualServiceValue && !isNaN(parseFloat(residualServiceValue.trim())) && parseFloat(residualServiceValue.trim()) < 2) {
                const errorSpan = residualServiceInput.parent().find(".error");
                if (errorSpan.length) {
                    errorSpan.text("Residual service must be at least 2 yrs");
                }
                errorlist.push("Residual Service");
                hasError = true;
            }
        }

        inputs.each(function () {
            const input = $(this);
            const inputElement = this; // Get the DOM element for checkValidity()

            if (loanType === "1" && ($("#pcaAccordianWrapper").find(input).length || $("#caAccordianWrapper").find(input).length)) {
                return;
            }
            else if (loanType === "2" && ($("#pcaAccordianWrapper").find(input).length || $("#hbaAccordianWrapper").find(input).length)) {
                return;
            }
            else if (loanType === "3" && ($("#caAccordianWrapper").find(input).length || $("#hbaAccordianWrapper").find(input).length)) {
                return;
            }

            if (!inputElement.checkValidity()) {
                const errorSpan = input.parent().find(".error");
                if (errorSpan.length) {
                    errorSpan.text(inputElement.validationMessage);
                }
                // errorlist += input.name + ", ";
                let errorText = input.attr("name");
                const prefixes = ["CommonData.", "HBAApplication.", "CarApplication.", "PCAApplication."];
                prefixes.forEach(prefix => {
                    if (errorText.includes(prefix)) {
                        errorText = errorText.replace(prefix, "");
                    }
                });
                errorlist.push(errorText);
                hasError = true;
                if (input.val() && input.val().trim() !== "") {
                    input.removeAttr("required");
                }
            }
        });

        //$("#msgerror").text(hasError ? "Error in: " + errorlist : "");
        //  $("#msgerror").text(hasError ? "Error in: " + errorlist.join(", ") : "");

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

            let unitVal = $('#PresenttxtUnit').val();
            if (unitVal && unitVal.trim() !== '') {
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
                    // If already registered, allow editing of unit
                    //document.querySelector("input[name='Unit']").removeAttribute("required");
                    //if (unitValidation) {
                    //    unitValidation.innerHTML = "";  // Clear error message
                    //    unitValidation.classList.remove("field-validation-error");
                    //    unitValidation.classList.add("field-validation-valid");
                    //}
                    // Open unit search popup
                    $('#unitSearchDialog').show();
                } else if (result === false) {
                    // If not registered, set unit input back to required
                    formSubmitting = true;
                    $('#myForm').submit();
                    //document.getElementById("txtUnit").removeAttribute("disabled");
                    //document.getElementById("txtUnit").setAttribute("required", "required");
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

//$("#unitSearchInput").autocomplete({
//    source: function (request, response) {
//        //alert(1);
//        // $("#Unit").val(0);

//        if (request.term.length > 2) {
//            var param = { "UnitName": request.term };
//            $("#CommonData_IOUnit").val(0);
//            $.ajax({
//                url: '/Account/GetALLByUnitName',
//                contentType: 'application/x-www-form-urlencoded',
//                data: param,
//                type: 'POST',
//                success: function (data) {
//                    if (data.length != 0) {

//                        response($.map(data, function (item) {

//                            return { label: item.pcda_Pao + ' ' + item.name, value: item.id };

//                        }))
//                    }
//                    else {
//                        $("#CommonData_IOUnit").val(0);
//                        $("#unitSearchInput").val("");

//                        //alert("Unit not found.")
//                        Swal.fire({
//                            icon: 'warning',
//                            title: 'Unit not found',
//                            text: 'Please check the unit name and try again.',
//                            confirmButtonText: 'OK'
//                        });
//                    }

//                },
//                error: function (response) {
//                    alert(response.responseText);
//                },
//                failure: function (response) {
//                    alert(response.responseText);
//                }
//            });
//        }
//    },
//    select: function (e, i) {
//        e.preventDefault();

//        $("#unitSearchInput").val(i.item.label);
//        //$("#IOUnit").val(i.item.value);

//        //$("#PresenttxtUnit").val(i.item.label);
//        $("#CommonData_IOUnit").val(i.item.value);
//       // $("input[name='CommonData.PresentUnit']").val(i.item.value);


//        $("#unitSearchConfirmBtn").prop("disabled", false);
//    },
//    appendTo: '#suggesstion-box'
//});

$('#unitSearchInput').on('input', function () {
    const inputValue = $(this).val().trim();
    $('#unitSearchConfirmBtn').prop('disabled', inputValue === '');
});

$("#unitSearchConfirmBtn").click(function (e) {
    //const $message = $('#unitSearchMessage');
    //if ($message.length) {
    //    $message.hide();
    //}
    e.preventDefault();
    e.stopPropagation();
    //var value = $("#CommonData_IOUnit").val()
    var value = $("#unitSearchInput").val()
    //console.log($("#PresentUnitId").val());

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

    //$.ajax({
    //    url: '/OnlineApplication/CheckIsUnitRegister',
    //    method: 'GET',
    //    data: { ArmyNo: (ArmyNo).toUpperCase()},
    //    success: function (response) {
    //        if (response === true) {

    //            $('#unitSearchMessage').text("Unit Already Registered.\nYou are already registered as CO for this unit. Please select another unit.");

    //            // Clear input and reopen dialog
    //            $("#unitSearchInput").val("");
    //            $("#CommonData_IOUnit").val(0)
    //        }

    //        else {


    //            checkUnitRegistration(unitId);


    //        }
    //    },
    //    error: function () {
    //        alert('Could not verify unit registration. Please try again.');
    //    }
    //});

    if (ArmyNo == Value.toUpperCase()) {
        //console.log("Unit is same as Army No");
        $('#unitSearchMessage').text("Army Number Already Registered.\nYou are already registered as CO for this unit. Please select another Army Number.");
    }

    // else {
    //$.ajax({
    //    url: '/OnlineApplication/CheckIsUnitRegister',
    //    method: 'GET',
    //    data: { ArmyNo: (ArmyNo).toUpperCase()},
    //    success: function (response) {
    //        if (response === true) {

    //            $('#unitSearchMessage').text("Unit Already Registered.\nYou are already registered as CO for this unit. Please select another unit.");

    //            // Clear input and reopen dialog
    //            $("#unitSearchInput").val("");
    //            $("#CommonData_IOUnit").val(0)
    //        }

    //        else {


    //            checkUnitRegistration(unitId);


    //        }
    //    },
    //    error: function () {
    //        alert('Could not verify unit registration. Please try again.');
    //    }
    //});
    //checkUnitRegistration(ArmyNo);
    //}

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
                        $('#myForm').submit();
                    } else if (result === false) {
                        // If not registered, set unit input back to required
                        $('#unitSearchMessage').text();
                        formSubmitting = false;
                        //$('#myForm').submit();
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
                                // Redirect to Create page
                                //var appType = $('#ApplicationType').val();
                                //if (appType == "9") {
                                //    window.location.href = `/HBA_Application/Create`;
                                //}
                                //else {
                                //    window.location.href = `/Car_PC_Advance_Application/Create`;
                                //}
                                $('#unitSearchDialog').hide();
                            } else if (result.isDismissed) {
                                // If Cancel is clicked, reopen the unit search popup
                                //$('#txtUnit').val('');
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

//function checkUnitRegistration(selectedUnitId) {
//    $.ajax({
//        url: '/OnlineApplication/CheckIsCoRegister',
//        method: 'GET',
//        data: { UnitId: selectedUnitId },
//        success: function (response) {
//            if (response === true) {
//                //alert('Unit Available. You can proceed with this unit.');
//                $("#CommonData_IOUnit").val(selectedUnitId);
//                //$('#txtUnit').val(selectedUnitName);
//                formSubmitting = true;
//                $('#myForm').submit();
//            } else {
//                //if (confirm('Unit registration pending/not activated. Please approach your Unit IO.\nGo to Create page?')) {
//                //    window.location.href = '/Car_PC_Advance_Application/Create';
//                //} else {
//                //    openUnitSearchPopup();
//                //}
//                $('#unitSearchMessage').text('');


//                Swal.fire({
//                    icon: 'info',
//                    title: '<span style="font-size: 20px;">Unit Registration Pending/Not Activated</span>',
//                    html: '<span style="font-size: 18px;">Please approach your Unit IO to register/contact to Agif.</span>',
//                    confirmButtonText: 'OK',
//                    cancelButtonText: 'Cancel', // Cancel button text
//                    showCancelButton: true, // Enable cancel button
//                    reverseButtons: true,  // Make Cancel button appear on the left
//                }).then((result) => {
//                    if (result.isConfirmed) {
//                        // Redirect to Create page
//                        //var appType = $('#ApplicationType').val();
//                        //if (appType == "9") {
//                        //    window.location.href = `/HBA_Application/Create`;
//                        //}
//                        //else {
//                        //    window.location.href = `/Car_PC_Advance_Application/Create`;
//                        //}
//                        $('#unitSearchDialog').hide();
//                    } else if (result.isDismissed) {
//                        // If Cancel is clicked, reopen the unit search popup
//                        //$('#txtUnit').val('');
//                        $("unitSearchDialog").show();
//                    }
//                });
//            }
//        },
//        error: function () {
//            alert('Could not verify unit registration. Please try again.');
//        }
//    });
//}

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

//PCA Calculations
function RefreshMaxAmt_PCA() {
    $("#computerCost").on('change', function () {
        var rawValue = $('#computerCost').val().replace(/,/g, '');
        var PcCost = parseFloat(rawValue);

        PcCost = Math.round(PcCost * 0.9);
        if (isNaN(PcCost)) {
            alert("Please enter a valid computer cost.");
            return;
        }
        var Amount = 200000;
        if (Amount > PcCost) {
            $('#PCA_Amt_Eligible_for_loan').val(PcCost.toLocaleString('en-IN'));
        }
        else {
            $('#PCA_Amt_Eligible_for_loan').val(Amount.toLocaleString('en-IN'));
        }

        setOutlineActive("PCA_Amt_Eligible_for_loan");
        calculateMaxEMI_PCA();
        calculateEMIRepayingCapacity_PCA();
    });
}
function calculateEMIRepayingCapacity_PCA() {
    var credit = parseInt($('#totalCredit').val().replace(/,/g, ''));
    var debit = parseInt($('#totalDeductions').val().replace(/,/g, ''));
    if (isNaN(credit) || isNaN(debit)) {
        alert("Please enter valid credit and debit amounts.");
        return;
    }
    var repayingCapacity = credit * 0.75 - debit;
    if (repayingCapacity < 0) {
        repayingCapacity = 0;
    }
    $('#PCA_repayingCapacity').val(repayingCapacity.toLocaleString('en-IN'));
    setOutlineActive("PCA_repayingCapacity");
}
function calculateMaxEMI_PCA() {

    var Residual = parseInt($('#totalResidualMonth').val().trim()) || 0;
    Residual -= 6;

    var EMI = 48;
    if (EMI < Residual) {
        $("#PCA_EMI_Eligible").val(EMI);
    }
    else {
        $("#PCA_EMI_Eligible").val(Residual);
    }

    setOutlineActive("PCA_EMI_Eligible");
}
function validateAmount_PCA(input) {
    var $input = $(input);
    var enteredAmount = parseFloat($input.val().replace(/,/g, ''));
    var eligibleAmount = parseFloat($("#PCA_Amt_Eligible_for_loan").val().replace(/,/g, ''));
    var disbusermentAmt = enteredAmount * 0.99;
    if (isNaN(enteredAmount)) {
        $input.val('');
    } else {
        if (enteredAmount > eligibleAmount) {
            $input.val($("#PCA_Amt_Eligible_for_loan").val());
            disbusermentAmt = parseFloat($("#PCA_Amt_Eligible_for_loan").val().replace(/,/g, '')) * 0.99;
        }
    }

    $('#PCA_approxDisbursementAmt').val(disbusermentAmt.toLocaleString('en-IN'));
    setOutlineActive("PCA_approxDisbursementAmt");
}
function validateEMI_PCA(input) {
    var $input = $(input);
    var enteredEMI = parseFloat($input.val());
    var eligibleEMI = parseFloat($("#PCA_EMI_Eligible").val());

    if (isNaN(enteredEMI)) {
        $input.val('');
    } else {
        if (enteredEMI > eligibleEMI) {
            $input.val(eligibleEMI);
        }
    }
}
function calculateEMI_PCA() {
    let P = parseFloat($("#PCA_Amount_Applied_For_Loan").val().replace(/[^0-9.]/g, '')); // Loan amount
    let N = parseInt($("#PCA_EMI_Applied").val()); // Number of EMIs
    let R = (8.50 / 12) / 100;

    if (!P || !N || !R) {
        $("#PCA_approxEMIAmount").val("");
        return;
    }

    let EMI = (P * R * Math.pow(1 + R, N)) / (Math.pow(1 + R, N) - 1);
    $("#PCA_approxEMIAmount").val(Number(EMI.toFixed(0)).toLocaleString('en-IN'));

    setOutlineActive("PCA_approxEMIAmount");
}

//CA Calculations
function RefreshMaxAmt_CA() {
    $("#vehicleCost").on('change', function () {
        var vehicalType = $('#veh_Loan_Type').val();
        var CarEngineType = $('#VehTypeId').val();
        var rawValue = $('#vehicleCost').val().replace(/,/g, '');
        var carCost = parseFloat(rawValue);
        var prefix = $('#armyPrefix').val();
        carCost = Math.round(carCost * 0.9);
        var Amount = 0;
        if (isNaN(carCost)) {
            alert("Please enter a valid car cost.");
            return;
        }
        if (vehicalType == 2) {
            if (CarEngineType == 4)
                Amount = (prefix == 13 || prefix == 14) ? 1500000 : 2500000;
            else
                Amount = (prefix == 13 || prefix == 14) ? 1000000 : 2000000;
        }
        else if (vehicalType == 3) {
            Amount = (prefix == 13 || prefix == 14) ? 500000 : 1000000;
        }
        else {
            Amount = (prefix == 13 || prefix == 14) ? 200000 : 1000000;
        }

        if (Amount > carCost) {
            $('#CA_Amt_Eligible_for_loan').val(carCost.toLocaleString('en-IN'));
        }
        else {
            $('#CA_Amt_Eligible_for_loan').val(Amount.toLocaleString('en-IN'));
        }

        setOutlineActive("CA_Amt_Eligible_for_loan");
        calculateMaxEMI_CA(vehicalType);
        calculateEMIRepayingCapacity_CA();
    });
}
function calculateEMIRepayingCapacity_CA() {
    var credit = parseInt($('#totalCredit').val().replace(/,/g, ''));
    var debit = parseInt($('#totalDeductions').val().replace(/,/g, ''));
    if (isNaN(credit) || isNaN(debit)) {
        alert("Please enter valid credit and debit amounts.");
        return;
    }
    var repayingCapacity = credit * 0.75 - debit;
    if (repayingCapacity < 0) {
        repayingCapacity = 0;
    }
    $('#CA_repayingCapacity').val(repayingCapacity.toLocaleString('en-IN'));

    setOutlineActive("CA_repayingCapacity");
}
function calculateMaxEMI_CA(vehicalType) {

    var Residual = parseInt($('#totalResidualMonth').val().trim()) || 0;
    Residual -= 6;
    var freqOfLoan = parseInt($('#CA_LoanFreq').val().trim()) || 0;
    var EMI = 0;
    if (vehicalType == 2) {
        EMI = 96;
        if (freqOfLoan == 2) {
            EMI = 72;
        }
        if (EMI < Residual) {
            $("#CA_EMI_Eligible").val(EMI);
        }
        else {
            $("#CA_EMI_Eligible").val(Residual);
        }
    }
    else if (vehicalType == 3) {
        EMI = 72;
        if (EMI < Residual) {
            $("#CA_EMI_Eligible").val(EMI);
        }
        else {
            $("#CA_EMI_Eligible").val(Residual);
        }
    }
    else {
        EMI = 60;
        if (EMI < Residual) {
            $("#CA_EMI_Eligible").val(EMI);
        }
        else {
            $("#CA_EMI_Eligible").val(Residual);
        }
    }

    setOutlineActive("CA_EMI_Eligible");

}
function validateAmount_CA(input) {
    var $input = $(input);
    var enteredAmount = parseFloat($input.val().replace(/,/g, ''));
    var eligibleAmount = parseFloat($("#CA_Amt_Eligible_for_loan").val().replace(/,/g, ''));
    var disbusermentAmt = enteredAmount * 0.99;
    if (isNaN(enteredAmount)) {
        $input.val('');
    } else {
        if (enteredAmount > eligibleAmount) {
            $input.val($("#CA_Amt_Eligible_for_loan").val());
            disbusermentAmt = parseFloat($("#CA_Amt_Eligible_for_loan").val().replace(/,/g, '')) * 0.99;
        }
    }

    $('#CA_approxDisbursementAmt').val(disbusermentAmt.toLocaleString('en-IN'));

    setOutlineActive("CA_approxDisbursementAmt");
}
function validateEMI_CA(input) {
    var $input = $(input);
    var enteredEMI = parseFloat($input.val());
    var eligibleEMI = parseFloat($("#CA_EMI_Eligible").val());

    if (isNaN(enteredEMI)) {
        $input.val('');
    } else {
        if (enteredEMI > eligibleEMI) {
            $input.val(eligibleEMI);
        }
    }
}
function calculateEMI_CA() {
    let P = parseFloat($("#CA_Amount_Applied_For_Loan").val().replace(/[^0-9.]/g, '')); // Loan amount
    let N = parseInt($("#CA_EMI_Applied").val()); // Number of EMIs
    let R = (8.50 / 12) / 100;

    if (!P || !N || !R) {
        $("#CA_approxEMIAmount").val("");
        return;
    }

    let EMI = (P * R * Math.pow(1 + R, N)) / (Math.pow(1 + R, N) - 1);
    $("#CA_approxEMIAmount").val(Number(EMI.toFixed(0)).toLocaleString('en-IN'));

    setOutlineActive("CA_approxEMIAmount");
}

//HBA Calculations
function RefreshMaxAmt_HBA() {
    $("#propertyCost").on('change', function () {
        var propType = $('#propertyType').val();
        if (!propType || propType == "0") {
            alert("Please select Property Type.");
            return;
        }
        var rawValue = $('#propertyCost').val().replace(/,/g, '');
        var propertyCost = parseFloat(rawValue);

        propertyCost = Math.round(propertyCost * 0.85);
        if (isNaN(propertyCost)) {
            alert("Please enter a valid car cost.");
            return;
        }
        if (propType == 5) {
            var Amount = 2000000;
            if (Amount > propertyCost) {
                $('#HBA_Amt_Eligible_for_loan').val(propertyCost.toLocaleString('en-IN'));
            }
            else {
                $('#HBA_Amt_Eligible_for_loan').val(Amount.toLocaleString('en-IN'));
            }
        }
        else {
            var prefix = $('#armyPrefix').val();
            if (!prefix || prefix == "0") {
                alert("Please select Prefix.");
                return;
            }
            if (prefix == 13) {
                var Amount = 5000000;
                if (Amount > propertyCost) {
                    $('#HBA_Amt_Eligible_for_loan').val(propertyCost.toLocaleString('en-IN'));
                }
                else {
                    $('#HBA_Amt_Eligible_for_loan').val(Amount.toLocaleString('en-IN'));
                }
            }
            else if (prefix == 14) {
                var Amount = 4000000;
                if (Amount > propertyCost) {
                    $('#HBA_Amt_Eligible_for_loan').val(propertyCost.toLocaleString('en-IN'));
                }
                else {
                    $('#HBA_Amt_Eligible_for_loan').val(Amount.toLocaleString('en-IN'));
                }
            }
            else {
                var Amount = 10000000;
                if (Amount > propertyCost) {
                    $('#HBA_Amt_Eligible_for_loan').val(propertyCost.toLocaleString('en-IN'));
                }
                else {
                    $('#HBA_Amt_Eligible_for_loan').val(Amount.toLocaleString('en-IN'));
                }
            }
        }

        setOutlineActive("HBA_Amt_Eligible_for_loan");
        calculateMaxEMI_HBA(propType);
        calculateEMIRepayingCapacity_HBA();
    });
}
function calculateEMIRepayingCapacity_HBA() {
    var credit = parseInt($('#totalCredit').val().replace(/,/g, ''));
    var debit = parseInt($('#totalDeductions').val().replace(/,/g, ''));
    if (isNaN(credit) || isNaN(debit)) {
        alert("Please enter valid credit and debit amounts.");
        return;
    }
    var repayingCapacity = credit * 0.75 - debit;
    if (repayingCapacity < 0) {
        repayingCapacity = 0;
    }
    $('#HBA_repayingCapacity').val(repayingCapacity.toLocaleString('en-IN'));
    setOutlineActive("HBA_repayingCapacity");
}
function calculateMaxEMI_HBA(propType) {
    var Residual = parseInt($('#totalResidualMonth').val().trim()) || 0;
    Residual -= 6;

    if (propType == 5) {
        EMI = 120;
        if (EMI < Residual) {
            $("#HBA_EMI_Eligible").val(EMI);
        }
        else {
            $("#HBA_EMI_Eligible").val(Residual);
        }
    }
    else {
        EMI = 240;
        if (EMI < Residual) {
            $("#HBA_EMI_Eligible").val(EMI);
        }
        else {
            $("#HBA_EMI_Eligible").val(Residual);
        }
    }

    setOutlineActive("HBA_EMI_Eligible");
}
function validateAmount_HBA(input) {
    var $input = $(input);
    var enteredAmount = parseFloat($input.val().replace(/,/g, ''));
    var eligibleAmount = parseFloat($("#HBA_Amt_Eligible_for_loan").val().replace(/,/g, ''));
    var disbusermentAmt = enteredAmount * 0.99;
    if (isNaN(enteredAmount)) {
        $input.val('');
    } else {
        if (enteredAmount > eligibleAmount) {
            $input.val($("#HBA_Amt_Eligible_for_loan").val());
            disbusermentAmt = parseFloat($("#HBA_Amt_Eligible_for_loan").val().replace(/,/g, '')) * 0.99;
        }
    }

    $('#HBA_approxDisbursementAmt').val(disbusermentAmt.toLocaleString('en-IN'));

    setOutlineActive("HBA_approxDisbursementAmt");
}
function validateEMI_HBA(input) {
    var $input = $(input);
    var enteredEMI = parseFloat($input.val());
    var eligibleEMI = parseFloat($("#HBA_EMI_Eligible").val());

    if (isNaN(enteredEMI)) {
        $input.val('');
    } else {
        if (enteredEMI > eligibleEMI) {
            $input.val(eligibleEMI);
        }
    }
}
function calculateEMI_HBA() {
    let P = parseFloat($("#HBA_Amount_Applied_For_Loan").val().replace(/[^0-9.]/g, '')); // Loan amount
    let N = parseInt($("#HBA_EMI_Applied").val()); // Number of EMIs
    let R = (8.50 / 12) / 100;

    if (!P || !N || !R) {
        $("#HBA_approxEMIAmount").val("");
        return;
    }

    let EMI = (P * R * Math.pow(1 + R, N)) / (Math.pow(1 + R, N) - 1);
    $("#HBA_approxEMIAmount").val(Number(EMI.toFixed(0)).toLocaleString('en-IN'));

    setOutlineActive("HBA_approxEMIAmount");
}

function validateEMIHba() {
    var hbaApproxEmi = parseFloat($("#HBA_approxEMIAmount").val().replace(/,/g, ''));
    var hbaEmiRepayingCapacity = parseFloat($("#HBA_repayingCapacity").val().replace(/,/g, ''));
    if (isNaN(hbaApproxEmi) || isNaN(hbaEmiRepayingCapacity)) {
        alert("Please ensure all fields are filled correctly.");
        return false;
    }
    if (hbaApproxEmi > hbaEmiRepayingCapacity) {
        Swal.fire({
            title: "EMI Exceeds Repaying Capacity",
            text: "The calculated EMI exceeds your repaying capacity. Please adjust the loan amount or EMI.",
            icon: "warning",
        }).then(() => {
            $('#HBA_Amount_Applied_For_Loan').val("");
            $('#HBA_EMI_Applied').val("");
            $('#HBA_approxEMIAmount').val("");
            $('#HBA_approxDisbursementAmt').val("");
        })
    }
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
        $("input[name='CommonData.ParentUnit']").val(i.item.value);
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
        $("input[name='CommonData.PresentUnit']").val(i.item.value);

        // $("#spnUnitMapId").html(i.item.value);
        //alert(i.item.value)

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

function validateEMIForRepayingCapacity(prefix) {
    var approxEmi = parseFloat($(`#${prefix}_approxEMIAmount`).val().replace(/,/g, ''));
    var repayingCapacity = parseFloat($(`#${prefix}_repayingCapacity`).val().replace(/,/g, ''));

    if (isNaN(approxEmi) || isNaN(repayingCapacity)) {
        alert("Please ensure all fields are filled correctly.");
        return false;
    }

    if (approxEmi > repayingCapacity) {
        Swal.fire({
            title: "EMI Exceeds Repaying Capacity",
            text: "The calculated EMI exceeds your repaying capacity. Please adjust the loan amount or EMI.",
            icon: "warning",
        }).then(() => {
            $(`#${prefix}_Amount_Applied_For_Loan`).val("");
            $(`#${prefix}_EMI_Applied`).val("");
            $(`#${prefix}_approxEMIAmount`).val("");
            $(`#${prefix}_approxDisbursementAmt`).val("");
        });
        return false;
    }

    return true;
}
