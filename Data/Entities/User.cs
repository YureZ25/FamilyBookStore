using Data.Entities.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities
{
    public class User : IdentityUser<int>, ICommonEntity
    {
    }
}
