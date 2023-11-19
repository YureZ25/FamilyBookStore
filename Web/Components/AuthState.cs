using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.ComponentViewModels;

namespace Web.Components
{
    public class AuthState : ViewComponent
    {
        private readonly UserManager<User> _userManager;

        public AuthState(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!UserClaimsPrincipal.Identity.IsAuthenticated || !UserClaimsPrincipal.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                return View(new AuthStateComponentVM { IsLoggedIn = false });
            }

            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            return View(new AuthStateComponentVM { IsLoggedIn = true, UserName = user.UserName });
        }
    }
}
