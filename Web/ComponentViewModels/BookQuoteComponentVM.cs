using Services.ViewModels.BookQuoteVMs;

namespace Web.ComponentViewModels
{
    public class BookQuoteComponentVM
    {
        public IEnumerable<BookQuoteGetVM> BookQuotes { get; set; }
        public BookQuotePostVM BookQuotePost { get; set; }
        public required int BookId { get; set; }
    }
}
