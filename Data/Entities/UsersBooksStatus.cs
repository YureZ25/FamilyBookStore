using Data.Entities.Contracts;
using Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class UsersBooksStatus : ICommonEntity
    {
        [Key]
        public int Id { get; set; }

        [EnumDataType(typeof(BookStatus))]
        public BookStatus BookStatus { get; set; }

        public DateTime? WishRead { get; set; } // TODO: Rename to WillRead?

        public DateTime? StartRead { get; set; }

        [Range(1, int.MaxValue)]
        public short? CurrentPage { get; set; }

        public DateTime? EndRead { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
