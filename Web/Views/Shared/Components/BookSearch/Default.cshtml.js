import "./Default.cshtml.css"

import $ from "jquery"
import "typeahead.js";
import Bloodhound from "typeahead.js/dist/bloodhound";
import Handlebars from "handlebars";

const Module = function () {

    console.log("Book search works!")

    const bookSource = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.whitespace,
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        identify: (e) => { console.log(e); return e.id;},
        remote: {
            url: "/Home/BooksPrompts?prompt=$prompt$",
            wildcard: "$prompt$",
            rateLimitby: "debounce",
            rateLimitWait: "500"
        },
    });

    $(".typeahead").typeahead(
        {
            highlight: true,
            minLength: 1,
            hint: true
        }, 
        {
            name: "books",
            source: bookSource,
            limit: 5,

            display: "title",
            templates: {
                suggestion: Handlebars.compile('<div><strong>{{title}}</strong></div>')
            }
        });
};

export const BookSearch = function () { $(Module) }();