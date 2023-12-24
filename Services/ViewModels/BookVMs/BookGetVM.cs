using Data.Entities;
using Data.Enums;
using Services.ViewModels.AuthorVMs;
using Services.ViewModels.GenreVMs;
using Services.ViewModels.StoreVMs;

namespace Services.ViewModels.BookVMs
{
    public class BookGetVM
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public ISBN? Isbn { get; set; }
        public decimal? Price { get; set; }

        public BookStatus BookStatus { get; set; }
        public short? PageCount { get; set; }
        public short? CurrentPage { get; set; }
        public short? ReadingProgress => (short?)(CurrentPage / (decimal)PageCount * 100);

        public AuthorGetVM Author { get; set; }
        public GenreGetVM Genre { get; set; }
        public StoreGetVM Store { get; set; }
    }
}
