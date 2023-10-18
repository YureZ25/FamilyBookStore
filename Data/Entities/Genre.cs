using Data.Entities.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Genre : ICommonEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }
    }
}
