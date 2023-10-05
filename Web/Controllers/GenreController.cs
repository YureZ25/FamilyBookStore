using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels;
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

            return View(new GenrePageVM
            {
                Genre = await _genreService.GetByIdAsync(id.Value, cancellationToken),
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddGenre([FromForm(Name = nameof(GenrePageVM.Genre))] GenreVM genreVM, CancellationToken cancellationToken)
        {
            genreVM = await _genreService.InsertAsync(genreVM, cancellationToken);

            return RedirectToAction(nameof(GenreList));
        }

        [HttpPost]
        public async Task<IActionResult> EditGenre([FromForm(Name = nameof(GenrePageVM.Genre))] GenreVM genreVM, CancellationToken cancellationToken)
        {
            genreVM = await _genreService.UpdateAsync(genreVM, cancellationToken);

            return RedirectToAction(nameof(GenreList));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveGenre([FromForm(Name = nameof(GenrePageVM.Genre))] GenreVM genreVM, CancellationToken cancellationToken)
        {
            genreVM = await _genreService.DeleteByIdAsync(genreVM.Id, cancellationToken);

            return RedirectToAction(nameof(GenreList));
        }
    }
}
