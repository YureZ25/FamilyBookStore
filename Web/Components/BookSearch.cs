using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class BookSearch : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
