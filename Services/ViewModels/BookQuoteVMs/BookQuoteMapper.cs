using Data.Entities;

namespace Services.ViewModels.BookQuoteVMs
{
    internal static class BookQuoteMapper
    {
        public static BookQuoteGetVM Map(this BookQuote bookQuote)
        {
            return new BookQuoteGetVM
            {
                Id = bookQuote.Id,
                Text = bookQuote.Text,
                Page = bookQuote.Page,
            };
        }

        public static BookQuote Map(this BookQuotePostVM bookQuoteVM)
        {
            return new BookQuote
            {
                Id = bookQuoteVM.Id ?? 0,
                BookId = bookQuoteVM.BookId,
                Text = bookQuoteVM.Text,
                Page = bookQuoteVM.Page,
            };
        }
    }
}
