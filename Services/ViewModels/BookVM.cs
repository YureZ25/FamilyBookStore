using Data.Entities;

namespace Services.ViewModels
{
    public class BookVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    internal static class BookMapper
    {
        public static BookVM Map(this Book book)
        {
            return new BookVM
            {
                Id = book.Id,
                Title = book.Title,
            };
        }

        public static Book Map(this BookVM bookVM)
        {
            return new Book
            {
                Id = bookVM.Id,
                Title = bookVM.Title,
            };
        }
    }
}
