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

    function previewFile(input, previewSelector) {
        const file = input.files[0];
        const preview = $(previewSelector);
        const errorContainer = $(input).closest('.col-md-10').find('.file-error-message');
        const maxFileSize = 1 * 1024 * 1024; // 1MB

        errorContainer.text('');

        if (file) {
            if (file.type !== 'application/pdf') {
                errorContainer.text('Only PDF files are allowed').css('color', 'red');
                input.value = '';
                return;
            }

            if (file.size > maxFileSize) {
                errorContainer.text('File size must not exceed 1 MB').css('color', 'red');
                input.value = '';
            } else {
                preview.html(`
                <i class="bi bi-eye uploadeye"></i>
            `);

                preview.find('.uploadeye').on('click', function () {
                    showPdfInModal(file);
                });
            }
        } else {
            preview.html('<p>No file selected</p>');
        }
    }

    function showPdfInModal(file) {
        const pdfContainer = document.getElementById("pdfContainer");
        const loader = document.getElementById("loadingOverlay");

        // ✅ Show loader
        loader.classList.remove("d-none");

        // ✅ Remove only existing embed (not loader)
        const oldEmbed = pdfContainer.querySelector("embed");
        if (oldEmbed) {
            oldEmbed.remove();
        }

        const blob = new Blob([file], { type: 'application/pdf' });
        const pdfUrl = URL.createObjectURL(blob);

        const embed = document.createElement("embed");
        embed.src = pdfUrl + "#toolbar=0&navpanes=0&scrollbar=0";
        embed.type = "application/pdf";
        embed.classList.add("w-100", "h-100", "border-0", "rounded");

        // ✅ Hide loader after load
        embed.onload = function () {
            loader.classList.add("d-none");
        };

        pdfContainer.appendChild(embed);

        $("#ViewPdf").modal("show");

        $('#ViewPdf').one('hidden.bs.modal', function () {
            URL.revokeObjectURL(pdfUrl);

            // ✅ Remove only embed
            const embed = pdfContainer.querySelector("embed");
            if (embed) embed.remove();

            loader.classList.add("d-none");
        });
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
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
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

        const $serviceExtnField = $('#SeviceExtnPdf');

        if (IsExtension) {
            $serviceExtnField.prop('disabled', false);
            $serviceExtnField.prop('required', true);

            if ($serviceExtnField.length && (!$serviceExtnField[0].files || $serviceExtnField[0].files.length === 0)) {
                allFilled = false;
            }
        } else {
            $serviceExtnField.prop('disabled', true);
            $serviceExtnField.prop('required', false);
            $serviceExtnField.val('');
        }

        $uploadBtn.prop('disabled', !allFilled);
        if (allFilled) {
            $uploadBtn.removeClass('btn-secondary').addClass('btn-success');
        } else {
            $uploadBtn.removeClass('btn-success').addClass('btn-secondary');
        }
    }

    $('input[type="file"]').on('change', checkAllRequiredFiles);

    checkAllRequiredFiles();
}

function messageHandler() {
    const message = $('#messageHolder').val();

    if (message && message.trim() !== '') {
        Swal.fire({
            html: `<span id="message">${message}</span>`,
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