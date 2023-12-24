using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;

namespace Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IStoreService _storeService;
        private readonly IAuthService _authService;

        public HomeController(IStoreService storeService, IAuthService authService)
        {
            _storeService = storeService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser();

            var stores = await _storeService.GetUserStoresOverviewAsync(user.Id, cancellationToken);

            return View(stores);
        }
    }
}
