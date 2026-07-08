
$(document).ready(function () {
    mMsater(0, "loanType", 5, 0);
    mMsater(0, "applicantCategory", 6, 0);
    $('#btnAgree').click(function () {
        const loanType = $('#loanType').val();
        const applicantCategory = $('#applicantCategory').val();

        if (loanType !== "" && applicantCategory !== "" && loanType !== null && applicantCategory !== null) {
            if ((loanType == 3) && (applicantCategory == 2 || applicantCategory == 3)) {
                Swal.fire({
                    title: "Warning",
                    html: `
                <p>You are requested to fill application on Arpan website with your login ID.</p>
                <p class="mt-2">
                    <a href="/PdfViewer/InstrARPAN" target="_blank">
                        📄 Click here to read General Instructions
                    </a>
                </p>
            `,
                    icon: "warning",
                    showDenyButton: true,
                    confirmButtonText: "OK",
                    denyButtonText: "Open Arpan Website"
                }).then((result) => {
                    if (result.isDenied) {
                        window.open('https://arpan.example.com', '_blank', 'noopener,noreferrer');
                    }
                });
            }
            else {
                document.getElementById('loanForm').submit(); // Submit the form
            }
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
