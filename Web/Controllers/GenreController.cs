using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels.GenreVMs;
using Web.PageViewModels;

namespace Web.Controllers
{
    public class GenreController : BaseController
    {
        private readonly IGenreService _genreService;
        private readonly IBookService _bookService;

        public GenreController(IGenreService genreService, IBookService bookService)
        {
            _genreService = genreService;
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GenreList(CancellationToken cancellationToken)
        {
            var genres = await _genreService.GetGenres(cancellationToken);

            return View(genres);
        }

        [HttpGet]
        public async Task<IActionResult> Genre(int? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue) return View(new GenrePageVM());

            return View(new GenrePageVM(await _genreService.GetById(id.Value, cancellationToken))
            {
                GenreBooks = await _bookService.GetBooksByGenre(id.Value, cancellationToken)
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddGenre([FromForm(Name = nameof(GenrePageVM.GenrePost))] GenrePostVM genreVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Genre), new GenrePageVM(genreVM));
            }

            await _genreService.Insert(genreVM, cancellationToken);

            return RedirectToAction(nameof(GenreList));
        }

        [HttpPost]
        public async Task<IActionResult> EditGenre([FromForm(Name = nameof(GenrePageVM.GenrePost))] GenrePostVM genreVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Genre), new GenrePageVM(genreVM)
                {
                    GenreBooks = await _bookService.GetBooksByGenre(genreVM.Id.Value, cancellationToken)
                });
            }

            await _genreService.Update(genreVM, cancellationToken);

            return RedirectToAction(nameof(GenreList));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveGenre([FromForm(Name = nameof(GenrePageVM.GenrePost))] GenrePostVM genreVM, CancellationToken cancellationToken)
        {
            await _genreService.DeleteById(genreVM.Id ?? 0, cancellationToken);

            return RedirectToAction(nameof(GenreList));
        }
    }
}
