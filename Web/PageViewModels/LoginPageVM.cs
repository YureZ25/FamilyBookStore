using Services.ViewModels.AuthVMs;

namespace Web.PageViewModels
{
    public class LoginPageVM
    {
        public LoginPostVM LoginPost { get; set; }

        public LoginPageVM()
        {
            
        }

        public LoginPageVM(LoginPostVM loginPost)
        {
            LoginPost = loginPost;
        }
    }
}
