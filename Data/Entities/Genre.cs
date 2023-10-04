using Data.Entities.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Genre : ICommonEntity
    {
        public int Id { get; set; }

        [StringLength(1024)]
        public string Name { get; set; }
    }
}
