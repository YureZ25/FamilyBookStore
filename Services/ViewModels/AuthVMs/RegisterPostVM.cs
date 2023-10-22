using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.AuthVMs
{
    public class RegisterPostVM
    {
        [Required(ErrorMessage = "Поле 'Никнейм' обязательно для заполнения")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Поле 'Пароль' обязательно для заполнения")]
        public string Password { get; set; }
    }
}
