document.addEventListener('DOMContentLoaded', function () {
    //const tillDateInput = document.getElementById('tillDate');
    //const today = new Date();
    //const lastMonth = new Date(today.getFullYear(), today.getMonth() - 1, today.getDate());

    //// Format date as YYYY-MM-DD for input field
    //const formattedDate = lastMonth.toISOString().split('T')[0];
    //tillDateInput.value = formattedDate;
    setTillDateToPreviousMonth();
  

    // Set focus to date of joining field
    //    document.getElementById('dateOfBirth').focus();
});

// Form validation
document.getElementById('loanForm').addEventListener('submit', function (e) {
    const commissionDate = new Date(document.getElementById('commissionDate').value);
    const tillDate = new Date(document.getElementById('tillDate').value);

    if (commissionDate > tillDate) {
        e.preventDefault();
        alert('Commission Date cannot be later than Till Date');
        return false;
    }
});

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
function my_date(date_string) {
    const date_components = date_string.split("/");
    const day = date_components[0];
    const month = date_components[1];
    const year = date_components[2];
    return new Date(year, month - 1, day);
}
//function setTillDateToPreviousMonth() {
//    const today = new Date();
//    const previousMonth = new Date(today);

//    // Go to previous month
//    previousMonth.setMonth(today.getMonth() - 1);

//    // If we're in January, setMonth(-1) will correctly go to December of previous year
//    // Set to the 1st day of the previous month
//    previousMonth.setDate(1);

//    // Format as YYYY-MM-DD for date input
//    const formattedTillDate = previousMonth.toISOString().split('T')[0];

//    const tillDateInput = document.getElementById('tillDate');
//    tillDateInput.value = formattedTillDate;
//}
//function setTillDateToPreviousMonth() {
//    const today = new Date();

//    // Create date for last day of previous month
//    // new Date(year, month, 0) gives last day of the previous month
//    const lastDayOfPreviousMonth = new Date(today.getFullYear(), today.getMonth(), 0);

//    // Format as YYYY-MM-DD for date input
//    const formattedTillDate = lastDayOfPreviousMonth.toISOString().split('T')[0];
//    const tillDateInput = document.getElementById('tillDate');
//    tillDateInput.value = formattedTillDate;
//}
function setTillDateToPreviousMonth() {
    const today = new Date();
    const lastDayOfPreviousMonth = new Date(today.getFullYear(), today.getMonth(), 0);

    // Use local date methods to avoid timezone conversion
    const year = lastDayOfPreviousMonth.getFullYear();
    const month = String(lastDayOfPreviousMonth.getMonth() + 1).padStart(2, '0');
    const day = String(lastDayOfPreviousMonth.getDate()).padStart(2, '0');

    const formattedTillDate = `${year}-${month}-${day}`;
    const tillDateInput = document.getElementById('tillDate');
    tillDateInput.value = formattedTillDate;
}
//document.getElementById('calculateButton').addEventListener('click', function () {
//    const dt = document.getElementById('dateOfBirth').value; // Get the date input value

//    if (!dt) {
//        showErrorMessage("Please select a valid Date Of Joining");
//        return;
//    }
//    const categoryValue = $('#Category').val();
//    // Extract month and year from the selected date
//    const [day, month, year] = dt.split('/');
//    console.log(month, year);
//    // Make the AJAX request


//    //// Show full-screen loader
//    //$.LoadingOverlay("show", {
//    //    image: "",

//    //    fontawesome: "fa fa-spinner fa-spin", // Font Awesome spinner
//    //    //text: "Calculating maturity... please wait",
//    //    fontawesomeColor: "#28a745", // Green spinner

//    //    textResizeFactor: 0.8,
//    //    background: "rgba(0,0,0,0.4)"
//    //});


//    $.ajax({
//        url: '/EmiCalculator/Calculate', // The URL to your controller method
//        method: 'POST',
//        data: { month: month, year: year, categoryValue: categoryValue }, // Send month and year
//        success: function (response) {
//            //if (response.success) {
//            //    console.log("Maturity calculated:", response.value);
//            //    // You can update the page with the result
//            //    $('#result').html(`Maturity Value: ${response.value}`);
//            //} else {
//            //    console.log("Calculation failed:", response.message);
//            //    // Optionally, you can display the error message
//            //    $('#result').html("An error occurred while calculating the maturity.");
//            //}
//            //$.LoadingOverlay("hide");

//            if (response.success) {
//                console.log("Maturity calculated:", response.value);

