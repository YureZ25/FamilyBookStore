using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.UserVMs
{
    public class UserPostVM
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Поле 'Никнейм' обязательно для заполнения")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле 'Пароль' обязательно для заполнения")]
        public string Password { get; set; }
    }
}
