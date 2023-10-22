using Services.ViewModels.AuthVMs;

namespace Web.PageViewModels
{
    public class RegisterPageVM
    {
        public RegisterPostVM RegisterPost { get; set; }

        public RegisterPageVM()
        {
            
        }

        public RegisterPageVM(RegisterPostVM registerPost)
        {
            RegisterPost = registerPost;
        }
    }
}
