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
        internal ulong ISBNStoreValue { get; set; }
        public ISBN ISBN
        {
            get => ISBN.FromStoreValue(ISBNStoreValue); 
            set => ISBNStoreValue = value.ToStoreValue();
        }

        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }
        public Author Author { get; set; }

        [ForeignKey(nameof(Genre))]
        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        [ForeignKey(nameof(Book2Stores.BookId))]
        public Store Store { get; set; }
    }
}
