using Data.Entities;
using Services.ViewModels.AuthorVMs;
using Services.ViewModels.GenreVMs;
using Services.ViewModels.StoreVMs;

namespace Services.ViewModels.BookVMs
{
    internal static class BookMapper
    {
        public static BookGetVM Map(this Book book)
        {
            return new BookGetVM
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Author = book.Author.Map(),
                Genre = book.Genre.Map(),
                Store = book.Store?.Map()
            };
        }

        public static Book Map(this BookPostVM bookVM)
        {
            return new Book
            {
                Id = bookVM.Id ?? 0,
                Title = bookVM.Title,
                Description = bookVM.Description,
                AuthorId = bookVM.AuthorId,
                Author = new Author { Id = bookVM.AuthorId },
                GenreId = bookVM.GenreId,
                Genre = new Genre { Id = bookVM.GenreId },
                Store = new Store { Id = bookVM.StoreId },
            };
        }
    }
}
