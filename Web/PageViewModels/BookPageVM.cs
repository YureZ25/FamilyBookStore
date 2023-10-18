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
    }
}
