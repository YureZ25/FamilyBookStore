using Data.Entities.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Book : ICommonEntity
    {
        [Key]
        public int Id { get; set; }

        [StringLength(1024)]
        public string Title { get; set; }

        public string Description { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        public Store Store { get; set; }
    }
}
