using Data.Entities;
using Data.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.BookVMs
{
    public class BookPostVM
    {
        public int? Id { get; set; }

        [StringLength(1024)]
        [Required(ErrorMessage = "Поле 'Название' обязательно для заполнения")]
        public string Title { get; set; }

        public string Description { get; set; }

        [RegularExpression(
            "^(?=(?:\\D*\\d){10}(?:(?:\\D*\\d){3})?$)[\\d-]+$", 
            ErrorMessage = "Поле ISBN должно содержать 10 или 13 цифр разделенных дефисами")]
        public ISBN? Isbn { get; set; }

        [Range(1, short.MaxValue)]
        public short? CurrentPage { get; set; }

        [Range(1, short.MaxValue)]
        public short? PageCount { get; set; }

        [Range(0, 200_000)] // чтобы поместилось в sql тип smallmoney
        public decimal? Price { get; set; }

        [EnumDataType(typeof(BookStatus))]
        public BookStatus BookStatus { get; set; }

        public int AuthorId { get; set; }

        public int GenreId { get; set; }

        public int StoreId { get; set; }

        public IFormFile Image { get; set; }
    }
}
