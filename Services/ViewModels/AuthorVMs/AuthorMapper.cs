using Data.Entities;

namespace Services.ViewModels.AuthorVMs
{
    internal static class AuthorMapper
    {
        public static AuthorGetVM Map(this Author author)
        {
            return new AuthorGetVM
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                FullName = author.FullName,
            };
        }

        public static Author Map(this AuthorPostVM authorVM)
        {
            return new Author
            {
                Id = authorVM.Id ?? 0,
                FirstName = authorVM.FirstName,
                LastName = authorVM.LastName,
            };
        }
    }
}
