using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.ViewModels;

namespace Web.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        public IActionResult Result(ResultVM resultVM, Func<IActionResult> successResult, Func<IActionResult> errorResult)
        {
            if (resultVM.Success)
            {
                return successResult();
            }
            else
            {
                ModelState.AddModelError(resultVM.ErrorKey, resultVM.ErrorMessage);

                return errorResult();
            }
        }
    }
}
