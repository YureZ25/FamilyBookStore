using Data.Entities.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class BookQuote : ICommonEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(BookId))]
        public int BookId { get; set; }
        public Book Book { get; set; }

        public string Text { get; set; }

        [Range(1, short.MaxValue)]
        public short Page { get; set; }
    }
}
