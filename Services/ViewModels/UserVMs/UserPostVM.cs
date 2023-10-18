using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.UserVMs
{
    public class UserPostVM
    {
        public int? Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
