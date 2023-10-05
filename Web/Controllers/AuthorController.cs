using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels;
using Web.PageViewModels;

namespace Web.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IActionResult> AuthorList(CancellationToken cancellationToken)
        {
            var authors = await _authorService.GetAuthorsAsync(cancellationToken);

            return View(authors);
        }

        [HttpGet]
        public async Task<IActionResult> Author(int? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue) return View(new AuthorPageVM());

            return View(new AuthorPageVM
            {
                Author = await _authorService.GetByIdAsync(id.Value, cancellationToken)
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddAuthor([FromForm(Name = nameof(AuthorPageVM.Author))] AuthorVM authorVM, CancellationToken cancellationToken)
        {
            authorVM = await _authorService.InsertAsync(authorVM, cancellationToken);

            return RedirectToAction(nameof(AuthorList));
        }

        [HttpPost]
        public async Task<IActionResult> EditAuthor([FromForm(Name = nameof(AuthorPageVM.Author))] AuthorVM authorVM, CancellationToken cancellationToken)
        {
            authorVM = await _authorService.UpdateAsync(authorVM, cancellationToken);

            return RedirectToAction(nameof(AuthorList));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAuthor([FromForm(Name = nameof(AuthorPageVM.Author))] AuthorVM authorVM, CancellationToken cancellationToken)
        {
            authorVM = await _authorService.DeleteByIdAsync(authorVM.Id, cancellationToken);

            return RedirectToAction(nameof(AuthorList));
        }
    }
}