//                //        // Show result in SweetAlert
//                //        Swal.fire({
//                //            title: '',
//                //            html: `
//                //    <div style="text-align: justify;">
//                //        <h3 style="color: #28a745; margin-bottom: 15px;">ASSUMING ALL YOUR PREMIUMS HAVE BEER RECEVIVED AT AGIF, YOUR APPROX ACCUMULATION IN MATURITY  TILL PREVIOUS MONTH IS</h3>
//                //        <div style="font-size: 2rem; font-weight: bold; color: #2c3e50; margin: 20px 0;">
//                //            ₹${parseFloat(response.value).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
//                //        </div>

//                //    </div>
//                //`,
//                //            icon: 'success',
//                //            confirmButtonText: 'Great!',
//                //            confirmButtonColor: '#28a745',
//                //            showCloseButton: true,
//                //            timer: 20000, // Auto close after 20 seconds
//                //            timerProgressBar: true
//                //        });
//                Swal.fire({
//                    title: '', // Empty title as it's not needed
//                    html: `
//        <div style="text-align: center;">
//            <h3 style="color: #310; margin-bottom: 15px; font-size: 1.5rem; font-weight: bold;">
//                Assuming all your premiums have been received at AGIF, your approximate accumulation in maturity till previous month is
//            </h3>
//             <h3 style="color: #310; margin-bottom: 15px; font-size: 1.5rem; font-weight: bold;">
//                This is for serving Personals only and E.I. amount will be deducted from final maturity amount.
//            </h3>
//            <div style="font-size: 2.5rem; font-weight: bold; color: #2c3e50; margin: 20px 0;">
//                ₹${parseFloat(response.value).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
//            </div>
//        </div>
//    `,
//                    icon: 'success',
//                    confirmButtonText: 'OK',
//                    confirmButtonColor: '#28a745',
//                    showCloseButton: true,
//                    //timer: 20000, // Auto-close after 20 seconds
//                    //timerProgressBar: true,
//                    customClass: {
//                        popup: 'swal-popup',
//                        title: 'swal-title',
//                        content: 'swal-content',
//                        confirmButton: 'swal-confirm-btn',
//                        closeButton: 'swal-close-btn'
//                    }
//                });
//            } else {
//                // Handle error case
//                Swal.fire({
//                    title: 'Calculation Failed',
//                    text: 'Unable to calculate maturity amount. Please try again.',
//                    icon: 'error',
//                    confirmButtonColor: '#dc3545'
//                });
//            }
//        },
//        error: function (error) {
//            //$.LoadingOverlay("hide");

//            console.error("Error calculating maturity:", error);
//            Swal.fire({
//                title: 'Error',
//                text: 'Something went wrong while calculating maturity.',
//                icon: 'error',
//                confirmButtonColor: '#dc3545'
//            });
//            //console.error("Error calculating maturity:", error);
//        }
//    });
//});
// Detect change in dropdown and show the button when a value is selected
$('#Category').on('change', function () {
    const categoryValue = $(this).val(); // Get the selected value
    if (categoryValue) {
        $('#calculateButton').removeClass('d-none'); // Show the button
    } else {
        $('#calculateButton').addClass('d-none'); // Hide the button if no value is selected
    }
});
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
//const radioButtons = document.querySelectorAll('input[name="category"]');
//radioButtons.forEach(radio => {
//    radio.addEventListener('change', function () {
//        if (this.checked) {
//            // Small delay to show the selection animation
//            setTimeout(() => {
//                calculateMaturity();
//            }, 300);
//        }
//    });
//});

