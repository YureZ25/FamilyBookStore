using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels.BookVMs;
using Services.ViewModels.StoreVMs;
using Web.PageViewModels;

namespace Web.Controllers
{
    public class StoreController : BaseController
    {
        private readonly IBookService _bookService;
        private readonly IStoreService _storeService;

        public StoreController(IBookService bookService, IStoreService storeService)
        {
            _bookService = bookService;
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<IActionResult> StoreBookList(int? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue) return View(Enumerable.Empty<BookGetVM>());

            var store = await _storeService.GetById(id.Value, cancellationToken);
            var books = await _bookService.GetBooksByStore(store.Id, cancellationToken);

            ViewData["Title"] = $"Список книг хранилища {store.Name}";
            return View(books);
        }

        [HttpGet]
        public async Task<IActionResult> StoreList(CancellationToken cancellationToken)
        {
            var stores = await _storeService.GetStores(cancellationToken);

            return View(stores);
        }

        [HttpGet]
        public async Task<IActionResult> Store(int? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue) return View(new StorePageVM());

            return View(new StorePageVM(await _storeService.GetById(id.Value, cancellationToken)));
        }

        [HttpPost]
        public async Task<IActionResult> LinkStoreToUser(int? storeId, CancellationToken cancellationToken)
        {
            if (!storeId.HasValue) return BadRequest();

            await _storeService.LinkStoreToUser(storeId.Value, cancellationToken);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UnlinkStoreFromUser(int? storeId, CancellationToken cancellationToken)
        {
            if (!storeId.HasValue) return BadRequest();

            await _storeService.UnlinkStoreFromUser(storeId.Value, cancellationToken);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddStore([FromForm(Name = nameof(StorePageVM.StorePost))] StorePostVM storeVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Store), new StorePageVM(storeVM));
            }

            await _storeService.Insert(storeVM, cancellationToken);

            return RedirectToAction(nameof(StoreList));
        }

        [HttpPost]
        public async Task<IActionResult> EditStore([FromForm(Name = nameof(StorePageVM.StorePost))] StorePostVM storeVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Store), new StorePageVM(storeVM));
            }

            await _storeService.Update(storeVM, cancellationToken);

            return RedirectToAction(nameof(StoreList));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveStore([FromForm(Name = nameof(StorePageVM.StorePost))] StorePostVM storeVM, CancellationToken cancellationToken)
        {
            await _storeService.DeleteById(storeVM.Id ?? 0, cancellationToken);

            return RedirectToAction(nameof(StoreList));
        }
    }
}
