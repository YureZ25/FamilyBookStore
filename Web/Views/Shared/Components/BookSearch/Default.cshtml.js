import $ from "jquery"
import "typeahead.js";
import Bloodhound from "typeahead.js/dist/bloodhound";
import Handlebars from "handlebars";

const Module = function () {
    
    const bookSource = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace("title"),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        identify: (e) => e.id,
        remote: {
            url: "/Home/BooksPrompts?prompt=$prompt$",
            wildcard: "$prompt$",
            rateLimitby: "debounce",
            rateLimitWait: "500"
        },
    });

    const authorSource = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.whitespace,
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        identify: (e) => e.id,
        remote: {
            url: "/Home/AuthorsPrompts?prompt=$prompt$",
            wildcard: "$prompt$",
            rateLimitby: "debounce",
            rateLimitWait: "500"
        },
    });

    $(".typeahead").typeahead(
        {
            highlight: true,
            minLength: 1,
            hint: true,
        }, 
        {
            name: "books",
            source: bookSource,
            limit: 5,
            async: true,
            display: "title",
            templates: {
                header: '<h5 class="dropdown-header">Книги</h5>',
                suggestion: Handlebars.compile('<span>{{title}}</span>'),
            },
        },
        {
            name: "authors",
            source: authorSource,
            limit: 5,
            async: true,
            display: "fullName",
            templates: {
                header: '<h5 class="dropdown-header">Авторы</h5>',
                suggestion: Handlebars.compile('<span>{{fullName}}</span>'),
            },
        });

    $(".typeahead").on('typeahead:select', function(ev, suggestion) {
        console.log('Selection: ' + JSON.stringify(suggestion));
        $("form[role='search']").trigger("submit");
    });    
};

export const BookSearch = function () { $(Module) }();