using Data.Entities.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class Store : ICommonEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(1024)]
        public string Address { get; set; }

        [ForeignKey(nameof(Book2Stores.StoreId))]
        public ICollection<Book> Books { get; set; } = [];

        [ForeignKey(nameof(Users2Stores.StoreId))]
        public ICollection<User> Users { get; set; } = [];
    }
}
