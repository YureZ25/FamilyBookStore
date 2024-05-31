import "./Default.cshtml.css"

import $ from "jquery"
import "typeahead.js";
import Bloodhound from "bloodhound-js";

const Module = function () {
    const books = ["muracami", "london"];

    const bookSource = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.whitespace,
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        local: books
    })

    const bookDataset = {
        name: "books",
        source: bookSource
    };

    const options = {
        highlight: true,
        minLength: 3,
        hint: true,
    };

    $(".typeahead").typeahead(options, bookDataset);
};

export const BookSearch = function () { $(Module) }();