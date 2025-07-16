function handleNavbarClick() {
    let currentPath = window.location.pathname.toLowerCase();

    $(".dropdown-item, .nav-link").each(function () {
        let linkPath = $(this).attr("href")?.toLowerCase();

        if (linkPath && currentPath === linkPath) {
            $(this).addClass("active fw-bold").attr("aria-current", "page");

            // Highlight top-level menu (e.g., "Benefits")
            let topNavItem = $(this).closest(".nav-item.dropdown");
            topNavItem.find(".nav-link.dropdown-toggle").first().addClass("active fw-bold");

            // ✅ Highlight middle-level parent (e.g., "While in Service")
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
