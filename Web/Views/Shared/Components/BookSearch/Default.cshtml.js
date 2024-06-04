import $ from "jquery"
import "typeahead.js";
import Bloodhound from "typeahead.js/dist/bloodhound";
import Handlebars from "handlebars";

const Module = function () {
    
    const bookSource = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace("title"),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        identify: (e) => e.id,
        prefetch: {
            url: "/Home/BooksSuggestions",
            cache: false,
        },
        remote: {
            url: "/Home/BooksPrompts?prompt=$prompt$",
            wildcard: "$prompt$",
            rateLimitby: "debounce",
            rateLimitWait: "500"
        },
    });

    function getBookSuggestions(query, sync, async) {
        if (query === "") {
            sync(bookSource.all());
        } else {
            bookSource.search(query, sync, async);
        }
    }

    const authorSource = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace("fullName"),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        identify: (e) => e.id,
        prefetch: {
            url: "/Home/AuthorsSuggestions",
            cache: false,
        },
        remote: {
            url: "/Home/AuthorsPrompts?prompt=$prompt$",
            wildcard: "$prompt$",
            rateLimitby: "debounce",
            rateLimitWait: "500"
        },
    });

    function getAuthorSuggestions(query, sync, async) {
        if (query === "") {
            sync(authorSource.all());
        } else {
            authorSource.search(query, sync, async);
        }
    }

    $(".typeahead").typeahead(
        {
            highlight: true,
            minLength: 0,
            hint: true,
        }, 
        {
            source: getBookSuggestions,
            name: "books",
            limit: 5,
            async: true,
            display: "title",
            templates: {
                header: '<h5 class="dropdown-header">Книги</h5>',
                suggestion: Handlebars.compile('<span>{{title}}</span>'),
            },
        },
        {
            source: getAuthorSuggestions,
            name: "authors",
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