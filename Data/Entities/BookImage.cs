using Data.Entities.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class BookImage : ICommonEntity
    {
        [Key]
        public int Id { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(64)]
        public string ContentType { get; set; }

        public byte[] Content { get; set; }
    }
}
