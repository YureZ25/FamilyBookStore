import $ from "jquery"

export const Index = function () {
    $("button.nav-link[data-bs-target]").click(function () {
        const searchParams = new URLSearchParams(window.location.search);
        searchParams.set("@(nameof(BookStatus))", this.getAttribute("data-bs-target").replace("#", ""));
        history.pushState(null, '', window.location.pathname + '?' + searchParams.toString());
    });

    $(document).ready(function () {
        const searchParams = new URLSearchParams(window.location.search);

        let bookStatusTab = searchParams.get("@(nameof(BookStatus))");
        if (!bookStatusTab) bookStatusTab = "@(BookStatus.WillRead)";

        $(`button.nav-link[data-bs-target='#${bookStatusTab}']`).click();
    });
}();