using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using System.Security.Claims;

namespace Web.Controllers
{
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var stores = await _storeService.GetUserStoresOverviewAsync(int.Parse(userId), cancellationToken);

            return View(stores);
        }
    }
}
