
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
    accordionAutoOpenClose();
    findDataWithArmyNumber();
    findDataWithApplicationId();
});
function accordionAutoOpenClose() {
    $('.accordion').on('keydown', '.last-input', function (e) {
        if (e.key === 'Tab' && !e.shiftKey) {
            e.preventDefault();
            const currentAccordionItem = $(this).closest('.accordion-item');
            const nextAccordionItem = currentAccordionItem.next('.accordion-item');
            if (nextAccordionItem.length > 0) {
                currentAccordionItem.find('.accordion-collapse').collapse('hide');
                nextAccordionItem.find('.accordion-collapse').collapse('show');
            }
            setTimeout(() => {
                nextAccordionItem.find('input:visible:first').focus();
            }, 300);
        }

    });
}
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


    const applicantCategoryFromUrl = params.get("applicantCategory");

    const applicantCategoryFromInput = document.getElementById('applicantCategory')?.value || null;

    const applicantCategory = applicantCategoryFromUrl ? applicantCategoryFromUrl : applicantCategoryFromInput;

    const armyPrefixValue = $('#armyPrefix').data('army-prefix');
    const OldArmyPrefixvalue = $('#oldArmyPrefix').data('oldarmy-prefix');
    const Rank = $('#ddlrank').data('rank-prefix');
    const regtcorps = $('#regtCorps').data('regt-prefix');
    const parentunit = $('#parentUnit').data('parent-prefix');
    const presentunit = $('#presentUnit').data('present-prefix');
    const Armypostoffice = $('#armyPostOffice').data('armypost-prefix');
    const propertytype = $('#propertyType').data('propertytype-prefix');
    const hbaloanfreq = $('#HBA_LoanFreq').data('hbaloanfreq-prefix');
    const vehicletype = $('#veh_Loan_Type').data('vehicletype-prefix');
    const Cavehicleloanfreq = $('#CA_LoanFreq').data('vehicleloanfreq-prefix');
    const computer_Loan_Type = $('#computer_Loan_Type').data('pcaloantype-prefix');
    const Pcaloanfreq = $('#PCA_LoanFreq').data('pcaloanfreq-prefix');
    const VehType = $('#VehTypeId').data('vehicletypeId-prefix');

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
        const accountNo = $('#salaryAcctNo').val();
        const reEnterAccountNo = $('#confirmSalaryAcctNo').val();
        if (accountNo !== reEnterAccountNo) {
            Swal.fire({
                title: "Alert",
                text: "You Salary Account No is mismatch",
                icon: "warning"
            }).then(() => {

                $('#confirmSalaryAcctNo').val("");
                $('#confirmSalaryAcctNo').focus();
            });
        }

    });
}
function SetSuffixLetter(obj) {
    let arr1 = [], arr2 = [];
    let ArmyNumber = $(obj).val();
    const fixed = "98765432";
    arr1 = ArmyNumber.split('');
    arr2 = fixed.split('');
    const len = arr2.length - arr1.length;

    if (len === 1) {
        ArmyNumber = "0" + ArmyNumber;
    }
    if (len === 2) {
        ArmyNumber = "00" + ArmyNumber;
    }
    if (len === 3) {
        ArmyNumber = "000" + ArmyNumber;
    }
    if (len === 4) {
        ArmyNumber = "0000" + ArmyNumber;
    }
    if (len === 5) {
        ArmyNumber = "00000" + ArmyNumber;
    }
    if (len === 6) {
        ArmyNumber = "000000" + ArmyNumber;
    }
    if (len === 7) {
        ArmyNumber = "0000000" + ArmyNumber;
    }

    let total = 0;
    arr1 = ArmyNumber.split('');
    for (let i = 0; i < arr1.length; i++) {
        const val1 = arr1[i];
        const val2 = arr2[i];
        total += parseInt(val1) * parseInt(val2);
    }

    const rem = total % 11;
    let Sletter = '';
    switch (rem.toString()) {
        case '0':
            Sletter = 'A';
            break;
        case '1':
            Sletter = 'F';
            break;
        case '2':
            Sletter = 'H';
            break;
        case '3':
            Sletter = 'K';
            break;
        case '4':
            Sletter = 'L';
            break;
        case '5':
            Sletter = 'M';
            break;
        case '6':
            Sletter = 'N';
            break;
        case '7':
            Sletter = 'P';
            break;
        case '8':
            Sletter = 'W';
            break;
        case '9':
            Sletter = 'X';
            break;
        case '10':
            Sletter = 'Y';
            break;
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
        url: "/OnlineApplication/CheckExistUser",
        data: { armyNumber: armyNumber, Prefix: Prefix, Suffix: Suffix, appType: appType },
        success: function (data) {
            if (data.exists) {
                Swal.fire({
                    title: "You Have Already applied for Loan.",
                    text: "Would you like to apply for a new Loan !",
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
        text: "Your previous Loan data will be deleted permanently !",
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
            icon: 'warning'
        }).then(() => {
            $('#dateOfCommission').val("");
        });
    }
}

function calculateYearDifference() {
    const value = $('#dateOfCommission').val();
    if (!value) {
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
    maxDate: 0,
    yearRange: "1900:+0",
    defaultDate: null,

    onSelect: function (dateText, inst) {
        const dt = $('#dateOfBirth').val();

        formatDate(this);
        validateDateFormat(this);

        let newdt = new Date(my_date(dt));
        newdt.setFullYear(newdt.getFullYear() + 18);

    },
    beforeShowDay: function (date) {
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
    maxDate: 0,
    yearRange: "1900:+0",
    defaultDate: null,



    onSelect: function (dateText, inst) {
        const dt = $('#dateOfCommission').val();

        formatDate(this);
        validateDateFormat(this);
        calculateYearDifference();

        let newdt = new Date(my_date(dt));
        newdt.setFullYear(newdt.getFullYear() + 18);

    }
});

$('.DopPicker').datepicker({
    changeMonth: true,
    changeYear: true,
    showButtonPanel: true,
    dateFormat: 'dd/mm/yy',
    yearRange: "1900:2100",
    defaultDate: null,

    onSelect: function (dateText, inst) {
        const dt = $('#dateOfPromotion').val();

        formatDate(this);
        updateRetDateOnPromotionDateSelection();

        let newdt = new Date(my_date(dt));
        newdt.setFullYear(newdt.getFullYear() + 18);


    }
});

$('.Payslippicker').datepicker({
    changeMonth: true,
    changeYear: true,
    showButtonPanel: true,
    dateFormat: 'dd/mm/yy',
    yearRange: "1900:2100",
    defaultDate: null,

    onSelect: function (dateText, inst) {
        const dt = $('#monthlyPaySlip').val();


        formatDate(this);
        Validate_Salary_Slip_date(this);

        let newdt = new Date(my_date(dt));
        newdt.setFullYear(newdt.getFullYear() + 18);


    }
});

$('.LicencePicker').datepicker({
    changeMonth: true,
    changeYear: true,
    showButtonPanel: true,
    dateFormat: 'dd/mm/yy',
    yearRange: "1900:2100",
    minDate: 1,
    defaultDate: null,

    onSelect: function (dateText, inst) {
        const dt = $('#validity_Date_DL').val();

        formatDate(this);

        let newdt = new Date(my_date(dt));
        newdt.setFullYear(newdt.getFullYear() + 18);

    }

});
$('.LicencePicker').on('blur change', function () {
    const inputDate = $(this).val();

    if (inputDate) {
        const parts = inputDate.split('/');
        if (parts.length === 3) {
            const day = parseInt(parts[0], 10);
            const month = parseInt(parts[1], 10) - 1;
            const year = parseInt(parts[2], 10);

            const selectedDate = new Date(year, month, day);
            const today = new Date();
            today.setHours(0, 0, 0, 0);
            if (selectedDate < today) {
                $(this).val('');
                Swal.fire({
                    title: "Alert",
                    text: "Your Driving Licence has Expired!",
                    icon: "warning"
                })
                $(this).focus();
            }
        }
    }
});
function formatDate(input) {
    let value = input.value.replace(/[^\d]/g, '');

    let cursorPosition = input.selectionStart;
    let oldLength = input.value.length;

    if (value.length >= 2) {
        value = value.substring(0, 2) + '/' + value.substring(2);
    }
    if (value.length >= 5) {
        value = value.substring(0, 5) + '/' + value.substring(5);
    }

    if (value.length > 10) {
        value = value.substring(0, 10);
    }

    input.value = value;

    let newLength = input.value.length;
    if (newLength > oldLength) {
        cursorPosition++;
    }

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

    if (value && !datePattern.test(value)) {
        Swal.fire({
            icon: "error",
            title: "Invalid date",
            text: "Invalid date format. Please select a valid date.",
        });
        input.focus();
        input.value = "";
        return;
    }

    if (value && datePattern.test(value)) {
        const parts = value.split('/');
        const day = parseInt(parts[0], 10);
        const month = parseInt(parts[1], 10) - 1;
        const year = parseInt(parts[2], 10);

        const currentDate = new Date();
        const currentYear = currentDate.getFullYear();
        const currentMonth = currentDate.getMonth();
        const currentDay = currentDate.getDate();

        if (year < 1900 || year > currentYear) {
            Swal.fire({
                icon: "error",
                title: "Invalid year",
                text: `Please enter a year between 1900 and ${currentYear}.`,
            });
            input.focus();
            input.value = "";
            return;
        }

        if (year === currentYear && month > currentMonth) {
            Swal.fire({
                icon: "error",
                title: "Invalid month",
                text: "Month cannot be in the future.",
            });
            input.focus();
            input.value = "";
            return;
        }

        if (year === currentYear && month === currentMonth && day > currentDay) {
            Swal.fire({
                icon: "error",
                title: "Invalid day",
                text: "Day cannot be in the future.",
            });
            input.focus();
            input.value = "";
            return;
        }

        const inputDate = new Date(year, month, day);
        if (inputDate.getDate() !== day) {
            Swal.fire({
                icon: "error",
                title: "Invalid date",
                text: "The date you entered does not exist.",
            });
            input.focus();
            input.value = "";
            return;
        }
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
    if (Prefix === "0" || Prefix === "") {
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
        if (EnrollDate === "" || EnrollDate === undefined || dateOfBirthString === "" || dateOfBirthString === undefined) {
            console.log('EnrollDate or dateOfBirthString is empty or undefined.');
        } else {
            $.ajax({
                type: "get",
                url: "/OnlineApplication/GetRetirementDate",
                data: { rankId: rankId, Prefix: Prefix, regtId: regtId },
                success: function (data) {
                    if (data.userTypeId == 1) {
                        // userTypeId == 1 => Officers
                        const dateOfBirth = $('#dateOfBirth').val();
                        const dateParts = dateOfBirth.split('/');
                        if (data != 0 && dateParts.length == 3) {
                            const year = dateParts[2];
                            const month = dateParts[1] - 1;
                            const day = dateParts[0];

                            const dob = new Date(year, month, day);
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
                        // userTypeId == 2 => Short Service Officers
                        const dateOfCommission = $('#dateOfCommission').val();
                        const dateParts = dateOfCommission.split('/');
                        if (data != 0 && dateParts.length == 3) {
                            const year = dateParts[2];
                            const month = dateParts[1] - 1;
                            const day = dateParts[0];

                            const dob = new Date(year, month, day);
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
                        // userTypeId == 3 => JCOs     userTypeId == 4 => ORs
                        const rankType = $('#ddlrank').val();
                        if (rankType == 0) {
                            alert("Please select Rank Type.");
                        }
                        if (rankType == 31 || rankType == 32 || rankType == 33) {
                            const dateOfBirth = $('#dateOfBirth').val();
                            const dateParts = dateOfBirth.split('/');
                            if (data != 0 && dateParts.length == 3) {
                                const year = dateParts[2];
                                const month = dateParts[1] - 1;
                                const day = dateParts[0];

                                const dob = new Date(year, month, day);
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
                            const dateOfCommission = $('#dateOfCommission').val();
                            const dateParts = dateOfCommission.split('/');
                            if (data != 0 && dateParts.length == 3) {
                                const year = dateParts[2];
                                const month = dateParts[1] - 1;
                                const day = dateParts[0];

                                const dob = new Date(year, month, day);
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
                    alert("Data Not loaded!");
                }
            });
        }
    } else {
        console.error('Invalid date string.');
    }
}

function calculateResidualService() {
    const retirementDateStr = $('#dateOfRetirement').val();
    if (!retirementDateStr) {
        return;
    }

    const retirementDate = new Date(retirementDateStr);
    const currentDate = new Date();

    retirementDate.setHours(0, 0, 0, 0);
    currentDate.setHours(0, 0, 0, 0);

    let years = retirementDate.getFullYear() - currentDate.getFullYear();
    let months = retirementDate.getMonth() - currentDate.getMonth();
    let days = retirementDate.getDate() - currentDate.getDate();

    if (days < 0) {
        months -= 1;
        const prevMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 0);
        days += prevMonth.getDate();
    }

    if (months < 0) {
        years -= 1;
        months += 12;
    }

    let totalMonths = years * 12 + months;
    let totalYears = years;

    if (retirementDate < currentDate) {
        const diffInMs = retirementDate - currentDate;
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
    const promotionDate = $('#dateOfPromotion').val();
    if (!promotionDate) {
        return;
    }
    const dateParts = promotionDate.split('/');
    if (dateParts.length === 3) {
        const year = dateParts[2];
        const month = dateParts[1] - 1;
        const day = dateParts[0];

        const dob = new Date(year, month, day);
        dob.setFullYear(dob.getFullYear() + 4);
        const yyyy = dob.getFullYear();
        const mm = String(dob.getMonth() + 1).padStart(2, '0');
        const dd = String(dob.getDate()).padStart(2, '0');
        const formattedDate = `${yyyy}-${mm}-${dd}`;
        $('#dateOfRetirement').val(formattedDate);
        calculateResidualService();
    } else {
        console.error('Invalid date string.');
    }
}

function extensionOfService() {
    const prefix = $('#armyPrefix').val();
    const extension = $('#ExtnOfService').val();
    const dop = $('#dateOfPromotion').val();

    if (!prefix) {
        Swal.fire({
            title: 'Warning!',
            html: '<p style="font-size: 18px;">Please select Prefix.</p>',
            icon: 'warning',
        });
        return;
    }

    if (prefix == 13 || prefix == 14) {
        if (extension == "Yes") {
            if (!dop || dop.trim() === "") {
                const currentRetDate = $('#dateOfRetirement').val();
                if (!currentRetDate) {
                    Swal.fire({
                        title: 'Error!',
                        html: '<p style="font-size: 18px;">Current retirement date is required for extension calculation.</p>',
                        icon: 'error',
                    });
                    return;
                }

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

                    Swal.fire({
                        title: 'Error!',
                        html: '<p style="font-size: 18px;">Invalid retirement date format.</p>',
                        icon: 'error',
                    });
                }
            } else {
                console.log('DoP is set:', dop);
            }
        } else if (!dop || dop.trim() === "") {
            if (globleRetirementDate?.value) {
                $('#dateOfRetirement').val(globleRetirementDate.value);
                calculateResidualService();
            } else {
                console.error('Global retirement date not available');
            }
        } else {
            console.log('DoP is set:', dop);
        }

    } else {
        console.log('Prefix does not qualify for extension (not 13 or 14)');
    }
}

function isValidDate(dateString) {
    if (!dateString) return false;
    const regex = /^\d{4}-\d{2}-\d{2}$/;
    if (!regex.test(dateString)) return false;

    const date = new Date(dateString);
    const timestamp = date.getTime();

    if (typeof timestamp !== 'number' || Number.isNaN(timestamp)) {
        return false;
    }

    return dateString === date.toISOString().split('T')[0];
}
function ExtensionOfServiceAccess() {
    const prefix = $('#armyPrefix').val();
    const yearOfService = parseFloat($('#residualService').val());
    let extensionDropdown = $('#ExtnOfService');
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
            return response.json();
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

    const selectedDate = new Date(my_date(value));
    const currentDate = new Date();
    const threeMonthsAgo = new Date(currentDate.getFullYear(), currentDate.getMonth() - 3, 1);

    const normalizedSelectedDate = new Date(selectedDate.getFullYear(), selectedDate.getMonth(), 1);
    const normalizedCurrentMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);

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
    const cr = parseValue($('#basicPay').val().replace(/,/g, ''))
        + parseValue($('#rank_gradePay').val().replace(/,/g, ''))
        + parseValue($('#msp').val().replace(/,/g, ''))
        + parseValue($('#CI_Pay').val().replace(/,/g, ''))
        + parseValue($('#npax_Pay').val().replace(/,/g, ''))
        + parseValue($('#techPay').val().replace(/,/g, ''))
        + parseValue($('#da').val().replace(/,/g, ''))
        + parseValue($('#pmha').val().replace(/,/g, ''))
        + parseValue($('#lra').val().replace(/,/g, ''))
        + parseValue($('#miscPay').val().replace(/,/g, ''));

    const debt = parseValue($('#dsop_afpp').val().replace(/,/g, ''))
        + parseValue($('#agif_Subs').val().replace(/,/g, ''))
        + parseValue($('#incomeTaxMonthly').val().replace(/,/g, ''))
        + parseValue($('#educationCess').val().replace(/,/g, ''))
        + parseValue($('#pli').val().replace(/,/g, ''))
        + parseValue($('#misc_Deduction').val().replace(/,/g, ''))
        + parseValue($('#loanEMI_Outside').val().replace(/,/g, ''))
        + parseValue($('#loanEMI').val().replace(/,/g, ''));
    const totalDebit = debt.toLocaleString('en-IN');
    $('#totalDeductions').val(totalDebit);
    $('#totalCredit').val(cr.toLocaleString('en-IN'));
    $('#salary_After_Deductions').val((cr - debt).toLocaleString('en-IN'));
    setOutlineActive("totalDeductions");
    setOutlineActive("totalCredit");
    setOutlineActive("salary_After_Deductions");
    calculateEMIRepayingCapacity_CA();
    calculateEMIRepayingCapacity_HBA();
    calculateEMIRepayingCapacity_PCA();
    resetLoanFields();
}
function resetLoanFields() {
    $('#CA_approxDisbursementAmt,#HBA_approxDisbursementAmt,#PCA_approxDisbursementAmt,#computerCost,#propertyCost,#vehicleCost,#PCA_EMI_Eligible,#HBA_EMI_Eligible,#CA_EMI_Eligible,#HBA_Amount_Applied_For_Loan,#HBA_EMI_Applied,#CA_Amount_Applied_For_Loan,#CA_EMI_Applied,#PCA_Amount_Applied_For_Loan,#PCA_EMI_Applied,#PCA_approxEMIAmount,#CA_approxEMIAmount,#HBA_approxEMIAmount,#CA_Amt_Eligible_for_loan,#PCA_Amt_Eligible_for_loan,#HBA_Amt_Eligible_for_loan').val(0);
}

let formSubmitting = false;
let formCancelled = false;
function filterAmountText(loanType) {
    if (loanType == 2) {
        const VehicleCost = $('#vehicleCost');
        const cleanedValue = VehicleCost.val().replace(/,/g, '');
        VehicleCost.val(cleanedValue);
    }
}


function handleSubmitClick() {
    $("#btn-save").on("click", function (event) {
        event.preventDefault();
        const form = $("#myForm");
        const inputs = $('input[required], select[required]');
        form.find(".error").each(function () {
            $(this).text("");
        });

        let errorlist = [];
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
            const inputElement = this;

            const loanWrappers = {
                "1": ["#pcaAccordianWrapper", "#caAccordianWrapper"],
                "2": ["#pcaAccordianWrapper", "#hbaAccordianWrapper"],
                "3": ["#caAccordianWrapper", "#hbaAccordianWrapper"]
            };

            if (loanWrappers[loanType] && (
                $(loanWrappers[loanType][0]).find(input).length ||
                $(loanWrappers[loanType][1]).find(input).length
            )) {
                return;
            }


            if (!inputElement.checkValidity()) {

                input.addClass("is-invalid").removeClass("is-valid");

                const errorSpan = input.parent().find(".error");
                if (errorSpan.length) {
                    errorSpan.text(inputElement.validationMessage);
                }
                let errorText = input.attr("name");
                const prefixes = ["CommonData.", "HBAApplication.", "CarApplication.", "PCAApplication.", "AddressDetails.", "AccountDetails."];
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


        const errors = hasError ? "Error in: " + errorlist.join(", ") : "";
        $("#msgerror").html('<div class="alert alert-danger" role="alert">⚠️' + errors + ' </div>')

        if (hasError) {
            return false;
        }
        else {
            $("#msgerror").html('');
            if (formSubmitting) return;
            if (formCancelled) {
                formCancelled = false;
                e.preventDefault();
                return;
            }

            let unitVal = $('#PresenttxtUnit').val();
            if (unitVal && unitVal.trim() !== '') {
                event.preventDefault();
                checkCORegistration();
            }
        }
    });
}

function checkCORegistration() {
    const armyNumber = $("#armyPrefix option:selected").text();
    const Prefix = $("#armyNumber").val();
    const Suffix = $("#txtSuffix").val();
    const ArmyNo = `${armyNumber}${Prefix}${Suffix}`.toUpperCase();
    if (Prefix === "0" || armyNumber === "" || Suffix === "") {
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
                    $('#unitSearchDialog').removeClass('d-none');
                } else if (result === false) {
                    formSubmitting = true;
                    $('#myForm').submit();
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
    const value = $("#unitSearchInput").val()

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

    $('#unitSearchDialog').addClass('d-none');

    formSubmitting = false;
    formCancelled = true;
    const $message = $('#unitSearchMessage');
    if ($message.length) {
        $message.text('');
    }

    $(unitSearchInput).val('');
    $('#unitSearchConfirmBtn').prop('disabled', true);
});


function checkUnitSameOrNot(ArmyNo) {
    const armyNumber = $("#armyPrefix option:selected").text();
    const Prefix = $("#armyNumber").val();
    const Suffix = $("#txtSuffix").val();

    const Value = armyNumber + Prefix + Suffix;

    if (ArmyNo.toUpperCase() == Value.toUpperCase()) {
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
                        $('#myForm').submit();
                    } else if (result === false) {
                        $('#unitSearchMessage').text();
                        formSubmitting = false;
                        Swal.fire({
                            icon: 'info',
                            title: '<span style="font-size: 20px;">Unit Registration Pending/Not Activated</span>',
                            html: '<span style="font-size: 18px;">Please approach your IO/Superior ath for Regn or contact AGIF.</span>',
                            confirmButtonText: 'OK',
                            cancelButtonText: 'Cancel',
                            showCancelButton: true,
                            reverseButtons: true,
                        }).then((result) => {
                            if (result.isConfirmed) {
                                $('#unitSearchDialog').addClass('d-none');
                            } else if (result.isDismissed) {
                                $("unitSearchDialog").removeClass('d-none');
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

function RefreshMaxAmt_PCA() {
    $("#computerCost").on('change', function () {
        const rawValue = $('#computerCost').val().replace(/,/g, '');
        let PcCost = parseFloat(rawValue);

        PcCost = Math.round(PcCost * 0.9);
        if (isNaN(PcCost)) {
            alert("Please enter a valid computer cost.");
            return;
        }
        const Amount = 200000;
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
    const credit = parseInt($('#totalCredit').val().replace(/,/g, ''));
    const debit = parseInt($('#totalDeductions').val().replace(/,/g, ''));
    if (isNaN(credit) || isNaN(debit)) {
        alert("Please enter valid credit and debit amounts.");
        return;
    }
    let repayingCapacity = credit * 0.75 - debit;
    if (repayingCapacity < 0) {
        repayingCapacity = 0;
    }
    $('#PCA_repayingCapacity').val(repayingCapacity.toLocaleString('en-IN'));
    setOutlineActive("PCA_repayingCapacity");
}
function calculateMaxEMI_PCA() {

    let Residual = parseInt($('#totalResidualMonth').val().trim()) || 0;
    Residual -= 6;

    const EMI = 48;
    if (EMI < Residual) {
        $("#PCA_EMI_Eligible").val(EMI);
    }
    else {
        $("#PCA_EMI_Eligible").val(Residual);
    }

    setOutlineActive("PCA_EMI_Eligible");
}
function validateAmount_PCA(input) {
    const $input = $(input);
    const enteredAmount = parseFloat($input.val().replace(/,/g, ''));
    const eligibleAmount = parseFloat($("#PCA_Amt_Eligible_for_loan").val().replace(/,/g, ''));

    let disbusermentAmt = enteredAmount * 0.99;

    if (isNaN(enteredAmount)) {
        $input.val('');
    } else if (enteredAmount > eligibleAmount) {
        $input.val($("#PCA_Amt_Eligible_for_loan").val());
        disbusermentAmt = parseFloat($("#PCA_Amt_Eligible_for_loan").val().replace(/,/g, '')) * 0.99;
    }
    $('#PCA_approxDisbursementAmt').val(disbusermentAmt.toLocaleString('en-IN'));
    setOutlineActive("PCA_approxDisbursementAmt");
}

function validateEMI_PCA(input) {
    const $input = $(input);
    const enteredEMI = parseFloat($input.val());
    const eligibleEMI = parseFloat($("#PCA_EMI_Eligible").val());

    if (isNaN(enteredEMI)) {
        $input.val('');
    } else if (enteredEMI > eligibleEMI) {
        $input.val(eligibleEMI);
    }

}
function calculateEMI_PCA() {
    let P = parseFloat($("#PCA_Amount_Applied_For_Loan").val().replace(/[^0-9.]/g, ''));
    let N = parseInt($("#PCA_EMI_Applied").val());
    let R = (8.50 / 12) / 100;

    if (!P || !N || !R) {
        $("#PCA_approxEMIAmount").val("");
        return;
    }

    let EMI = (P * R * Math.pow(1 + R, N)) / (Math.pow(1 + R, N) - 1);
    $("#PCA_approxEMIAmount").val(Number(EMI.toFixed(0)).toLocaleString('en-IN'));

    setOutlineActive("PCA_approxEMIAmount");
}

function RefreshMaxAmt_CA() {
    $("#vehicleCost").on('change', function () {
        const vehicalType = $('#veh_Loan_Type').val();
        const CarEngineType = $('#VehTypeId').val();
        const rawValue = $('#vehicleCost').val().replace(/,/g, '');
        let carCost = parseFloat(rawValue);
        const prefix = $('#armyPrefix').val();
        carCost = Math.round(carCost * 0.9);
        let Amount = 0;
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
    const credit = parseInt($('#totalCredit').val().replace(/,/g, ''));
    const debit = parseInt($('#totalDeductions').val().replace(/,/g, ''));
    if (isNaN(credit) || isNaN(debit)) {
        alert("Please enter valid credit and debit amounts.");
        return;
    }
    let repayingCapacity = credit * 0.75 - debit;
    if (repayingCapacity < 0) {
        repayingCapacity = 0;
    }
    $('#CA_repayingCapacity').val(repayingCapacity.toLocaleString('en-IN'));

    setOutlineActive("CA_repayingCapacity");
}
function calculateMaxEMI_CA(vehicalType) {

    let Residual = parseInt($('#totalResidualMonth').val().trim()) || 0;
    Residual -= 6;
    const freqOfLoan = parseInt($('#CA_LoanFreq').val().trim()) || 0;
    let EMI = 0;
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
    const $input = $(input);
    let enteredAmount = parseFloat($input.val().replace(/,/g, ''));
    const eligibleAmount = parseFloat($("#CA_Amt_Eligible_for_loan").val().replace(/,/g, ''));
    let disbusermentAmt = enteredAmount * 0.99;

    if (isNaN(enteredAmount)) {
        $input.val('');
    } else if (enteredAmount > eligibleAmount) {
        $input.val($("#CA_Amt_Eligible_for_loan").val());
        disbusermentAmt = parseFloat($("#CA_Amt_Eligible_for_loan").val().replace(/,/g, '')) * 0.99;

    }

    $('#CA_approxDisbursementAmt').val(disbusermentAmt.toLocaleString('en-IN'));

    setOutlineActive("CA_approxDisbursementAmt");
}

function validateEMI_CA(input) {
    const $input = $(input);
    let enteredEMI = parseFloat($input.val());
    const eligibleEMI = parseFloat($("#CA_EMI_Eligible").val());

    if (isNaN(enteredEMI)) {
        $input.val('');
    } else {
        if (enteredEMI > eligibleEMI) {
            $input.val(eligibleEMI);
        }
    }
}

function calculateEMI_CA() {
    let P = parseFloat($("#CA_Amount_Applied_For_Loan").val().replace(/[^0-9.]/g, ''));
    let N = parseInt($("#CA_EMI_Applied").val());
    let R = (8.50 / 12) / 100;

    if (!P || !N || !R) {
        $("#CA_approxEMIAmount").val("");
        return;
    }

    let EMI = (P * R * Math.pow(1 + R, N)) / (Math.pow(1 + R, N) - 1);
    $("#CA_approxEMIAmount").val(Number(EMI.toFixed(0)).toLocaleString('en-IN'));

    setOutlineActive("CA_approxEMIAmount");
}

function RefreshMaxAmt_HBA() {
    $("#propertyCost").on('change', function () {
        const propType = $('#propertyType').val();
        if (!propType || propType == "0") {
            alert("Please select Property Type.");
            return;
        }

        const rawValue = $('#propertyCost').val().replace(/,/g, '');
        let propertyCost = parseFloat(rawValue);

        propertyCost = Math.round(propertyCost * 0.85);
        if (isNaN(propertyCost)) {
            alert("Please enter a valid property cost.");
            return;
        }

        let Amount; // ✅ declared once

        if (propType == 5) {
            Amount = 2000000;
        } else {
            const prefix = $('#armyPrefix').val();
            if (!prefix || prefix == "0") {
                alert("Please select Prefix.");
                return;
            }

            if (prefix == 13) {
                Amount = 5000000;
            } else if (prefix == 14) {
                Amount = 4000000;
            } else {
                Amount = 10000000;
            }
        }

        // ✅ use Amount in one place
        if (Amount > propertyCost) {
            $('#HBA_Amt_Eligible_for_loan').val(propertyCost.toLocaleString('en-IN'));
        } else {
            $('#HBA_Amt_Eligible_for_loan').val(Amount.toLocaleString('en-IN'));
        }

        setOutlineActive("HBA_Amt_Eligible_for_loan");
        calculateMaxEMI_HBA(propType);
        calculateEMIRepayingCapacity_HBA();
    });
}

function calculateEMIRepayingCapacity_HBA() {
    const credit = parseInt($('#totalCredit').val().replace(/,/g, ''));
    const debit = parseInt($('#totalDeductions').val().replace(/,/g, ''));
    if (isNaN(credit) || isNaN(debit)) {
        alert("Please enter valid credit and debit amounts.");
        return;
    }
    let repayingCapacity = credit * 0.75 - debit;
    if (repayingCapacity < 0) {
        repayingCapacity = 0;
    }
    $('#HBA_repayingCapacity').val(repayingCapacity.toLocaleString('en-IN'));
    setOutlineActive("HBA_repayingCapacity");
}
function calculateMaxEMI_HBA(propType) {
    let Residual = parseInt($('#totalResidualMonth').val().trim()) || 0;
    Residual -= 6;

    let EMI = 0;

    if (propType == 5) {
        EMI = 120;
    } else {
        EMI = 240;
    }

    $("#HBA_EMI_Eligible").val(EMI < Residual ? EMI : Residual);

    setOutlineActive("HBA_EMI_Eligible");
}
function validateAmount_HBA(input) {
    const $input = $(input);
    let enteredAmount = parseFloat($input.val().replace(/,/g, ''));
    const eligibleAmount = parseFloat($("#HBA_Amt_Eligible_for_loan").val().replace(/,/g, ''));
    let disbusermentAmt = enteredAmount * 0.99;

    if (isNaN(enteredAmount)) {
        $input.val('');
    } else if (enteredAmount > eligibleAmount) {
        $input.val($("#HBA_Amt_Eligible_for_loan").val());
        disbusermentAmt = parseFloat($("#HBA_Amt_Eligible_for_loan").val().replace(/,/g, '')) * 0.99;
    }


    $('#HBA_approxDisbursementAmt').val(disbusermentAmt.toLocaleString('en-IN'));
    setOutlineActive("HBA_approxDisbursementAmt");
}

function validateEMI_HBA(input) {
    const $input = $(input);
    let enteredEMI = parseFloat($input.val());
    const eligibleEMI = parseFloat($("#HBA_EMI_Eligible").val());

    if (isNaN(enteredEMI)) {
        $input.val('');
    } else {
        if (enteredEMI > eligibleEMI) {
            $input.val(eligibleEMI);
        }
    }
}

function calculateEMI_HBA() {
    let P = parseFloat($("#HBA_Amount_Applied_For_Loan").val().replace(/[^0-9.]/g, ''));
    let N = parseInt($("#HBA_EMI_Applied").val());
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
    const hbaApproxEmi = parseFloat($("#HBA_approxEMIAmount").val().replace(/,/g, ''));
    const hbaEmiRepayingCapacity = parseFloat($("#HBA_repayingCapacity").val().replace(/,/g, ''));
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
        $("input[name='ParentUnit']").val(0);

        if (request.term.length > 2) {
            const param = { "UnitName": request.term };
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
                        $("#ParenttxtUnit").addClass("is-invalid");
                        $("input[name='CommonData.ParentUnit']").addClass("is-invalid");
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
        $("#ParenttxtUnit").removeClass("is-invalid").addClass("is-valid");
        $("input[name='CommonData.ParentUnit']").removeClass("is-invalid").addClass("is-valid");

        $("#ParenttxtUnit").closest('.form-outline').find('.text-danger').hide();
    },
    appendTo: '#suggesstion-box'
});

$("#PresenttxtUnit").autocomplete({
    source: function (request, response) {
        $("input[name='PresentUnit']").val(0);

        if (request.term.length > 2) {
            const param = { "UnitName": request.term };
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
                        $("#PresenttxtUnit").addClass("is-invalid");
                        $("input[name='CommonData.PresentUnit']").addClass("is-invalid");
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
        $("input[name='CommonData.PresentUnit']").val(i.item.value);

        $("#PresenttxtUnit").removeClass("is-invalid").addClass("is-valid");
        $("input[name='CommonData.PresentUnit']").removeClass("is-invalid").addClass("is-valid");

        $("#PresenttxtUnit").closest('.form-outline').find('.text-danger').hide();
        CheckIsCoRegister(i.item.value, i.item.label)

    },
    appendTo: '#suggesstion-box'
});

function CheckIsCoRegister(UnitId, UnitName) {
    const param = { "UnitId": UnitId };
    $("#PresentUnitId").val(0);
    $.ajax({
        url: '/Account/CheckIsCoRegister',
        contentType: 'application/x-www-form-urlencoded',
        data: param,
        type: 'POST',
        success: function (data) {

            if (data == 1) {
                $("#PresenttxtUnit").val(UnitName);
                $("#PresentUnitId").val(UnitId);
            } else {
                $("#PresenttxtUnit").val("");
                $("#PresentUnitId").val(0);
                Swal.fire({
                    icon: "error",
                    title: "Unit Cdr Not Registered on AGIF Web Appl",
                    text: "Please approach UNIT CDR to first Register on AGIF Web Appl.",
                }).then(() => {
                });
            }
        }
    });
}
$('#oldArmyNo').on('focus', function () {
    $(this).off('focus');
    Swal.fire({
        title: "Enter Present 'Army No'",
        text: "If 'Old Army No' is not applicable for you.",
        icon: "warning",
        confirmButtonText: "OK"
    });
});

function validateEMIForRepayingCapacity(prefix) {
    const approxEmi = parseFloat($(`#${prefix}_approxEMIAmount`).val().replace(/,/g, ''));
    const repayingCapacity = parseFloat($(`#${prefix}_repayingCapacity`).val().replace(/,/g, ''));

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
$("#ParenttxtUnit").on('input', function () {
    if ($(this).val() === '') {
        $("input[name='CommonData.ParentUnit']").val('');
        $("#ParentUnitId").val(0);

    }
});
$("#PresenttxtUnit").on('input', function () {
    if ($(this).val() === '') {
        $("input[name='CommonData.PresentUnit']").val('');
        $("#PresentUnitId").val(0);
    }
});


$('input[required], select[required]').on('input change blur', function () {
    const input = $(this);
    const inputElement = this;

    if (inputElement.checkValidity()) {
        input.removeClass("is-invalid").addClass("is-valid");

        input.closest('.form-outline').find('.text-danger').hide();
    } else {
        input.removeClass("is-valid").addClass("is-invalid");

        input.closest('.form-outline').find('.text-danger').show();
    }
});

$("input, textarea").on("paste", function (e) {
    e.preventDefault();
});


function setInputValueWithFloatingLabel(inputId, value) {
    const $input = $('#' + inputId);

    if (!$input.length) return; // Exit if element not found

    // Set the value
    $input.val(value);

    $input.addClass('active');
}
function findDataWithArmyNumber() {
    $('#armyNumber').on('blur', function () {
        const armyNumber = $('#armyNumber').val().trim();
        const armyPrefix = $('#armyPrefix').val().trim();
        const armySuffix = $('#txtSuffix').val().trim();

        // Validate required fields
        if (!armyPrefix) {
            alert('Please select an Army Prefix.');
            $('#armyPrefix').focus();
            return;
        }

        if (!armyNumber) {
            alert('Army Number is required.');
            $('#armyNumber').focus();
            return;
        }

        if (!armySuffix) {
            alert('Army Suffix is required.');
            $('#txtSuffix').focus();
            return;
        }

        const fullArmyNumber = `${armyPrefix}-${armyNumber}-${armySuffix}`.toUpperCase();
        if (fullArmyNumber) {
            $.ajax({
                url: '/OnlineApplication/GetDataByArmyNumber',
                type: 'GET',
                data: { ArmyNo: fullArmyNumber },
                success: function (data) {
                    if (data) {

                        setInputValueWithFloatingLabel('txtApplicantName', data.applicantName);
                        setInputValueWithFloatingLabel('armyNumber', data.number);
                        setInputValueWithFloatingLabel('txtSuffix', data.suffix);
                        setInputValueWithFloatingLabel('txtApplicantName', data.applicantName);
                        setInputValueWithFloatingLabel('oldArmyNo', data.oldNumber);
                        setInputValueWithFloatingLabel('txtOldSuffix', data.oldSuffix);
                        setInputValueWithFloatingLabel('aadharCardNo', data.aadharCardNo);
                        setInputValueWithFloatingLabel('panCardNo', data.panCardNo);
                        setInputValueWithFloatingLabel('mobileNo', data.mobileNo);
                        setInputValueWithFloatingLabel('emailId', data.email);
                        setInputValueWithFloatingLabel('totalService', data.totalService);
                        setInputValueWithFloatingLabel('residualService', data.residualService);
                        //Unit Details
                        setInputValueWithFloatingLabel('pcda_pao', data.pcda_pao);
                        setInputValueWithFloatingLabel('pcda_AcctNo', data.pcda_AcctNo);
                        setInputValueWithFloatingLabel('presentUnitPin', data.presentUnitPin);
                        setInputValueWithFloatingLabel('civilPostalAddress', data.civilPostalAddress);
                        setInputValueWithFloatingLabel('nextFmnHQ', data.nextFmnHQ);
                        //Permanent Address Details
                        setInputValueWithFloatingLabel('Vill_Town', data.vill_Town);
                        setInputValueWithFloatingLabel('postOffice', data.postOffice);
                        setInputValueWithFloatingLabel('distt', data.distt);
                        setInputValueWithFloatingLabel('state', data.state);
                        setInputValueWithFloatingLabel('Code', data.code);
                        //Salary Account Details
                        setInputValueWithFloatingLabel('salaryAcctNo', data.salaryAcctNo);
                        setInputValueWithFloatingLabel('confirmSalaryAcctNo', data.confirmSalaryAcctNo);
                        setInputValueWithFloatingLabel('ifsCode', data.ifsCode);
                        setInputValueWithFloatingLabel('nameOfBank', data.nameOfBank);
                        setInputValueWithFloatingLabel('nameOfBankBranch', data.nameOfBankBranch);
                        //salary details
                        //setInputValueWithFloatingLabel('basicPay', data.basicPay);
                        //setInputValueWithFloatingLabel('rank_gradePay', data.rank_gradePay);
                        //setInputValueWithFloatingLabel('msp', data.msp);
                        //setInputValueWithFloatingLabel('npax_Pay', data.npax_Pay);
                        //setInputValueWithFloatingLabel('da', data.da);
                        //setInputValueWithFloatingLabel('miscPay', data.miscPay);
                        //setInputValueWithFloatingLabel('pli', data.pli);
                        //setInputValueWithFloatingLabel('agif_Subs', data.agif_Subs);
                        //setInputValueWithFloatingLabel('incomeTaxMonthly', data.incomeTaxMonthly);
                        //setInputValueWithFloatingLabel('educationCess', data.educationCess);
                        //setInputValueWithFloatingLabel('loanEmi', data.loanEmi);
                        //setInputValueWithFloatingLabel('loanEMI_Outside', data.loanEMI_Outside);
                        //setInputValueWithFloatingLabel('misc_Deduction', data.misc_Deduction);
                        //setInputValueWithFloatingLabel('totalCredit', data.totalCredit);
                        //setInputValueWithFloatingLabel('salary_After_Deductions', data.salary_After_Deductions);
                        //setInputValueWithFloatingLabel('cI_Pay', data.cI_Pay);
                        //setInputValueWithFloatingLabel('techPay', data.techPay);
                        //setInputValueWithFloatingLabel('pmha', data.pmha);
                        //setInputValueWithFloatingLabel('lra', data.lra);
                        //setInputValueWithFloatingLabel('dsop_afpp', data.dsop_afpp);
                        //setInputValueWithFloatingLabel('totalDeductions', data.totalDeductions);



                        $('#oldArmyPrefix').val(data.oldArmyPrefix).trigger('change');
                        $('#ddlrank').val(data.rankId).trigger('change');
                        $('#regtCorps').val(data.regtCorpsId).trigger('change');
                        $('#armyPostOffice').val(data.armyPostOfficeId).trigger('change');
                        $('#emailDomain').val(data.emailDomain).trigger('change');
                        //const formattedDOB = formatDateToDDMMYYYY(data.dateOfBirth);
                        //setInputValueWithFloatingLabel('dateOfBirth', formattedDOB);
                        ////$('#dateOfBirth').val(formatDateToDDMMYYYY(data.dateOfBirth));
                        ////$('#dateOfCommission').val(formatDateToDDMMYYYY(data.dateOfCommission));
                        //const formattedDOC = formatDateToDDMMYYYY(data.dateOfCommission);
                        //setInputValueWithFloatingLabel('dateOfCommission', formattedDOC);
                        ////$('#dateOfPromotion').val(formatDateToDDMMYYYY(data.dateOfPromotion));
                        //const formattedDOP = formatDateToDDMMYYYY(data.dateOfPromotion);
                        //setInputValueWithFloatingLabel('dateOfPromotion', formattedDOP);
                        ////$('#dateOfRetirement').val(formatDateToDDMMYYYY(data.dateOfRetirement));
                        //const dateOfRetirement = formatDateToDDMMYYYY(data.dateOfRetirement);
                        //console.log(dateOfRetirement);
                        //setInputValueWithFloatingLabel('dateOfRetirement', dateOfRetirement);

                        console.log(data);
                    }
                    else {
                        console.log("Data not found for the provided Army Number.");
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error fetching data:", error);
                }
            });
        }
    });
}

function findDataWithApplicationId() {

    const applicationid = $('#applicationid').val();

    if (applicationid!= 0) {
            $.ajax({
                url: '/OnlineApplication/GetDataByApplicationId',
                type: 'GET',
                data: { applicationid: applicationid },
                success: function (data) {
                    if (data) {
                        
                        setInputValueWithFloatingLabel('txtApplicantName', data.applicantName);
                        setInputValueWithFloatingLabel('armyNumber', data.number);
                        setInputValueWithFloatingLabel('txtSuffix', data.suffix);
                        setInputValueWithFloatingLabel('txtApplicantName', data.applicantName);
                        setInputValueWithFloatingLabel('oldArmyNo', data.oldNumber);
                        setInputValueWithFloatingLabel('txtOldSuffix', data.oldSuffix);
                        setInputValueWithFloatingLabel('aadharCardNo', data.aadharCardNo);
                        setInputValueWithFloatingLabel('panCardNo', data.panCardNo);
                        setInputValueWithFloatingLabel('mobileNo', data.mobileNo);
                        setInputValueWithFloatingLabel('emailId', data.email);
                        setInputValueWithFloatingLabel('totalService', data.totalService);
                        setInputValueWithFloatingLabel('residualService', data.residualService);
                        //Unit Details
                        setInputValueWithFloatingLabel('pcda_pao', data.pcda_pao);
                        setInputValueWithFloatingLabel('pcda_AcctNo', data.pcda_AcctNo);
                        setInputValueWithFloatingLabel('presentUnitPin', data.presentUnitPin);
                        setInputValueWithFloatingLabel('civilPostalAddress', data.civilPostalAddress);
                        setInputValueWithFloatingLabel('nextFmnHQ', data.nextFmnHQ);
                        //Permanent Address Details
                        setInputValueWithFloatingLabel('Vill_Town', data.vill_Town);
                        setInputValueWithFloatingLabel('postOffice', data.postOffice);
                        setInputValueWithFloatingLabel('distt', data.distt);
                        setInputValueWithFloatingLabel('state', data.state);
                        setInputValueWithFloatingLabel('Code', data.code);
                        //Salary Account Details
                        setInputValueWithFloatingLabel('salaryAcctNo', data.salaryAcctNo);
                        setInputValueWithFloatingLabel('confirmSalaryAcctNo', data.confirmSalaryAcctNo);
                        setInputValueWithFloatingLabel('ifsCode', data.ifsCode);
                        setInputValueWithFloatingLabel('nameOfBank', data.nameOfBank);
                        setInputValueWithFloatingLabel('nameOfBankBranch', data.nameOfBankBranch);
                        //salary details
                        //setInputValueWithFloatingLabel('basicPay', data.basicPay);
                        //setInputValueWithFloatingLabel('rank_gradePay', data.rank_gradePay);
                        //setInputValueWithFloatingLabel('msp', data.msp);
                        //setInputValueWithFloatingLabel('npax_Pay', data.npax_Pay);
                        //setInputValueWithFloatingLabel('da', data.da);
                        //setInputValueWithFloatingLabel('miscPay', data.miscPay);
                        //setInputValueWithFloatingLabel('pli', data.pli);
                        //setInputValueWithFloatingLabel('agif_Subs', data.agif_Subs);
                        //setInputValueWithFloatingLabel('incomeTaxMonthly', data.incomeTaxMonthly);
                        //setInputValueWithFloatingLabel('educationCess', data.educationCess);
                        //setInputValueWithFloatingLabel('loanEmi', data.loanEmi);
                        //setInputValueWithFloatingLabel('loanEMI_Outside', data.loanEMI_Outside);
                        //setInputValueWithFloatingLabel('misc_Deduction', data.misc_Deduction);
                        //setInputValueWithFloatingLabel('totalCredit', data.totalCredit);
                        //setInputValueWithFloatingLabel('salary_After_Deductions', data.salary_After_Deductions);
                        //setInputValueWithFloatingLabel('cI_Pay', data.cI_Pay);
                        //setInputValueWithFloatingLabel('techPay', data.techPay);
                        //setInputValueWithFloatingLabel('pmha', data.pmha);
                        //setInputValueWithFloatingLabel('lra', data.lra);
                        //setInputValueWithFloatingLabel('dsop_afpp', data.dsop_afpp);
                        //setInputValueWithFloatingLabel('totalDeductions', data.totalDeductions);


                        $('#armyPrefix').val(data.armyPrefix).trigger('change');
                        $('#oldArmyPrefix').val(data.oldArmyPrefix).trigger('change');
                        $('#ddlrank').val(data.rankId).trigger('change');
                        $('#regtCorps').val(data.regtCorpsId).trigger('change');
                        $('#armyPostOffice').val(data.armyPostOfficeId).trigger('change');
                        $('#emailDomain').val(data.emailDomain).trigger('change');
                        //const formattedDOB = formatDateToDDMMYYYY(data.dateOfBirth);
                        //setInputValueWithFloatingLabel('dateOfBirth', formattedDOB);
                        ////$('#dateOfBirth').val(formatDateToDDMMYYYY(data.dateOfBirth));
                        ////$('#dateOfCommission').val(formatDateToDDMMYYYY(data.dateOfCommission));
                        //const formattedDOC = formatDateToDDMMYYYY(data.dateOfCommission);
                        //setInputValueWithFloatingLabel('dateOfCommission', formattedDOC);
                        ////$('#dateOfPromotion').val(formatDateToDDMMYYYY(data.dateOfPromotion));
                        //const formattedDOP = formatDateToDDMMYYYY(data.dateOfPromotion);
                        //setInputValueWithFloatingLabel('dateOfPromotion', formattedDOP);
                        ////$('#dateOfRetirement').val(formatDateToDDMMYYYY(data.dateOfRetirement));
                        //const dateOfRetirement = formatDateToDDMMYYYY(data.dateOfRetirement);
                        //console.log(dateOfRetirement);
                        //setInputValueWithFloatingLabel('dateOfRetirement', dateOfRetirement);

                        console.log(data);
                    }
                    else {
                        console.log("Data not found for the provided Army Number.");
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error fetching data:", error);
                }
            });
        }
}
function formatDateToDDMMYYYY(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);

    if (isNaN(date)) return ''; // invalid date check

    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();

    return `${day}/${month}/${year}`;
}
