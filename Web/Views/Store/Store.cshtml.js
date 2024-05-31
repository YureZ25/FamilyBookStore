import $ from "jquery"

export const Store = function () {
    $('#linkStoreToUserButton').click(function () {
        $.post('/Store/LinkStoreToUser', {
            storeId: @Model.StoreGet?.Id,
        });
    });
    $('#unlinkStoreFromUserButton').click(function () {
        $.post('/Store/UnlinkStoreFromUser', {
            storeId: @Model.StoreGet?.Id,
        });
    });
}();