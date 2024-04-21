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

        public HomeController(IStoreService storeService, IAuthService authService, IBookService bookService)
        {
            _storeService = storeService;
            _authService = authService;
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser();

            var stores = await _storeService.GetUserStoresOverview(user.Id, cancellationToken);

            return View(stores);
        }

        [HttpPost]
        public async Task<IActionResult> Search([FromForm(Name = nameof(BookSearchComponentVM.SearchPost))] SearchPostVM searchVM, CancellationToken cancellationToken)
        {
            var books = await _bookService.GetBooksByPrompt(searchVM.Prompt, cancellationToken);

            return View("~/Views/Book/BookList.cshtml", books);
        }
    }
}
