$(document).ready(function () {
    setTillDateToPreviousMonth();

    $('#dateOfBirth').on('input', function () {
        formatDate(this);
    });

    $('#dateOfBirth').on('change', function () {
        validateDateFormat(this);
    });

    $('#commissionDate').on('input', function () {
        formatDate(this);
    });

    $('#commissionDate').on('change', function () {
        validateCommissionDate(this);
    });

    // Initialize datepickers
    initializeDatepickers();
});

function initializeDatepickers() {
    $('.monthPicker').datepicker({
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        dateFormat: 'dd/mm/yy',
        maxDate: 0,
        yearRange: "1900:+0",
        defaultDate: null,
        onSelect: function (dateText, inst) {
            formatDate(this);
            if (this.id === 'dateOfBirth') {
                validateDateFormat(this);
            } else if (this.id === 'commissionDate') {
                validateCommissionDate(this);
            }
        },
        beforeShowDay: function (date) {
            const today = new Date();
            today.setHours(23, 59, 59, 999);
            return [date <= today];
        }
    });
}

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
    setTillDateToPreviousMonth();
}

function validateCommissionDate(input) {
    const value = input.value;

    if (!value) {
        return;
    }

    const datePattern = /^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/(\d{4})$/;

    if (!datePattern.test(value)) {
        Swal.fire({
            icon: "error",
            title: "Invalid date",
            text: "Invalid date format. Please select a valid date.",
        });
        input.focus();
        input.value = "";
        return;
    }

    const enrolmentDate = $('#dateOfBirth').val();
    if (enrolmentDate) {
        const [enrolDay, enrolMonth, enrolYear] = enrolmentDate.split('/');
        const [commDay, commMonth, commYear] = value.split('/');

        const enrolDateObj = new Date(enrolYear, enrolMonth - 1, enrolDay);
        const commDateObj = new Date(commYear, commMonth - 1, commDay);

        if (commDateObj <= enrolDateObj) {
            Swal.fire({
                icon: "error",
                title: "Invalid Commission Date",
                text: "Commission date must be after the enrolment date.",
            });
            input.focus();
            input.value = "";
            return;
        }
    }
}

function my_date(date_string) {
    const date_components = date_string.split("/");
    const day = date_components[0];
    const month = date_components[1];
    const year = date_components[2];
    return new Date(year, month - 1, day);
}

function setTillDateToPreviousMonth() {
    const today = new Date();
    const lastDayOfPreviousMonth = new Date(today.getFullYear(), today.getMonth(), 0);

    const year = lastDayOfPreviousMonth.getFullYear();
    const month = String(lastDayOfPreviousMonth.getMonth() + 1).padStart(2, '0');
    const day = String(lastDayOfPreviousMonth.getDate()).padStart(2, '0');

    const formattedTillDate = `${year}-${month}-${day}`;
    const tillDateInput = document.getElementById('tillDate');
    tillDateInput.value = formattedTillDate;
}

