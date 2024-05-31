import "./Book.cshtml.css"

import $ from "jquery";

const Module = function () {
    let bookStatusId;
    $(":radio").on("click", function (e) {
        if (bookStatusId === e.target.id) {
            $(e.target).prop("checked", false);
            $("#None").prop("checked", true);
        }
        bookStatusId = e.target.id;
    });

    $("#BookPost_Price").on("change", function () {
        this.value = parseFloat(this.value).toFixed(2);
    });
};

export const Book = function () { $(Module) }();