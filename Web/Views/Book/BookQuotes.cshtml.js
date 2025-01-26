import $ from "jquery";
import "jquery-validation";
import { Modal } from "bootstrap";

const Module = function () {
  const addQuoteModal = document.getElementById("addQuoteModal");
  const addQuoteModalBs = new Modal(addQuoteModal);

  const editQuoteModal = document.getElementById("editQuoteModal");
  const editQuoteModalBs = new Modal(editQuoteModal);

  addQuoteModal.addEventListener("hide.bs.modal", () =>
    $("#addQuoteForm").trigger("reset")
  );
  editQuoteModal.addEventListener("hide.bs.modal", () =>
    $("#editQuoteForm").trigger("reset")
  );

  $("#addQuoteModalButton").on("click", () => {
    $("#addQuoteBookId").val(bookPageBookId);
    addQuoteModalBs.show();
  });

  $("#addQuoteForm").on("submit", (event) => {
    event.preventDefault();

    const form = $(event.target);
    const validator = form.validate();
    if (!validator.valid()) return;
    const formData = form.serialize();

    $.post("/BookQuote/AddQuote", formData)
      .done((response, status, xhr) => {
        if (tryShowErrors(xhr, validator)) return;

        const quote = xhr.responseJSON.data;
        const item = createQuoteItem(quote);
        $("#quotesList").append(item);

        addQuoteModalBs.hide();
      })
      .fail((xhr, status, error) => {
        tryShowErrors(xhr, validator);
      });
  });

  $("#quotesList").on("click", "button", (event) => {
    const quoteId = event.target.getAttribute("data-bs-quote-id");
    if (!quoteId || quoteId <= 0) return;

    $.get(`/BookQuote/GetQuote/${quoteId}`).done((response) => {
      const form = $("#editQuoteForm");

      form.find("#editQuoteId").val(response.id);
      form.find("#editQuoteBookId").val(bookPageBookId);
      form.find("#editQuotePage").val(response.page);
      form.find("#editQuoteText").val(response.text);

      editQuoteModalBs.show();
    });
  });

  $("#editQuoteForm").on("submit", (event) => {
    event.preventDefault();

    const form = $(event.target);
    const validator = form.validate();
    if (!validator.valid()) return;
    const formData = form.serialize();

    $.post("/BookQuote/EditQuote", formData)
      .done((response, status, xhr) => {
        if (tryShowErrors(xhr, validator)) return;

        const quote = xhr.responseJSON.data;
        const item = createQuoteItem(quote);
        $(`button[data-bs-quote-id="${quote.id}"]`).replaceWith(item);

        editQuoteModalBs.hide();
      })
      .fail((xhr, status, error) => {
        tryShowErrors(xhr, validator);
      });
  });

  $("#deleteQuoteButton").on("click", () => {
    const form = $("#editQuoteForm");
    const validator = form.validate();
    const quoteId = form.find("#editQuoteId").val();
    if (!quoteId || quoteId <= 0) return;

    $.ajax({
      url: `/BookQuote/RemoveQuote/${quoteId}`,
      method: "DELETE",
    })
      .done((response, status, xhr) => {
        if (tryShowErrors(xhr, validator)) return;

        const quote = xhr.responseJSON.data;
        $(`button[data-bs-quote-id="${quote.id}"]`).remove();

        editQuoteModalBs.hide();
      })
      .fail((xhr, status, error) => {
        tryShowErrors(xhr, validator);
      });
  });

  function tryShowErrors(xhr, validator) {
    const response = xhr.responseJSON;
    if (response.success) return false;

    const errors = {
      [response.errorKey]: response.errorMessage,
    };
    validator.showErrors(errors);
    return true;
  }

  function createQuoteItem(quote) {
    return $(`<button class="list-group-item list-group-item-action d-flex justify-content-between align-items-center" data-bs-quote-id="${quote.id}">
                  <h6>${quote.text}</h6>
                  <small>стр. ${quote.page}</small>
              </button>`);
  }
};

export const BookQuote = (function () { $(Module) })();
