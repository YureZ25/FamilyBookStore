using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Web.ComponentViewModels;

namespace Web.Components
{
    public class BookQuote : ViewComponent
    {
        private readonly IBookQuoteService _bookQuoteService;

        public BookQuote(IBookQuoteService bookQuoteService)
        {
            _bookQuoteService = bookQuoteService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int bookId)
        {
            var quotes = await _bookQuoteService.GetByBook(bookId, HttpContext.RequestAborted);

            return View(new BookQuoteComponentVM { BookQuotes = quotes, BookId = bookId });
        }
    }
}
