using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.GenreVMs
{
    public class GenrePostVM
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }
    }
}
