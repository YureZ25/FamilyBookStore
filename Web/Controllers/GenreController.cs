using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels.GenreVMs;
using Web.PageViewModels;

namespace Web.Controllers
{
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> GenreList(CancellationToken cancellationToken)
        {
            var genres = await _genreService.GetGenresAsync(cancellationToken);

            return View(genres);
        }

        [HttpGet]
        public async Task<IActionResult> Genre(int? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue) return View(new GenrePageVM());

            return View(new GenrePageVM(await _genreService.GetByIdAsync(id.Value, cancellationToken)));
        }

        [HttpPost]
        public async Task<IActionResult> AddGenre([FromForm(Name = nameof(GenrePageVM.GenrePost))] GenrePostVM genreVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Genre), new GenrePageVM(genreVM));
            }

            await _genreService.InsertAsync(genreVM, cancellationToken);

            return RedirectToAction(nameof(GenreList));
        }

        [HttpPost]
        public async Task<IActionResult> EditGenre([FromForm(Name = nameof(GenrePageVM.GenrePost))] GenrePostVM genreVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Genre), new GenrePageVM(genreVM));
            }

            await _genreService.UpdateAsync(genreVM, cancellationToken);

            return RedirectToAction(nameof(GenreList));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveGenre([FromForm(Name = nameof(GenrePageVM.GenrePost))] GenrePostVM genreVM, CancellationToken cancellationToken)
        {
            await _genreService.DeleteByIdAsync(genreVM.Id ?? 0, cancellationToken);

            return RedirectToAction(nameof(GenreList));
        }
    }
}
