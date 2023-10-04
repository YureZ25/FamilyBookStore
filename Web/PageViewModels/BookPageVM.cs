using Services.ViewModels;

namespace Web.PageViewModels
{
    public class BookPageVM
    {
        public BookVM Book { get; set; }
        public IEnumerable<AuthorVM> Authors { get; set; }
        public IEnumerable<GenreVM> Genres { get; set; }
    }
}
