using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.AuthorVMs
{
    public class AuthorPostVM
    {
        public int? Id { get; set; }

        [StringLength(1024)]
        [Required(ErrorMessage = "Поле 'Имя' обязательно для заполнения")]
        public string FirstName { get; set; }

        [StringLength(1024)]
        [Required(ErrorMessage = "Поле 'Фамилия' обязательно для заполнения")]
        public string LastName { get; set; }
    }
}
