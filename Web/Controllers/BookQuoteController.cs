using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels;
using Services.ViewModels.BookQuoteVMs;
using Web.ComponentViewModels;

namespace Web.Controllers
{
    public class BookQuoteController : BaseController
    {
        private readonly IBookQuoteService _bookQuoteService;

        public BookQuoteController(IBookQuoteService bookQuoteService)
        {
            _bookQuoteService = bookQuoteService;
        }

        [HttpPost]
        public async Task<IActionResult> AddQuote([FromForm(Name = nameof(BookQuoteComponentVM.BookQuotePost))] BookQuotePostVM bookQuoteVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ToResultVM());
            }

            return Result(
                await _bookQuoteService.Insert(bookQuoteVM, cancellationToken),
                e => Ok(e),
                e => BadRequest(e));
        }
    }
}
