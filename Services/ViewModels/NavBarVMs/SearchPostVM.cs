using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.NavBarVMs
{
    public class SearchPostVM
    {
        [Required(ErrorMessage = "Поле 'Запрос' обязательно для заполнения")]
        public string Prompt { get; set; }
    }
}
