using Data.Entities.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Author : ICommonEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(1024)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(1024)]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
