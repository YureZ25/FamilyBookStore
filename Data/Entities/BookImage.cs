using Data.Entities.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class BookImage : ICommonEntity
    {
        [Key]
        public int Id { get; set; }

        [StringLength(128, ErrorMessage = "Имя файла обложки книги должно быть короче 128 сиволов")]
        public string FileName { get; set; }

        [StringLength(64, ErrorMessage = "Название типа файла обложки книги должно быть короче 64 сиволов")]
        public string ContentType { get; set; }

        [Length(50 * 1024, 5 * 1024 * 1024, ErrorMessage = "Обложка книги должна весить больше 50Кб и меньше 5Мб")]
        public byte[] Content { get; set; }
    }
}