function showErrorMessage(message) {
    const alertHtml = `
        <div class="alert alert-danger alert-dismissible fade show global-alert-box" role="alert">
            <i class="lni lni-cross-circle"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    $('body').append(alertHtml);

    setTimeout(function () {
        $('.alert-danger').fadeOut(300, function () {
            $(this).remove();
        });
    }, 2000);
}

// Handle rank group selection - SWAP POSITIONS
$('input[name="category"]').on('change', function () {
    const categoryValue = $(this).val();
    const dateFieldsContainer = $('#dateFieldsContainer');
    const commissionDateSection = $('#commissionDateSection');
    const rankGroupSection = $('#rankGroupSection');
    const loanForm = $('#loanForm');

    // Move date fields to top and rank group to bottom
    dateFieldsContainer.detach().prependTo(loanForm);
    rankGroupSection.detach().insertBefore(loanForm.find('.input-section').last());

    // Show date fields with animation
    dateFieldsContainer.slideDown(400);
    dateFieldsContainer.removeClass('hidden');
    // Reinitialize datepickers
    initializeDatepickers();

    // Show/hide commission date based on selection
    if (categoryValue === '1') { // Officers
        commissionDateSection.slideDown(300);
    } else { // JCO/OR
        commissionDateSection.slideUp(300);
        $('#commissionDate').val('');
    }
});

// Handle calculation when button is clicked
$('#calculateButton').on('click', function () {
    const dt = document.getElementById('dateOfBirth').value;

    if (!dt) {
        showErrorMessage("Please select a valid Enrolment/Start Date");
        return;
    }

    const categoryValue = $('input[name="category"]:checked').val();

    if (!categoryValue) {
        showErrorMessage("Please select a Rank Group");
        return;
    }

    const [day, month, year] = dt.split('/');

    const commissionDateValue = $('#commissionDate').val();
    let commissionMonth = null;
    let commissionYear = null;

    if (commissionDateValue) {
        const [commDay, commMonth, commYear] = commissionDateValue.split('/');
        commissionMonth = commMonth;
        commissionYear = commYear;
    }

    $.ajax({
        url: '/EmiCalculator/Calculate',
        method: 'POST',
        data: {
            month: month,
            year: year,
            categoryValue: categoryValue,
            commissionMonth: commissionMonth,
            commissionYear: commissionYear
        },
        success: function (response) {
            if (response.success) {
                let joiningDate = formatDateToReadable($('#dateOfBirth').val());
                let tilldate = formatDateToReadable($('#tillDate').val());

                Swal.fire({
                    title: '', // Empty title as it's not needed
                    html: `
             <div class="swal-center-container">
        <h3 class="swal-heading">
            Your month of Joining Indian Army is:
            <span class="swal-highlight">${joiningDate}</span>
        </h3>

        <h3 class="swal-heading">
            Assuming all your premiums have been received at AGIF, your approximate accumulation in maturity till previous month is
        </h3>

        <h3 class="swal-heading">
            This is for serving Personnel only and E.I. amount is included in final maturity benefit.
        </h3>

        <h3 class="swal-heading">
            For officers with service in ranks, Please contact AGIF Helpline No. 01126148055 for maturity details
        </h3>

        <div class="swal-amount">
            ₹${parseFloat(response.currentBalance).toLocaleString('en-IN', {
                        minimumFractionDigits: 2,
                        maximumFractionDigits: 2
                    })}
        </div>
    </div>
        `,
                    icon: 'success',
                    confirmButtonText: 'OK',
                    confirmButtonColor: '#28a745',
                    showCloseButton: true,
                    width: '700px',
                    customClass: {
                        popup: 'swal-popup',
                        title: 'swal-title',
                        content: 'swal-content',
                        confirmButton: 'swal-confirm-btn',
                        closeButton: 'swal-close-btn'
                    }
                });
            } else {
                Swal.fire({
                    title: 'Calculation Failed',
                    text: 'Unable to calculate maturity amount. Please try again.',
                    icon: 'error',
                    confirmButtonColor: '#dc3545'
                });
            }
        },
        error: function (error) {
            console.error("Error calculating maturity:", error);
            Swal.fire({
                title: 'Error',
                text: 'Something went wrong while calculating maturity.',
                icon: 'error',
                confirmButtonColor: '#dc3545'
            });
        }
    });
});

function formatDateToReadable(dateString) {
    if (!dateString) return '';

    try {
        let dateParts = dateString.split('/');
        if (dateParts.length === 3) {
            let day = parseInt(dateParts[0]);
            let month = parseInt(dateParts[1]) - 1;
            let year = parseInt(dateParts[2]);

            let dateObj = new Date(year, month, day);

            return dateObj.toLocaleDateString('en-GB', {
                day: 'numeric',
                month: 'long',
                year: 'numeric'
            });
        }
    } catch (error) {
        console.error('Error formatting date:', error);
        return dateString;
    }

    return dateString;
}