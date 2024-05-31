import $ from "jquery"

const Module = function () {
    const storeId = $("#StorePost_Id").attr("value");

    $("#linkStoreToUserButton").on("click", function () {
        $.post("/Store/LinkStoreToUser", {
            storeId: storeId,
        });
    });
    $("#unlinkStoreFromUserButton").on("click", function () {
        $.post("/Store/UnlinkStoreFromUser", {
            storeId: storeId,
        });
    });
}();

export const Store = function () { $(Module) }();