using Services.ViewModels.AuthorVMs;
using Services.ViewModels.BookVMs;

namespace Web.PageViewModels
{
    public class AuthorPageVM
    {
        public AuthorGetVM AuthorGet { get; set; }
        public AuthorPostVM AuthorPost { get; set; }
        public IEnumerable<BookGetVM> AuthorBooks { get; set; }

        public AuthorPageVM()
        {
            
        }

        public AuthorPageVM(AuthorGetVM authorGet)
        {
            AuthorGet = authorGet;
        }

        public AuthorPageVM(AuthorPostVM authorPost)
        {
            AuthorGet = new AuthorGetVM
            {
                Id = authorPost.Id ?? 0,
                FirstName = authorPost.FirstName,
                LastName = authorPost.LastName,
            };
        }
    }
}
