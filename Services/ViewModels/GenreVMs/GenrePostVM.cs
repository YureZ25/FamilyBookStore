using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.GenreVMs
{
    public class GenrePostVM
    {
        public int? Id { get; set; }

        [StringLength(128)]
        [Required(ErrorMessage = "Поле 'Название' обязательно для заполнения")]
        public string Name { get; set; }
    }
}
