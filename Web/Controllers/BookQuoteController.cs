using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels;
using Services.ViewModels.BookQuoteVMs;
using Web.PageViewModels;

namespace Web.Controllers
{
    public class BookQuoteController : BaseController
    {
        private readonly IBookQuoteService _bookQuoteService;

        public BookQuoteController(IBookQuoteService bookQuoteService)
        {
            _bookQuoteService = bookQuoteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuote(int id, CancellationToken cancellationToken)
        {
            return Ok(await _bookQuoteService.GetById(id, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> AddQuote([FromForm(Name = nameof(BookPageVM.BookQuotesTab.BookQuotePost))] BookQuotePostVM bookQuoteVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ToResultVM());
            }

            return Result(await _bookQuoteService.Insert(bookQuoteVM, cancellationToken), e => Ok(e), e => BadRequest(e));
        }

        [HttpPost]
        public async Task<IActionResult> EditQuote([FromForm(Name = nameof(BookPageVM.BookQuotesTab.BookQuotePost))] BookQuotePostVM bookQuoteVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ToResultVM());
            }

            return Result(await _bookQuoteService.Update(bookQuoteVM, cancellationToken), e => Ok(e), e => BadRequest(e));
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveQuote(int id, CancellationToken cancellationToken)
        {
            return Result(await _bookQuoteService.DeleteById(id, cancellationToken), e => Ok(e), e => BadRequest(e));
        }
    }
}
