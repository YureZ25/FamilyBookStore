using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.StoreVMs
{
    public class StorePostVM
    {
        public int? Id { get; set; }

        [StringLength(128)]
        [Required(ErrorMessage = "Поле 'Название' обязательно для заполнения")]
        public string Name { get; set; }

        [StringLength(1024)]
        public string Address { get; set; }
    }
}
