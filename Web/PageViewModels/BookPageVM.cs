using Services.ViewModels.AuthorVMs;
using Services.ViewModels.BookVMs;
using Services.ViewModels.GenreVMs;
using Services.ViewModels.StoreVMs;

namespace Web.PageViewModels
{
    public class BookPageVM
    {
        public BookGetVM BookGet { get; set; }
        public BookPostVM BookPost { get; set; }
        public IEnumerable<AuthorGetVM> Authors { get; set; }
        public IEnumerable<GenreGetVM> Genres { get; set; }
        public IEnumerable<StoreGetVM> Stores { get; set; }

        public BookPageVM(IEnumerable<AuthorGetVM> authors, IEnumerable<GenreGetVM> genres, IEnumerable<StoreGetVM> stores)
        {
            Authors = authors;
            Genres = genres;
            Stores = stores;
        }

        public BookPageVM(BookGetVM bookGet, IEnumerable<AuthorGetVM> authors, IEnumerable<GenreGetVM> genres, IEnumerable<StoreGetVM> stores)
        {
            BookGet = bookGet;
            Authors = authors;
            Genres = genres;
            Stores = stores;
        }

        public BookPageVM(BookPostVM bookPostVM, IEnumerable<AuthorGetVM> authors, IEnumerable<GenreGetVM> genres, IEnumerable<StoreGetVM> stores)
        {
            BookGet = new BookGetVM
            {
                Id = bookPostVM.Id ?? 0,
                Title = bookPostVM.Title,
                Description = bookPostVM.Description,
                Author = authors.FirstOrDefault(e => e.Id == bookPostVM.AuthorId),
                Genre = genres.FirstOrDefault(e => e.Id == bookPostVM.GenreId),
                Store = stores.FirstOrDefault(e => e.Id == bookPostVM.StoreId),
            };
            Authors = authors;
            Genres = genres;
            Stores = stores;
        }
    }
}
