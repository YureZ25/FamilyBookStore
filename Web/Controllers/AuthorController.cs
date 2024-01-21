using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels.AuthorVMs;
using Web.PageViewModels;

namespace Web.Controllers
{
    public class AuthorController : BaseController
    {
        private readonly IAuthorService _authorService;
        private readonly IBookService _bookService;

        public AuthorController(IAuthorService authorService, IBookService bookService)
        {
            _authorService = authorService;
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> AuthorList(CancellationToken cancellationToken)
        {
            var authors = await _authorService.GetAuthors(cancellationToken);

            return View(authors);
        }

        [HttpGet]
        public async Task<IActionResult> Author(int? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue) return View(new AuthorPageVM());

            return View(new AuthorPageVM(await _authorService.GetById(id.Value, cancellationToken))
            {
                AuthorBooks = await _bookService.GetBooksByAuthor(id.Value, cancellationToken)
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddAuthor([FromForm(Name = nameof(AuthorPageVM.AuthorPost))] AuthorPostVM authorVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Author), new AuthorPageVM(authorVM));
            }

            await _authorService.Insert(authorVM, cancellationToken);

            return RedirectToAction(nameof(AuthorList));
        }

        [HttpPost]
        public async Task<IActionResult> EditAuthor([FromForm(Name = nameof(AuthorPageVM.AuthorPost))] AuthorPostVM authorVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Author), new AuthorPageVM(authorVM)
                {
                    AuthorBooks = await _bookService.GetBooksByAuthor(authorVM.Id.Value, cancellationToken)
                });
            }

            await _authorService.Update(authorVM, cancellationToken);

            return RedirectToAction(nameof(AuthorList));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAuthor([FromForm(Name = nameof(AuthorPageVM.AuthorPost))] AuthorPostVM authorVM, CancellationToken cancellationToken)
        {
            await _authorService.DeleteById(authorVM.Id ?? 0, cancellationToken);

            return RedirectToAction(nameof(AuthorList));
        }
    }
}
