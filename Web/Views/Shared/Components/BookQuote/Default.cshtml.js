import $ from "jquery"

const Module = function () {
    $("#addQuoteModalForm").on("submit", function (event) {
        event.preventDefault();

        const formData = $(this).serialize();

        $.post("/BookQuote/AddQuote", formData, function(response) {
          console.log('Server Response: ', response);
        }).fail(function(xhr, status, error) {
          console.error('Error: ', error);
        });
    });
};

export const BookQuote = function () { $(Module) }();