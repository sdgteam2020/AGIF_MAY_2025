
$(document).ready(function () {
    handleNavbarClick();
});
function handleNavbarClick() {
    let currentPath = window.location.pathname.toLowerCase();

    $(".dropdown-item, .nav-link").each(function () {
        let linkPath = $(this).attr("href")?.toLowerCase();

        if (linkPath && currentPath === linkPath) {
            $(this).addClass("active fw-bold").attr("aria-current", "page");

            let topNavItem = $(this).closest(".nav-item.dropdown");
            topNavItem.find(".nav-link.dropdown-toggle").first().addClass("active fw-bold");

            let parentSubmenuItem = $(this).closest(".dropdown-submenu").prev(".dropdown-item");
            parentSubmenuItem.addClass("active fw-bold");
        } else {
            $(this).removeClass("active fw-bold").removeAttr("aria-current");
        }
    });

    $(".nav-link.dropdown-toggle").on("click", function () {
        $(".nav-link.dropdown-toggle").removeClass("active fw-bold");
        $(this).addClass("active fw-bold");
    });
}
function getCsrfToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}
$(document).ready(function () {
    $.ajax({
        url: '/Home/UpdateHitCounter', // Controller Action
        type: 'GET',
        success: function (response) {
            console.log(response);
            $('#today').text('Today: ' + response.todayCount);
            $('#monthly').text('Monthly: ' + response.monthlyCount);
            $('#total').text('Total: ' + response.totalCount);
        },
        error: function () {
            console.log('Error loading hit counter');
        }
    });

});
