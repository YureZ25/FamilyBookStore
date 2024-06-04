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
        private readonly ISearchService _searchService;


        public HomeController(
            IStoreService storeService, 
            IAuthService authService, 
            ISearchService searchService)
        {
            _storeService = storeService;
            _authService = authService;
            _searchService = searchService;
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
            var prompts = await _searchService.GetBooksPrompts(prompt, cancellationToken);

            return Json(prompts);
        }

        [HttpGet]
        public async Task<IActionResult> AuthorsPrompts([FromQuery] string prompt, CancellationToken cancellationToken)
        {
            var prompts = await _searchService.GetAuthorsPrompts(prompt, cancellationToken);

            return Json(prompts);
        }

        [HttpPost]
        public async Task<IActionResult> Search([FromForm(Name = nameof(BookSearchComponentVM.SearchPost))] SearchPostVM searchVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return NoContent();

            var books = await _searchService.GetBooksByPrompt(searchVM.Prompt, cancellationToken);
            if (books.Count() == 1)
            {
                return RedirectToAction("Book", "Book", new { books.First().Id });
            }

            return View("~/Views/Book/BookList.cshtml", books);
        }
    }
}
