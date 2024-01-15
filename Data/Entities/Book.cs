using Data.Entities.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class Book : ICommonEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(1024)]
        public string Title { get; set; }

        public string Description { get; set; }

        [DataType("bigint")]
        internal long? IsbnStoreValue { get; set; }
        public ISBN? Isbn
        {
            get => IsbnStoreValue.HasValue ? ISBN.FromStoreValue(IsbnStoreValue.Value) : null; 
            set => IsbnStoreValue = value?.ToStoreValue();
        }

        [Range(1, int.MaxValue)]
        public short? PageCount { get; set; }

        [Range(0, 200_000)]
        [DataType("smallmoney")]
        public decimal? Price { get; set; }

        [ForeignKey(nameof(Image))]
        public int? ImageId { get; set; }
        public BookImage Image { get; set; }

        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }
        public Author Author { get; set; }

        [ForeignKey(nameof(Genre))]
        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        [ForeignKey(nameof(Book2Stores.BookId))]
        public Store Store { get; set; }

        [ForeignKey(nameof(UsersBooksStatus.BookId))]
        public UsersBooksStatus Status { get; set; }
    }
}