$('input[name="category"]').on('click', function () {

        const dt = document.getElementById('dateOfBirth').value; // Get the date input value

        if (!dt) {
            showErrorMessage("Please select a valid Date Of Joining");
            return;
    }

    const categoryValue = $('input[name="category"]:checked').val();
    console.log("Selected category value:", categoryValue);
        const [day, month, year] = dt.split('/');
        console.log(month, year);
    //    // Make the AJAX request


    //    //// Show full-screen loader
    //    //$.LoadingOverlay("show", {
    //    //    image: "",

    //    //    fontawesome: "fa fa-spinner fa-spin", // Font Awesome spinner
    //    //    //text: "Calculating maturity... please wait",
    //    //    fontawesomeColor: "#28a745", // Green spinner

    //    //    textResizeFactor: 0.8,
    //    //    background: "rgba(0,0,0,0.4)"
    //    //});


        $.ajax({
            url: '/EmiCalculator/Calculate', // The URL to your controller method
            method: 'POST',
            data: { month: month, year: year, categoryValue: categoryValue }, // Send month and year
            success: function (response) {

                if (response.success) {
                    console.log("Maturity calculated:", response.value);

                    let joiningDate = formatDateToReadable($('#dateOfBirth').val());
                    let tilldate = formatDateToReadable($('#tillDate').val());
                    Swal.fire({
                        title: '', // Empty title as it's not needed
                        html: `
             <div style="text-align: center; max-width: 600px; margin: 0 auto;">
        <!-- Joining Date Section -->
        <div style="border: 2px solid #28a745; border-radius: 8px; padding: 15px; margin-bottom: 10px; background-color: #f8fff9;">
            <h4 style="color: #28a745; margin: 0; font-size: 1.2rem; font-weight: 600;">
                📅 Your Month of Joining Indian Army is
            </h4>
            <div style="color: #2c3e50; font-size: 1.5rem; font-weight: bold; margin-top: 8px;">
                ${joiningDate}
            </div>
        </div>

        <!-- Information Section 1 -->
        <div style="border: 2px solid #007bff; border-radius: 8px; padding: 15px; margin-bottom: 10px; background-color: #f8f9ff;">
            <p style="color: #007bff; margin: 0; font-size: 1.1rem; font-weight: 600; line-height: 1.4;">
               💵 Contribution Made Presuming All Payments Deducted By CDA(O) And Paid to AGIF
            </p>
            <div style="font-size: 2.0rem; font-weight: bold; color: #2c3e50; margin: 0;">
                ₹${parseFloat(response.balCount).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
            </div>
        </div>

        <!-- Information Section 2 -->
        <div style="border: 2px solid #ffc107; border-radius: 8px; padding: 15px; margin-bottom: 10px; background-color: #fffef8;">
            <p style="color: #856404; margin: 0; font-size: 1.1rem; font-weight: 600; line-height: 1.4;">
                💵 Saving Element From Your Contributed Amount
            </p>
              <div style="font-size: 2.0rem; font-weight: bold; color: #2c3e50; margin: 0;">
                ₹${parseFloat(response.saveEL).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
            </div>
        </div>


        <!-- Amount Section -->
        <div style="border: 3px solid #2c3e50; border-radius: 12px; padding: 25px; background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%); box-shadow: 0 4px 8px rgba(0,0,0,0.1);margin-bottom: 10px;">
            <div style="color: #6c757d; margin: 0; font-size: 1.1rem; font-weight: 600; line-height: 1.4;">
               💰 Maturity Amount That Your Saving Element has Grown As on ${tilldate}
            </div>
            <div style="font-size: 2.0rem; font-weight: bold; color: #2c3e50; margin: 0;">
                ₹${parseFloat(response.currentBalance).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
            </div>
        </div>
          <!-- Note Section -->
        <div style="border: 2px solid #17a2b8; border-radius: 8px; padding: 15px; margin-bottom: 10px;  background-color: #f0f9ff; border-left: 5px solid #17a2b8;">
            <p style="color: #0c5460; margin: 0; font-size: 1rem; font-weight: 600; line-height: 1.4;">
                📝 <strong>NOTE:</strong> Tax free return for FY 2025-2026 is 8.7%
            </p>
        </div>
        
        <!-- Information Section 3 -->
        <div style="border: 2px solid #dc3545; border-radius: 8px; padding: 15px; margin-bottom: 10px; background-color: #fff8f8;">
            <p style="color: #dc3545; margin: 0; font-size: 1.1rem; font-weight: 600; line-height: 1.4;">
               📞 For officers with service in ranks, Please contact AGIF Helpline No. 01126148055 for maturity details
            </p>
        </div>
    </div>
        `,
                        icon: 'success',
                        confirmButtonText: 'OK',
                        confirmButtonColor: '#28a745',
                        showCloseButton: true,
                        //timer: 20000, // Auto-close after 20 seconds
                        //timerProgressBar: true,
                        customClass: {
                            popup: 'swal-popup',
                            title: 'swal-title',
                            content: 'swal-content',
                            confirmButton: 'swal-confirm-btn',
                            closeButton: 'swal-close-btn'
                        }
                    });
                } else {
                    // Handle error case
                    Swal.fire({
                        title: 'Calculation Failed',
                        text: 'Unable to calculate maturity amount. Please try again.',
                        icon: 'error',
                        confirmButtonColor: '#dc3545'
                    });
                }
            },
            error: function (error) {
                //$.LoadingOverlay("hide");

                console.error("Error calculating maturity:", error);
                Swal.fire({
                    title: 'Error',
                    text: 'Something went wrong while calculating maturity.',
                    icon: 'error',
                    confirmButtonColor: '#dc3545'
                });
                //console.error("Error calculating maturity:", error);
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
        return dateString; // Return original if formatting fails
    }

    return dateString;
}