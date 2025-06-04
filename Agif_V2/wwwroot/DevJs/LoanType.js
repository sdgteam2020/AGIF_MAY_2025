


$('#btnAgree').click(function () {
    const loanType = $('#loanType').val();
    const applicantCategory = $('#applicantCategory').val();

    //if (loanType == null || applicantCategory == null) {
    //    $(".errormessage").html(`<div class="alert alert-danger" role="alert">Please select loan type and applicant category</div>`);
       
    //    return false;
    //}
    //if (loanType != "" && applicantCategory != "") {
    //    const url = `/OnlineApplication/OnlineApplication?loanType=${encodeURIComponent(loanType)}&applicantCategory=${encodeURIComponent(applicantCategory)}`;
    //    window.location.href = url;
    //}
    //else {
    //    $('.errormessage').html('<div class="alert alert-danger" role="alert">⚠️ Please Select Loan Type and Category Both!</div>');
    //}

    if (loanType !== "" && applicantCategory !== "") {
        document.getElementById('loanForm').submit(); // Submit the form
    } else {
        // Show an error message if required fields are missing
       // alert("Please select both Loan Type and Applicant Category.");
        $('.errormessage').html('<div class="alert alert-danger" role="alert">⚠️ Please Select Loan Type and Category Both!</div>');
        
    }
  
})