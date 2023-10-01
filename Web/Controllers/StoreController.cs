using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;

namespace Web.Controllers
{
    public class StoreController : Controller
    {
        private readonly IBookService _bookService;

        public StoreController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IActionResult> StoreBookList(int id, CancellationToken cancellationToken)
        {
            var books = await _bookService.GetBooksByStoreAsync(id, cancellationToken);

            return View(books);
        }
    }
}
