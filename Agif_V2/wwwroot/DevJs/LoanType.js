


$('#btnAgree').click(function () {
    const loanType = $('#loanType').val();
    const applicantCategory = $('#applicantCategory').val();

    if (loanType == null || applicantCategory == null) {
        $(".errormessage").html(`<div class="alert alert-danger" role="alert">Please select loan type and applicant category</div>`);
       
        return false;
    }
    const url = `/OnlineApplication/OnlineApplication?loanType=${encodeURIComponent(loanType)}&applicantCategory=${encodeURIComponent(applicantCategory)}`;
    window.location.href = url;
})