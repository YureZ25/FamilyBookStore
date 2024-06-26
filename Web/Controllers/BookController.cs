﻿using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels.BookVMs;
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

        public BookController(
            IImageService imageService,
            IBookService bookService, 
            IAuthorService authorService, 
            IGenreService genreService,
            IStoreService storeService)
        {
            _imageService = imageService;
            _bookService = bookService;
            _authorService = authorService;
            _genreService = genreService;
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<IActionResult> Image(int? bookId, CancellationToken cancellationToken)
        {
            var (content, contentType) = await _imageService.GetBookImage(bookId, cancellationToken);

            return File(content, contentType);
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
            var model = new BookPageVM(
                    await _authorService.GetAuthors(cancellationToken),
                    await _genreService.GetGenres(cancellationToken),
                    await _storeService.GetStores(cancellationToken));

            if (!id.HasValue) return View(model);

            model.BookGet = await _bookService.GetById(id.Value, cancellationToken);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromForm(Name = nameof(BookPageVM.BookPost))] BookPostVM bookVM, CancellationToken cancellationToken)
        {
            var bookPageVM = new BookPageVM(bookVM,
                        await _authorService.GetAuthors(cancellationToken),
                        await _genreService.GetGenres(cancellationToken),
                        await _storeService.GetStores(cancellationToken));

            if (!ModelState.IsValid)
            {
                return View(nameof(Book), bookPageVM);
            }

            return Result(
                await _bookService.Insert(bookVM, cancellationToken), 
                () => RedirectToAction(nameof(BookList)), 
                () => View(nameof(Book), bookPageVM));
        }

        [HttpPost]
        public async Task<IActionResult> EditBook([FromForm(Name = nameof(BookPageVM.BookPost))] BookPostVM bookVM, CancellationToken cancellationToken)
        {
            var bookPageVM = new BookPageVM(bookVM,
                        await _authorService.GetAuthors(cancellationToken),
                        await _genreService.GetGenres(cancellationToken),
                        await _storeService.GetStores(cancellationToken));

            if (!ModelState.IsValid)
            {
                return View(nameof(Book), bookPageVM);
            }

            return Result(
                await _bookService.Update(bookVM, cancellationToken),
                () => RedirectToAction(nameof(BookList)),
                () => View(nameof(Book), bookPageVM));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBook([FromForm(Name = nameof(BookPageVM.BookPost))] BookPostVM bookVM, CancellationToken cancellationToken)
        {
            return Result(
                await _bookService.DeleteById(bookVM.Id ?? 0, cancellationToken),
                (r) => RedirectToAction(nameof(BookList)),
                (r) => BadRequest(r.ErrorMessage));
        }
    }
}
