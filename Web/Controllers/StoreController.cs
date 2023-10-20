using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels.StoreVMs;
using Web.PageViewModels;

namespace Web.Controllers
{
    public class StoreController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IStoreService _storeService;

        public StoreController(IBookService bookService, IStoreService storeService)
        {
            _bookService = bookService;
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<IActionResult> StoreBookList(int id, CancellationToken cancellationToken)
        {
            var books = await _bookService.GetBooksByStoreAsync(id, cancellationToken);

            return View(books);
        }

        [HttpGet]
        public async Task<IActionResult> StoreList(CancellationToken cancellationToken)
        {
            var stores = await _storeService.GetStoresAsync(cancellationToken);

            return View(stores);
        }

        [HttpGet]
        public async Task<IActionResult> Store(int? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue) return View(new StorePageVM());

            return View(new StorePageVM(await _storeService.GetByIdAsync(id.Value, cancellationToken)));
        }

        [HttpPost]
        public async Task<IActionResult> AddStore([FromForm(Name = nameof(StorePageVM.StorePost))] StorePostVM storeVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Store), new StorePageVM(storeVM));
            }

            await _storeService.InsertAsync(storeVM, cancellationToken);

            return RedirectToAction(nameof(StoreList));
        }

        [HttpPost]
        public async Task<IActionResult> EditStore([FromForm(Name = nameof(StorePageVM.StorePost))] StorePostVM storeVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Store), new StorePageVM(storeVM));
            }

            await _storeService.UpdateAsync(storeVM, cancellationToken);

            return RedirectToAction(nameof(StoreList));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveStore([FromForm(Name = nameof(StorePageVM.StorePost))] StorePostVM storeVM, CancellationToken cancellationToken)
        {
            await _storeService.DeleteByIdAsync(storeVM.Id ?? 0, cancellationToken);

            return RedirectToAction(nameof(StoreList));
        }
    }
}
