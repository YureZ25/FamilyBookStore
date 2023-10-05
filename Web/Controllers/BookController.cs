using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels;
using Web.PageViewModels;

namespace Web.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IGenreService _genreService;

        public BookController(IBookService bookService, IAuthorService authorService, IGenreService genreService)
        {
            _bookService = bookService;
            _authorService = authorService;
            _genreService = genreService;
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
            var model = new BookPageVM
            {
                Authors = await _authorService.GetAuthorsAsync(cancellationToken),
                Genres = await _genreService.GetGenresAsync(cancellationToken),
            };

            if (!id.HasValue) return View(model);

            model.Book = await _bookService.GetByIdAsync(id.Value, cancellationToken);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromForm(Name = nameof(BookPageVM.Book))] BookVM bookVM, CancellationToken cancellationToken)
        {
            bookVM = await _bookService.InsertAsync(bookVM, cancellationToken);

            return RedirectToAction(nameof(BookList));
        }

        [HttpPost]
        public async Task<IActionResult> EditBook([FromForm(Name = nameof(BookPageVM.Book))] BookVM bookVM, CancellationToken cancellationToken)
        {
            bookVM = await _bookService.UpdateAsync(bookVM, cancellationToken);

            return RedirectToAction(nameof(BookList));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBook([FromForm(Name = nameof(BookPageVM.Book))] BookVM bookVM, CancellationToken cancellationToken)
        {
            bookVM = await _bookService.DeleteByIdAsync(bookVM.Id, cancellationToken);

            return RedirectToAction(nameof(BookList));
        }
    }
}
