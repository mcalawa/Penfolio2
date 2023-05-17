// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function () {
    $(document).ready(function () {
        $("#profileMenu").on("mousedown keydown", function (event) {
            if (event.key === "Tab") {
                return;
            }

            $("#profileMenu + .nav-dropdown-content").toggleClass("show");

            $("#writingMenu + .nav-dropdown-content").removeClass("show");
        });

        $("#writingMenu").on("mousedown keydown", function (event) {
            if (event.key === "Tab") {
                return;
            }

            $("#writingMenu + .nav-dropdown-content").toggleClass("show");

            $("#profileMenu + .nav-dropdown-content").removeClass("show");
        });

        $("#notificationsMenu").on("mousedown keydown", function (event) {
            if (event.key === "Tab") {
                return;
            }

            $("#notificationsMenu + .dropdown-content").toggleClass("show");
        })
    })
})();