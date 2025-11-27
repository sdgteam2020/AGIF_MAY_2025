$(document).ready(function () {
    messageHandler() 
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
    //function previewFile(input, previewSelector) {
    //    const file = input.files[0];
    //    const preview = $(previewSelector);
    //    const errorContainer = $(input).closest('.col-md-10').find('.file-error-message'); // Find the error container below the input
    //    const maxFileSize = 1 * 1024 * 1024; // 1MB in bytes
    //    errorContainer.text('');

    //    if (file) {
    //        // Check if the file is a PDF
    //        if (file.type !== 'application/pdf') {
    //            errorContainer.text('Only PDF files are allowed').css('color', 'red');
    //            input.value = ''; // Clear the input field
    //            return;
    //        }

    //        // Check the file size (must not exceed 1MB)
    //        if (file.size > maxFileSize) {
    //            errorContainer.text('File size must not exceed 1 MB').css('color', 'red');
    //            input.value = ''; // Clear the input field
    //        } else {
    //            const reader = new FileReader();

    //            reader.onload = function (e) {
    //                // Create a link with the eye icon and the PDF file URL
    //                const fileUrl = URL.createObjectURL(file); // Create a Blob URL for the file
    //                preview.html(`
    //                    <a href="${fileUrl}" target="_blank" style="font-size: 1.5rem; text-decoration: none;">
    //                        <i class="bi bi-eye" style="font-size: 1.5rem; cursor: pointer;"></i>
    //                    </a>
    //                `);
    //            };

    //            reader.readAsDataURL(file);
    //        }
    //    } else {
    //        preview.html('<p>No file selected</p>');
    //    }
    //}


    //function previewFile(input, previewSelector) {
    //    const file = input.files[0];
    //    const preview = $(previewSelector);
    //    const errorContainer = $(input).closest('.col-md-10').find('.file-error-message');
    //    const maxFileSize = 1 * 1024 * 1024;

    //    errorContainer.text('');

    //    if (file) {
    //        if (file.type !== 'application/pdf') {
    //            errorContainer.text('Only PDF files are allowed').css('color', 'red');
    //            input.value = '';
    //            return;
    //        }

    //        if (file.size > maxFileSize) {
    //            errorContainer.text('File size must not exceed 1 MB').css('color', 'red');
    //            input.value = '';
    //        } else {
    //            const reader = new FileReader();
    //            reader.onload = function (e) {
    //                preview.html(`
    //                <i class="bi bi-eye uploadeye"></i>
    //            `);

    //                preview.find('.uploadeye').on('click', function () {
    //                    showPdfModal(e.target.result);
    //                });
    //            };
    //            reader.readAsDataURL(file);
    //        }
    //    }
    //}

    //function showPdfModal(pdfData) {
    //    const modal = $('<div class="modal fade" tabindex="-1">').html(`
    //    <div class="modal-dialog modal-lg">
    //        <div class="modal-content">
    //            <div class="modal-header">
    //                <h5 class="modal-title">PDF Preview</h5>
    //                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
    //            </div>
    //            <div class="modal-body pdf-scroll-body">
    //                <div id="pdfCanvasContainer"></div>
    //            </div>
    //        </div>
    //    </div>
    //`);

    //    $('body').append(modal);
    //    modal.modal('show');

    //    const loadingTask = pdfjsLib.getDocument(pdfData);
    //    loadingTask.promise.then(function (pdf) {
    //        const container = document.getElementById('pdfCanvasContainer');
    //        const numPages = pdf.numPages;

    //        // Render all pages
    //        for (let pageNum = 1; pageNum <= numPages; pageNum++) {
    //            pdf.getPage(pageNum).then(function (page) {
    //                const canvas = document.createElement('canvas');
    //                canvas.className = 'pdf-page-canvas';
    //                const context = canvas.getContext('2d');
    //                const viewport = page.getViewport({ scale: 1.5 });

    //                canvas.height = viewport.height;
    //                canvas.width = viewport.width;

    //                container.appendChild(canvas);

    //                page.render({
    //                    canvasContext: context,
    //                    viewport: viewport
    //                });
    //            });
    //        }
    //    });

    //    modal.on('hidden.bs.modal', function () {
    //        modal.remove();
    //    });
    //}

    function previewFile(input, previewSelector) {
        const file = input.files[0];
        const preview = $(previewSelector);
        const errorContainer = $(input).closest('.col-md-10').find('.file-error-message');
        const maxFileSize = 1 * 1024 * 1024; // 1MB

        errorContainer.text('');

        if (file) {
            // Check if the file is a PDF
            if (file.type !== 'application/pdf') {
                errorContainer.text('Only PDF files are allowed').css('color', 'red');
                input.value = '';
                return;
            }

            // Check the file size
            if (file.size > maxFileSize) {
                errorContainer.text('File size must not exceed 1 MB').css('color', 'red');
                input.value = '';
            } else {
                // Show eye icon for preview
                preview.html(`
                <i class="bi bi-eye uploadeye"></i>
            `);

                // Click event to show PDF in modal
                preview.find('.uploadeye').on('click', function () {
                    showPdfInModal(file);
                });
            }
        } else {
            preview.html('<p>No file selected</p>');
        }
    }

    // Function to show PDF in modal using createObjectURL
    function showPdfInModal(file) {
        // Clear previous PDF if any
        const pdfContainer = document.getElementById("pdfContainer");
        pdfContainer.innerHTML = '';

        // Create blob URL
        const blob = new Blob([file], { type: 'application/pdf' });
        const pdfUrl = URL.createObjectURL(blob);

        // Create embed element
        const embed = document.createElement("embed");
        embed.src = pdfUrl + "#toolbar=0&navpanes=0&scrollbar=0";
        embed.type = "application/pdf";
        embed.classList.add("w-100", "h-100", "border-0", "rounded");

        // Append to container
        pdfContainer.appendChild(embed);

        // Show modal
        $("#ViewPdf").modal("show");

        // Clean up blob URL when modal is closed
        $('#ViewPdf').on('hidden.bs.modal', function () {
            URL.revokeObjectURL(pdfUrl);
            pdfContainer.innerHTML = '';
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
            url: "/Claim/InfoBeforeUpload",
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
    const $uploadBtn = $('#uploadBtn');
    const requiredFields = ['CancelledCheque', 'PaySlipPdf']; // Make these fields required
    let allFilled = true;
    let IsExtension = $('#isExtension').val() === 'true';  // Get the value for IsExtension
    const type = $('#hiddenFormType').val(); // Get the value for type
    // Check if the required fields have files selected
    requiredFields.forEach(function (fieldId) {
        const $fileInput = $('#' + fieldId);
        if ($fileInput.length && (!$fileInput[0].files || $fileInput[0].files.length === 0)) {
            allFilled = false;
        }
    });

    // Handle Service Extension field based on IsExtension
    const $serviceExtnField = $('#SeviceExtnPdf');
    const $SpdocusPdfField = $('#SpdocusPdf');

    if (type === 'SP') {
        $SpdocusPdfField.prop('disabled', false);
        $SpdocusPdfField.prop('required', true);

        // Check if Service Extension file is uploaded
        if ($SpdocusPdfField.length && (!$SpdocusPdfField[0].files || $SpdocusPdfField[0].files.length === 0)) {
            allFilled = false;
        }

    }
    else {
        // If extension is false: disable field and make it not required
        $SpdocusPdfField.prop('disabled', true);
        $SpdocusPdfField.prop('required', false);
        // Clear the file input if disabled
        $SpdocusPdfField.val('');
    }



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
$('input[type="file"]').on('change', checkUploadFiles);


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