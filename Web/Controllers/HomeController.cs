﻿using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStoreService _storeService;

        public HomeController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var stores = await _storeService.GetUserStoresOverviewAsync(666, cancellationToken);

            return View(stores);
        }
    }
}
