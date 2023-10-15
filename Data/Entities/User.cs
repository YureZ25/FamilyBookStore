using Data.Entities.Contracts;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class User : IdentityUser<int>, ICommonEntity
    {
        [Key]
        public override int Id { get; set; }
    }
}
