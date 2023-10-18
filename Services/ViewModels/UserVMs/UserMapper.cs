using Data.Entities;

namespace Services.ViewModels.UserVMs
{
    internal static class UserMapper
    {
        public static UserGetVM Map(this User user)
        {
            return new UserGetVM
            {
                Id = user.Id,
                UserName = user.UserName,
            };
        }
    }
}
