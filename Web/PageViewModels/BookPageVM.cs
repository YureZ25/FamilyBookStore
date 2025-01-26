using Services.ViewModels.AuthorVMs;
using Services.ViewModels.BookQuoteVMs;
using Services.ViewModels.BookVMs;
using Services.ViewModels.GenreVMs;
using Services.ViewModels.StoreVMs;
using System.ComponentModel.DataAnnotations;

namespace Web.PageViewModels
{
    public class BookPageVM
    {
        public GeneralTab General { get; set; }
        public BookQuotesTab BookQuotes { get; set; }
        public ImageForm Image { get; set; }

        public BookPageVM(GeneralTab general)
        {
            General = general;
        }

        public BookPageVM(GeneralTab general, BookQuotesTab bookQuotes)
        {
            General = general;
            BookQuotes = bookQuotes;
        }

        public class GeneralTab
        {
            public BookPostVM BookPost { get; set; }
            public BookGetVM BookGet { get; set; }
            public IEnumerable<AuthorGetVM> Authors { get; set; }
            public IEnumerable<GenreGetVM> Genres { get; set; }
            public IEnumerable<StoreGetVM> Stores { get; set; }

            public GeneralTab(IEnumerable<AuthorGetVM> authors, IEnumerable<GenreGetVM> genres, IEnumerable<StoreGetVM> stores)
            {
                Authors = authors;
                Genres = genres;
                Stores = stores;
            }

            public GeneralTab(BookGetVM bookGet, IEnumerable<AuthorGetVM> authors, IEnumerable<GenreGetVM> genres, IEnumerable<StoreGetVM> stores)
            {
                BookGet = bookGet;
                Authors = authors;
                Genres = genres;
                Stores = stores;
            }

            public GeneralTab(BookPostVM bookPostVM, IEnumerable<AuthorGetVM> authors, IEnumerable<GenreGetVM> genres, IEnumerable<StoreGetVM> stores)
            {
                BookGet = new BookGetVM
                {
                    Id = bookPostVM.Id ?? 0,
                    Title = bookPostVM.Title,
                    Description = bookPostVM.Description,
                    BookStatus = bookPostVM.BookStatus,
                    CurrentPage = bookPostVM.CurrentPage,
                    PageCount = bookPostVM.PageCount,
                    Isbn = bookPostVM.Isbn,
                    Price = bookPostVM.Price,
                    Author = authors.FirstOrDefault(e => e.Id == bookPostVM.AuthorId),
                    Genre = genres.FirstOrDefault(e => e.Id == bookPostVM.GenreId),
                    Store = stores.FirstOrDefault(e => e.Id == bookPostVM.StoreId),
                };
                Authors = authors;
                Genres = genres;
                Stores = stores;
            }
        }

        public class BookQuotesTab
        {
            public IEnumerable<BookQuoteGetVM> BookQuotes { get; set; }
            public BookQuotePostVM BookQuotePost { get; set; }

            public BookQuotesTab(IEnumerable<BookQuoteGetVM> bookQuotes)
            {
                BookQuotes = bookQuotes;
            }
        }

        public class ImageForm
        {
            [Required(ErrorMessage = "Пожалуйста, выберите обожку книги")]
            public IFormFile ImagePost { get; set; }
        }
    }
}
