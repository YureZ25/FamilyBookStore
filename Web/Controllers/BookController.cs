using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels;

namespace Web.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> BookList(CancellationToken cancellationToken)
        {
            var books = await _bookService.GetBooksAsync(cancellationToken);

            return View(books);
        }

        [HttpGet]
        public async Task<IActionResult> Book(int? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue) return View(null);

            var bookVM = await _bookService.GetById(id.Value, cancellationToken);

            return View(bookVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromForm] BookVM bookVM, CancellationToken cancellationToken)
        {
            bookVM = await _bookService.InsertAsync(bookVM, cancellationToken);

            return RedirectToAction(nameof(BookList));
        }

        [HttpPost]
        public async Task<IActionResult> EditBook([FromForm] BookVM bookVM, CancellationToken cancellationToken)
        {
            bookVM = await _bookService.UpdateAsync(bookVM, cancellationToken);

            return RedirectToAction(nameof(BookList));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBook([FromForm] BookVM bookVM, CancellationToken cancellationToken)
        {
            bookVM = await _bookService.DeleteByIdAsync(bookVM.Id, cancellationToken);

            return RedirectToAction(nameof(BookList));
        }
    }
}
