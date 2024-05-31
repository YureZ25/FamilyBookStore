import $ from "jquery"

export const Store = function () {
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