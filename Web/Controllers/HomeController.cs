using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;

namespace Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IStoreService _storeService;

        public HomeController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var stores = await _storeService.GetUserStoresOverviewAsync(1, cancellationToken);

            return View(stores);
        }
    }
}
