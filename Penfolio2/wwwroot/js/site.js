// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function () {
    $(document).ready(function () {
        $("#privacyMenu").on("focusin", function () {
            if ($("#profileMenu + .nav-dropdown-content").hasClass("show")) {
                $("#profileMenu + .nav-dropdown-content").removeClass("show");

                if ($("#profileMenu i").hasClass("fa-caret-up")) {
                    $("#profileMenu i").removeClass("fa-caret-up");
                }

                if ($("#profileMenu i").hasClass("fa-caret-down") !== true) {
                    $("#profileMenu i").addClass("fa-caret-down");
                }
            }
        });

        $("#profileMenu").on("focusin", function () {
            if ($("#writingMenu + .nav-dropdown-content").hasClass("show")) {
                $("#writingMenu + .nav-dropdown-content").removeClass("show");

                if ($("#writingMenu i").hasClass("fa-caret-up")) {
                    $("#writingMenu i").removeClass("fa-caret-up");
                }

                if ($("#writingMenu i").hasClass("fa-caret-down") !== true) {
                    $("#writingMenu i").addClass("fa-caret-down");
                }
            }
        });

        $("#profileMenu").on("mousedown keydown", function (event) {
            if (event.key === "Tab" || event.key === "Shift") {
                return;
            }

            $("#profileMenu + .nav-dropdown-content").toggleClass("show");
            $("#profileMenu i").toggleClass("fa-caret-down");
            $("#profileMenu i").toggleClass("fa-caret-up");

            if ($("#writingMenu + .nav-dropdown-content").hasClass("show")) {
                $("#writingMenu + .nav-dropdown-content").removeClass("show");

                if ($("#writingMenu i").hasClass("fa-caret-up")) {
                    $("#writingMenu i").removeClass("fa-caret-up");
                }

                if ($("#writingMenu i").hasClass("fa-caret-down") !== true) {
                    $("#writingMenu i").addClass("fa-caret-down");
                }
            }

            if ($("#notificationsMenu + .dropdownContent").hasClass("show")) {
                $("#notificationsMenu + .dropdownContent").removeClass("show");
            }
        });

        $("#writingMenu").on("focusin", function () {
            if ($("#profileMenu + .nav-dropdown-content").hasClass("show")) {
                $("#profileMenu + .nav-dropdown-content").removeClass("show");

                if ($("#profileMenu i").hasClass("fa-caret-up")) {
                    $("#profileMenu i").removeClass("fa-caret-up");
                }

                if ($("#profileMenu i").hasClass("fa-caret-down") !== true) {
                    $("#profileMenu i").addClass("fa-caret-down");
                }
            }
        });

        $("#writingMenu").on("mousedown keydown", function (event) {
            if (event.key === "Tab" || event.key === "Shift") {
                return;
            }

            $("#writingMenu + .nav-dropdown-content").toggleClass("show");
            $("#writingMenu i").toggleClass("fa-caret-down");
            $("#writingMenu i").toggleClass("fa-caret-up");

            if ($("#profileMenu + .nav-dropdown-content").hasClass("show")) {
                $("#profileMenu + .nav-dropdown-content").removeClass("show");

                if ($("#profileMenu i").hasClass("fa-caret-up")) {
                    $("#profileMenu i").removeClass("fa-caret-up");
                }

                if ($("#profileMenu i").hasClass("fa-caret-down") !== true) {
                    $("#profileMenu i").addClass("fa-caret-down");
                }
            }

            if ($("#notificationsMenu + .dropdownContent").hasClass("show")) {
                $("#notificationsMenu + .dropdownContent").removeClass("show");
            }
        });

        $("#manageMenu").on("focusin", function () {
            if ($("#writingMenu + .nav-dropdown-content").hasClass("show")) {
                $("#writingMenu + .nav-dropdown-content").removeClass("show");

                if ($("#writingMenu i").hasClass("fa-caret-up")) {
                    $("#writingMenu i").removeClass("fa-caret-up");
                }

                if ($("#writingMenu i").hasClass("fa-caret-down") !== true) {
                    $("#writingMenu i").addClass("fa-caret-down");
                }
            }

            if ($("#notificationsMenu + .dropdownContent").hasClass("show")) {
                $("#notificationsMenu + .dropdownContent").removeClass("show");
            }
        });

        $("#registerMenu").on("focusin", function () {
            if ($("#writingMenu + .nav-dropdown-content").hasClass("show")) {
                $("#writingMenu + .nav-dropdown-content").removeClass("show");

                if ($("#writingMenu i").hasClass("fa-caret-up")) {
                    $("#writingMenu i").removeClass("fa-caret-up");
                }

                if ($("#writingMenu i").hasClass("fa-caret-down") !== true) {
                    $("#writingMenu i").addClass("fa-caret-down");
                }
            }
        });

        $("#notificationsMenu").on("mousedown keydown", function (event) {
            if (event.key === "Tab" || event.key === "Shift") {
                return;
            }

            $("#notificationsMenu + .dropdownContent").toggleClass("show");
        });

        $("#logoutMenu").on("focusin", function () {
            if ($("#notificationsMenu + .dropdownContent").hasClass("show")) {
                $("#notificationsMenu + .dropdownContent").removeClass("show");
            }
        });

        $("#loginMenu").on("focusin", function () {
            if ($("#notificationsMenu + .dropdownContent").hasClass("show")) {
                $("#notificationsMenu + .dropdownContent").removeClass("show");
            }
        });
    })
})();