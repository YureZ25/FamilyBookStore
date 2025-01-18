using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.BookQuoteVMs
{
    public class BookQuotePostVM
    {
        public int? Id { get; set; }

        public int BookId { get; set; }

        [Required(ErrorMessage = "Поле 'Цитата' обязательно для заполнения")]
        public string Text { get; set; }

        [Range(1, short.MaxValue)]
        [Required(ErrorMessage = "Поле 'Страница' обязательно для заполнения")]
        public short Page { get; set; }
    }
}
