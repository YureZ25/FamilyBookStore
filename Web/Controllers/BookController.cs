using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels.BookVMs;
using Web.PageViewModels;

namespace Web.Controllers
{
    public class BookController : BaseController
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IGenreService _genreService;
        private readonly IStoreService _storeService;

        public BookController(
            IBookService bookService, 
            IAuthorService authorService, 
            IGenreService genreService,
            IStoreService storeService)
        {
            _bookService = bookService;
            _authorService = authorService;
            _genreService = genreService;
            _storeService = storeService;
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
            var model = new BookPageVM(
                    await _authorService.GetAuthorsAsync(cancellationToken),
                    await _genreService.GetGenresAsync(cancellationToken),
                    await _storeService.GetStoresAsync(cancellationToken));

            if (!id.HasValue) return View(model);

            model.BookGet = await _bookService.GetByIdAsync(id.Value, cancellationToken);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromForm(Name = nameof(BookPageVM.BookPost))] BookPostVM bookVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    nameof(Book),
                    new BookPageVM(
                        bookVM,
                        await _authorService.GetAuthorsAsync(cancellationToken),
                        await _genreService.GetGenresAsync(cancellationToken),
                        await _storeService.GetStoresAsync(cancellationToken)));
            }

            await _bookService.InsertAsync(bookVM, cancellationToken);

            return RedirectToAction(nameof(BookList));
        }

        [HttpPost]
        public async Task<IActionResult> EditBook([FromForm(Name = nameof(BookPageVM.BookPost))] BookPostVM bookVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    nameof(Book),
                    new BookPageVM(
                        bookVM,
                        await _authorService.GetAuthorsAsync(cancellationToken),
                        await _genreService.GetGenresAsync(cancellationToken),
                        await _storeService.GetStoresAsync(cancellationToken)));
            }

            await _bookService.UpdateAsync(bookVM, cancellationToken);

            return RedirectToAction(nameof(BookList));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBook([FromForm(Name = nameof(BookPageVM.BookPost))] BookPostVM bookVM, CancellationToken cancellationToken)
        {
            await _bookService.DeleteByIdAsync(bookVM.Id ?? 0, cancellationToken);

            return RedirectToAction(nameof(BookList));
        }
    }
}
