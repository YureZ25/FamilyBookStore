using Data.Entities.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Author : ICommonEntity
    {
        public int Id { get; set; }

        [StringLength(1024)]
        public string FirstName { get; set; }

        [StringLength(1024)]
        public string LastName { get; set; }
    }
}
