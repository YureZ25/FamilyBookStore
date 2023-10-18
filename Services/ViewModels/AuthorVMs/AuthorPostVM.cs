using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.AuthorVMs
{
    public class AuthorPostVM
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(1024)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(1024)]
        public string LastName { get; set; }
    }
}
