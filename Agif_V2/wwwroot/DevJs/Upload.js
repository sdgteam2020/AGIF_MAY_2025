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
        required.forEach(function (fieldId) {
            const $fileInput = $('#' + fieldId);
            if ($fileInput.length && (!$fileInput[0].files || $fileInput[0].files.length === 0)) {
                allFilled = false;
            }
        });
        $uploadBtn.prop('disabled', !allFilled);
        if (allFilled) {
            $uploadBtn.removeClass('btn-secondary').addClass('btn-success');
        } else {
            $uploadBtn.removeClass('btn-success').addClass('btn-secondary');
        }
    }
    $('input[type="file"]').on('change', checkAllRequiredFiles);

    // Initial check
    checkAllRequiredFiles();
}

