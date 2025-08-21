

$(document).ready(function () {
    mMsater(0, "Category", 6, 0);
    mMsater(0, "PurposeOfWithdrwal", 19, 0);

    $('#btnAgree').click(function () {
        const PurposeOfWithdrwal = $('#PurposeOfWithdrwal').val();
        const applicantCategory = $('#Category').val();

        if (PurposeOfWithdrwal !== "" && applicantCategory !== "") {
            document.getElementById('loanForm').submit(); // Submit the form
        } else {
            $('.errormessage').html('<div class="alert alert-danger" role="alert">⚠️ Please Select Category and Withdrawal Purpose Both!</div>');

        }

    })


    $('#genInstr').on('click', function (e) {
        e.preventDefault();
        window.open(`/PdfViewer/InstrPCA`, '_blank');
    });
});
