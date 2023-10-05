using Data.Entities;

namespace Services.ViewModels
{
    public class BookVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public AuthorVM Author { get; set; }
        public GenreVM Genre { get; set; }
        public StoreVM Store { get; set; }
    }

    internal static class BookMapper
    {
        public static BookVM Map(this Book book)
        {
            return new BookVM
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Author = book.Author.Map(),
                Genre = book.Genre.Map(),
                Store = book.Store?.Map()
            };
        }

        public static Book Map(this BookVM bookVM)
        {
            return new Book
            {
                Id = bookVM.Id,
                Title = bookVM.Title,
                Description = bookVM.Description,
                AuthorId = bookVM.Author.Id,
                Author = bookVM.Author.Map(),
                GenreId = bookVM.Genre.Id,
                Genre = bookVM.Genre.Map(),
                Store = bookVM.Store?.Map(),
            };
        }
    }
}
