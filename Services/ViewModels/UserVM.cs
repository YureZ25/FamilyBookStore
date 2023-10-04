using Data.Entities;

namespace Services.ViewModels
{
    public class UserVM
    {
        public int Id { get; set; }

        public string UserName { get; set; }
    }

    internal static class UserMapper
    {
        public static UserVM Map(this User user)
        {
            return new UserVM
            {
                Id = user.Id,
                UserName = user.UserName,
            };
        }

        public static User Map(this UserVM userVM)
        {
            return new User
            {
                Id = userVM.Id,
                UserName = userVM.UserName,
            };
        }
    }
}
