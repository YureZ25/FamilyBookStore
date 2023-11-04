using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels.AuthorVMs;
using Web.PageViewModels;

namespace Web.Controllers
{
    public class AuthorController : BaseController
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

            return View(new AuthorPageVM(await _authorService.GetByIdAsync(id.Value, cancellationToken)));
        }

        [HttpPost]
        public async Task<IActionResult> AddAuthor([FromForm(Name = nameof(AuthorPageVM.AuthorPost))] AuthorPostVM authorVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Author), new AuthorPageVM(authorVM));
            }

            await _authorService.InsertAsync(authorVM, cancellationToken);

            return RedirectToAction(nameof(AuthorList));
        }

        [HttpPost]
        public async Task<IActionResult> EditAuthor([FromForm(Name = nameof(AuthorPageVM.AuthorPost))] AuthorPostVM authorVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Author), new AuthorPageVM(authorVM));
            }

            await _authorService.UpdateAsync(authorVM, cancellationToken);

            return RedirectToAction(nameof(AuthorList));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAuthor([FromForm(Name = nameof(AuthorPageVM.AuthorPost))] AuthorPostVM authorVM, CancellationToken cancellationToken)
        {
            await _authorService.DeleteByIdAsync(authorVM.Id ?? 0, cancellationToken);

            return RedirectToAction(nameof(AuthorList));
        }
    }
}
