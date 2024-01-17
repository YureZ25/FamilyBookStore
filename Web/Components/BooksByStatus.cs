using Data.Enums;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Web.ComponentViewModels;

namespace Web.Components
{
    public class BooksByStatus : ViewComponent
    {
        private readonly IBookService _bookService;

        public BooksByStatus(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IViewComponentResult> InvokeAsync(BookStatus bookStatus)
        {
            var books = await _bookService.GetUserBooksByStatus(bookStatus, HttpContext.RequestAborted);

            return View(new BooksByStatusComponentVM { Books = books });
        }
    }
}
