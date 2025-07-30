function showSuccessMessage(message) {
    // Using Bootstrap alert (if you have Bootstrap)
    const alertHtml = `
        <div class="alert alert-success alert-dismissible fade show" role="alert" style="position: fixed; top: 20px; right: 20px; z-index: 9999; min-width: 300px;">
            <i class="lni lni-checkmark-circle"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    $('body').append(alertHtml);

    // Auto remove after 3 seconds
    setTimeout(function () {
        $('.alert-success').fadeOut(300, function () {
            $(this).remove();
        });
    }, 2000);
}
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
$(document).ready(function () {
    // Event listener for the link with id 'pendingLink'
    $('.submit-status').on('click', function (e) {
        e.preventDefault();  // Prevent default anchor behavior

        // Retrieve data attributes from the link
        const controller = $(this).data('controller');
        const method = $(this).data('method');
        const status = $(this).data('status');

        // Call the submitStatus function with the extracted data
        submitStatus(controller, method, status);
    });
});

function submitStatus(controller, method, status) {
    const form = document.createElement('form');
    form.method = 'POST';
    form.action = `/${controller}/${method}`;

    const input = document.createElement('input');
    input.type = 'hidden';
    input.name = 'status';
    input.value = status;

    form.appendChild(input);
    document.body.appendChild(form);
    form.submit();
}
