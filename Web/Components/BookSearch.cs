using Microsoft.AspNetCore.Mvc;
using Web.ComponentViewModels;

namespace Web.Components
{
    public class BookSearch : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(new BookSearchComponentVM());
        }
    }
}
