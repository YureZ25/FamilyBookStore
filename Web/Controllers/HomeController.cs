using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels;
using System.Threading;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;

        public HomeController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var books = await _bookService.GetBooksAsync(cancellationToken);

            return View(books);
        }

        [HttpGet]
        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromForm] BookVM bookVM, CancellationToken cancellationToken)
        {
            bookVM = await _bookService.InsertAsync(bookVM, cancellationToken);

            return View(bookVM);
        }

        [HttpGet]
        public async Task<IActionResult> EditBook(int id, CancellationToken cancellationToken)
        {
            var bookVM = await _bookService.GetById(id, cancellationToken);

            return View(bookVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditBook([FromForm] BookVM bookVM, CancellationToken cancellationToken)
        {
            bookVM = await _bookService.UpdateAsync(bookVM, cancellationToken);

            return View(bookVM);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveBook(int id, CancellationToken cancellationToken)
        {
            var bookVM = await _bookService.GetById(id, cancellationToken);

            return View(bookVM);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBook([FromForm] BookVM bookVM, CancellationToken cancellationToken)
        {
            bookVM = await _bookService.DeleteByIdAsync(bookVM.Id, cancellationToken);

            return View(bookVM);
        }
    }
}
