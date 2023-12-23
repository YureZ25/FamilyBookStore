using Data.Entities;
using Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Exeptions;
using Services.Services.Contracts;
using System.Security.Claims;
using Web.ComponentViewModels;

namespace Web.Components
{
    public class BooksByStatus : ViewComponent
    {
        private readonly UserManager<User> _userManager;
        private readonly IBookService _bookService;

        public BooksByStatus(UserManager<User> userManager, IBookService bookService)
        {
            _userManager = userManager;
            _bookService = bookService;
        }

        public async Task<IViewComponentResult> InvokeAsync(BookStatus bookStatus)
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId) ?? throw new EntityNotFoundExeption("Пользователь", userId);

            var books = await _bookService.GetBooksAsync(default);

            return View(new BooksByStatusComponentVM { Books = books });
        }
    }
}
