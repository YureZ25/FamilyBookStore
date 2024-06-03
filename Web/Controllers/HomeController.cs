using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels.NavBarVMs;
using Web.ComponentViewModels;

namespace Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IStoreService _storeService;
        private readonly IAuthService _authService;
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;

        public HomeController(
            IStoreService storeService, 
            IAuthService authService, 
            IBookService bookService,
            IAuthorService authorService)
        {
            _storeService = storeService;
            _authService = authService;
            _bookService = bookService;
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser();

            var stores = await _storeService.GetUserStoresOverview(user.Id, cancellationToken);

            return View(stores);
        }

        [HttpGet]
        public async Task<IActionResult> BooksPrompts([FromQuery] string prompt, CancellationToken cancellationToken)
        {
            var prompts = await _bookService.GetBooksPrompts(prompt, cancellationToken);

            return Json(prompts);
        }

        [HttpGet]
        public async Task<IActionResult> AuthorsPrompts([FromQuery] string prompt, CancellationToken cancellationToken)
        {
            var prompts = await _authorService.GetAuthorsPrompts(prompt, cancellationToken);

            return Json(prompts);
        }

        [HttpPost]
        public async Task<IActionResult> Search([FromForm(Name = nameof(BookSearchComponentVM.SearchPost))] SearchPostVM searchVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return NoContent();

            var books = await _bookService.GetBooksByPrompt(searchVM.Prompt, cancellationToken);
            if (books.Count() == 1)
            {
                return RedirectToAction("Book", "Book", new { books.First().Id });
            }

            return View("~/Views/Book/BookList.cshtml", books);
        }
    }
}
