using Microsoft.AspNetCore.Mvc;
using Web.ComponentViewModels;

namespace Web.Components
{
    public class BookSearch : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(new BookSearchComponentVM());
        }
    }
}
