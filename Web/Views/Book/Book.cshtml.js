import $ from "jquery"

export const Book = function () {
    let bookStatusId;
    $(":radio").click(function (e) {
        if (bookStatusId === e.target.id) {
            $(e.target).prop('checked', false);
            $('#@BookStatus.None.ToString()').prop('checked', true);
        }
        bookStatusId = e.target.id;
    });

    $("#@nameof(Model.BookPost)_@nameof(Model.BookPost.Price)").change(function () {
        this.value = parseFloat(this.value).toFixed(2);
    });
}();