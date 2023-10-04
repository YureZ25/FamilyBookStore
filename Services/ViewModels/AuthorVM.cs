using Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels
{
    public class AuthorVM
    {
        public int Id { get; set; }

        [StringLength(1024)]
        public string FirstName { get; set; }
         
        [StringLength(1024)]
        public string LastName { get; set; }
    }

    internal static class AuthorMapper
    {
        public static AuthorVM Map(this Author author)
        {
            return new AuthorVM
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
            };
        }

        public static Author Map(this AuthorVM authorVM)
        {
            return new Author
            {
                Id = authorVM.Id,
                FirstName = authorVM.FirstName,
                LastName = authorVM.LastName,
            };
        }
    }
}
