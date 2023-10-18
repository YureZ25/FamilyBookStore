using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.BookVMs
{
    public class BookPostVM
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(1024)]
        public string Title { get; set; }

        public string Description { get; set; }

        public int AuthorId { get; set; }

        public int GenreId { get; set; }

        public int StoreId { get; set; }
    }
}
