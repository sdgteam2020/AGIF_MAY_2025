


$(document).ready(function () {
    mMsater(0, "loanType", 5, 0);
    mMsater(0, "applicantCategory", 6, 0);
    $('#btnAgree').click(function () {
        const loanType = $('#loanType').val();
        const applicantCategory = $('#applicantCategory').val();

        if (loanType !== "" && applicantCategory !== "" && loanType !== null && applicantCategory !==null) {
            document.getElementById('loanForm').submit(); // Submit the form
        } else {
            $('.errormessage').html('<div class="alert alert-danger" role="alert">⚠️ Please Select Loan Type and Category Both!</div>');
        }

    })

    $('#genInstr').on('click', function (e) {
        e.preventDefault();
        const loanType = $('#loanType').val();
        if (loanType == "") {
            $('.errormessage').html('<div class="alert alert-danger" role="alert">⚠️ Please Select Loan Type!</div>');
            return;
        }
        let url = '';
        if (loanType == 1) {
            url = `/PdfViewer/InstrHBA`;
        }
        else if (loanType == 2) {
            url = `/PdfViewer/InstrCA`;
        }
        else {
            url = `/PdfViewer/InstrPCA`;
        }
        window.open(url, '_blank');
    });
});
