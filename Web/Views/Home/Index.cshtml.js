import $ from "jquery";

const Module = function () {
    $("button.nav-link[data-bs-target]").on("click", function () {
        const searchParams = new URLSearchParams(window.location.search);
        searchParams.set("BookStatus", this.getAttribute("data-bs-target").replace("#", ""));
        history.pushState(null, "", `${window.location.pathname}?${searchParams.toString()}`);
    });

    const searchParams = new URLSearchParams(window.location.search);

    let bookStatusTab = searchParams.get("BookStatus");
    if (!bookStatusTab) bookStatusTab = "WillRead";

    $(`button.nav-link[data-bs-target='#${bookStatusTab}']`).trigger("click");
}

export const Index = function () { $(Module) }();