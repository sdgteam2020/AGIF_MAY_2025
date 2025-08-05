$(document).ready(function () {
    messageHandler();
    checkUploadFiles();

    $('.file-upload-limited').on('change', function () {
        const file = this.files[0];
        const errorrMessage = $(this).next('.file-error-message'); // container for error
        const previewSelector = $(this).data('preview');

        if (file && file.size > 150 * 1024) {
            errorrMessage.text('File size must not exceed 150 KB').css('color', 'red');
            this.value = ''; // Clear the input field
        } else {
            errorrMessage.text(''); // Clear the error message if file size is valid
            if (previewSelector) {
                previewFile(this, previewSelector);
            }
        }
    });

    

    // Function to preview the file (PDF or Image)
    function previewFile(input, previewSelector) {
        const file = input.files[0];
        const preview = $(previewSelector);
        const errorContainer = $(input).closest('.col-md-10').find('.file-error-message'); // Find the error container below the input
        const maxFileSize = 1 * 1024 * 1024; // 1MB in bytes
        errorContainer.text('');

        if (file) {
            // Check if the file is a PDF
            if (file.type !== 'application/pdf') {
                errorContainer.text('Only PDF files are allowed').css('color', 'red');
                input.value = ''; // Clear the input field
                return;
            }

            // Check the file size (must not exceed 1MB)
            if (file.size > maxFileSize) {
                errorContainer.text('File size must not exceed 1 MB').css('color', 'red');
                input.value = ''; // Clear the input field
            } else {
                const reader = new FileReader();

                reader.onload = function (e) {
                    // Create a link with the eye icon and the PDF file URL
                    const fileUrl = URL.createObjectURL(file); // Create a Blob URL for the file
                    preview.html(`
                        <a href="${fileUrl}" target="_blank" style="font-size: 1.5rem; text-decoration: none;">
                            <i class="bi bi-eye" style="font-size: 1.5rem; cursor: pointer;"></i>
                        </a>
                    `);
                };

                reader.readAsDataURL(file);
            }   
        } else {
            preview.html('<p>No file selected</p>');
        }
    }
    $('#uploadBtn').on('click', function (e) {
        e.preventDefault();

        let applicationId = $('#hiddenApplicationId').val().trim();

        if (!applicationId) {
            Swal.fire('Missing', 'Application ID is missing.', 'warning');
            return;
        }

        $.ajax({
            url: "/Upload/InfoBeforeUpload",
            type: 'POST',
            data: { applicationId: applicationId },
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        title: 'Are you sure?',
                        html: response.message,
                        icon: 'info',
                        showCancelButton: true,
                        confirmButtonText: 'OK',
                        cancelButtonText: 'Cancel',
                        customClass: {
                            popup: 'custom-swal-popup',
                            title: 'custom-swal-title',
                            htmlContainer: 'custom-swal-html',
                            confirmButton: 'custom-swal-button'
                        }
                    }).then((result) => {
                        if (result.isConfirmed) {
                            $('#uploadForm').submit();
                        }
                    });
                } else {
                    Swal.fire('Error', response.message, 'error');
                }
            },
            error: function () {
                Swal.fire('Error', 'Something went wrong while validating.', 'error');
            }
        });
    });


});
function checkUploadFiles() {
    const formType = $('#hiddenFormType').val();
    const $uploadBtn = $('#uploadBtn');
    const requiredFields = {
        'CA': ['CancelledCheque', 'PaySlipPdf', 'QuotationPdf', 'DrivingLicensePdf'],
        'PCA': ['CancelledCheque', 'PaySlipPdf', 'QuotationPdf'],
        'HBA': ['CancelledCheque', 'PaySlipPdf']
    };

    function checkAllRequiredFiles() {
        const required = requiredFields[formType] || [];
        let allFilled = true;
        let IsExtension = $('#isExtension').val() === 'true';

        required.forEach(function (fieldId) {
            const $fileInput = $('#' + fieldId);
            if ($fileInput.length && (!$fileInput[0].files || $fileInput[0].files.length === 0)) {
                allFilled = false;
            }
        });

        // Handle Service Extension field based on IsExtension
        const $serviceExtnField = $('#SeviceExtnPdf');

        if (IsExtension) {
            // If extension is true: enable field and make it mandatory
            $serviceExtnField.prop('disabled', false);
            $serviceExtnField.prop('required', true);

            // Check if Service Extension file is uploaded
            if ($serviceExtnField.length && (!$serviceExtnField[0].files || $serviceExtnField[0].files.length === 0)) {
                allFilled = false;
            }
        } else {
            // If extension is false: disable field and make it not required
            $serviceExtnField.prop('disabled', true);
            $serviceExtnField.prop('required', false);
            // Clear the file input if disabled
            $serviceExtnField.val('');
        }

        // Update upload button state
        $uploadBtn.prop('disabled', !allFilled);
        if (allFilled) {
            $uploadBtn.removeClass('btn-secondary').addClass('btn-success');
        } else {
            $uploadBtn.removeClass('btn-success').addClass('btn-secondary');
        }
    }

    // Bind change event to all file inputs
    $('input[type="file"]').on('change', checkAllRequiredFiles);

    // Initial check
    checkAllRequiredFiles();
}

function messageHandler() {
    const message = $('#messageHolder').val();

    if (message && message.trim() !== '') {
        Swal.fire({
            html: `<span style="font-size: 25px; font-weight: bold; color: black;">${message}</span>`,
            icon: 'success',
            confirmButtonText: 'OK',
            customClass: {
                confirmButton: 'btn btn-primary btn-lg',
                title: 'font-weight-bold',
                htmlContainer: 'text-dark'
            },
            padding: '25px'
        });
    }
}