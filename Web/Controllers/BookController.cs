using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels.BookVMs;
using System.ComponentModel.DataAnnotations;
using Web.PageViewModels;

namespace Web.Controllers
{
    public class BookController : BaseController
    {
        private readonly IImageService _imageService;
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IGenreService _genreService;
        private readonly IStoreService _storeService;
        private readonly IBookQuoteService _bookQuoteService;

        public BookController(
            IImageService imageService,
            IBookService bookService, 
            IAuthorService authorService, 
            IGenreService genreService,
            IStoreService storeService,
            IBookQuoteService bookQuoteService)
        {
            _imageService = imageService;
            _bookService = bookService;
            _authorService = authorService;
            _genreService = genreService;
            _storeService = storeService;
            _bookQuoteService = bookQuoteService;
        }

        [HttpGet("Image/{bookId?}")]
        public async Task<IActionResult> Image([FromRoute] int? bookId, CancellationToken cancellationToken)
        {
            var (content, contentType) = await _imageService.GetBookImage(bookId, cancellationToken);

            return File(content, contentType);
        }

        [HttpPost("SetImage/{bookId}")]
        public async Task<IActionResult> SetImage(
            [FromRoute] int bookId, 
            [FromForm(Name = $"{nameof(BookPageVM.Image)}.{nameof(BookPageVM.Image.ImagePost)}")] IFormFile image, 
            CancellationToken cancellationToken)
        {
            return await Result(
                await _imageService.SetImage(image, bookId, cancellationToken), 
                () => Book(bookId, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> BookList(CancellationToken cancellationToken)
        {
            var books = await _bookService.GetBooks(cancellationToken);

            return View(books);
        }

        [HttpGet]
        public async Task<IActionResult> Book(int? id, CancellationToken cancellationToken)
        {
            var model = new BookPageVM(new(
                    await _authorService.GetAuthors(cancellationToken),
                    await _genreService.GetGenres(cancellationToken),
                    await _storeService.GetStores(cancellationToken)));

            if (!id.HasValue) return View(model);

            model.General.BookGet = await _bookService.GetById(id.Value, cancellationToken);
            model.BookQuotes = new(await _bookQuoteService.GetByBook(id.Value, cancellationToken));

            return View(nameof(Book), model);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromForm(Name = nameof(BookPageVM.GeneralTab.BookPost))] BookPostVM bookVM, CancellationToken cancellationToken)
        {
            var bookPageVM = new BookPageVM(new(bookVM,
                    await _authorService.GetAuthors(cancellationToken),
                    await _genreService.GetGenres(cancellationToken),
                    await _storeService.GetStores(cancellationToken)));

            if (!ModelState.IsValid)
            {
                return View(nameof(Book), bookPageVM);
            }

            return Result(
                await _bookService.Insert(bookVM, cancellationToken), 
                (e) => RedirectToAction(nameof(Book), new { id = e.Data.Id }), 
                (e) => View(nameof(Book), bookPageVM));
        }

        [HttpPost]
        public async Task<IActionResult> EditBook([FromForm(Name = nameof(BookPageVM.GeneralTab.BookPost))] BookPostVM bookVM, CancellationToken cancellationToken)
        {
            var bookPageVM = new BookPageVM(
                new(bookVM,
                    await _authorService.GetAuthors(cancellationToken),
                    await _genreService.GetGenres(cancellationToken),
                    await _storeService.GetStores(cancellationToken)),
                new(await _bookQuoteService.GetByBook(bookVM.Id.Value, cancellationToken)));

            if (!ModelState.IsValid)
            {
                return View(nameof(Book), bookPageVM);
            }

            return Result(
                await _bookService.Update(bookVM, cancellationToken),
                (e) => RedirectToAction(nameof(Book), new { id = e.Data.Id }),
                (e) => View(nameof(Book), bookPageVM));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBook([FromForm(Name = nameof(BookPageVM.GeneralTab.BookPost))] BookPostVM bookVM, CancellationToken cancellationToken)
        {
            return Result(
                await _bookService.DeleteById(bookVM.Id ?? 0, cancellationToken),
                (r) => RedirectToAction(nameof(BookList)),
                (r) => BadRequest(r.ErrorMessage));
        }
    }
}
