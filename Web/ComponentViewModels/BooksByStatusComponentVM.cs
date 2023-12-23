using Services.ViewModels.BookVMs;

namespace Web.ComponentViewModels
{
    public class BooksByStatusComponentVM
    {
        public IEnumerable<BookGetVM> Books { get; set; }
    }
}
