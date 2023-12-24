using Data.Entities;
using Services.ViewModels.AuthorVMs;
using Services.ViewModels.GenreVMs;
using Services.ViewModels.StoreVMs;

namespace Services.ViewModels.BookVMs
{
    internal static class BookMapper
    {
        public static BookGetVM Map(this Book book, Action<BookGetVM> additionalMapping = null)
        {
            var bookVM = new BookGetVM
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                PageCount = book.PageCount,
                Price = book.Price,
                Isbn = book.Isbn,
                Author = book.Author.Map(),
                Genre = book.Genre.Map(),
                Store = book.Store?.Map()
            };

            if (book.Status != null)
            {
                bookVM.BookStatus = book.Status.BookStatus;
                bookVM.CurrentPage = book.Status.CurrentPage;
            }

            additionalMapping?.Invoke(bookVM);

            return bookVM;
        }

        public static Book Map(this BookPostVM bookVM)
        {
            return new Book
            {
                Id = bookVM.Id ?? 0,
                Title = bookVM.Title,
                Description = bookVM.Description,
                PageCount = bookVM.PageCount,
                Price = bookVM.Price,
                Isbn = bookVM.Isbn,
                AuthorId = bookVM.AuthorId,
                Author = new Author { Id = bookVM.AuthorId },
                GenreId = bookVM.GenreId,
                Genre = new Genre { Id = bookVM.GenreId },
                Store = new Store { Id = bookVM.StoreId },
            };
        }
    }
}
