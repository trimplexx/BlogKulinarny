document.addEventListener("DOMContentLoaded", function () {
    const userDropdown = document.getElementById("userDropdown");
    const userMenu = document.querySelector(".user-menu");

    userDropdown.addEventListener("click", function (event) {
        event.stopPropagation();
    });

    userMenu.addEventListener("click", function (event) {
        event.stopPropagation();
    });

    document.body.addEventListener("click", function (event) {
        if (userMenu.parentElement.classList.contains("show")) {
            userDropdown.click();
        }
    });
});