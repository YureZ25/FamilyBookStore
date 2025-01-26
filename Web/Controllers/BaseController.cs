using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.ViewModels;

namespace Web.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        public Task<IActionResult> Result(ResultVM resultVM, Func<Task<IActionResult>> result)
        {
            return Result(resultVM, result, result);
        }

        public async Task<IActionResult> Result(ResultVM resultVM, Func<Task<IActionResult>> successResult, Func<Task<IActionResult>> errorResult)
        {
            if (resultVM.Success)
            {
                return await successResult();
            }
            else
            {
                ModelState.AddModelError(resultVM.ErrorKey, resultVM.ErrorMessage);

                return await errorResult();
            }
        }

        public IActionResult Result(ResultVM resultVM, Func<IActionResult> result)
        {
            return Result(resultVM, result, result);
        }

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

        public IActionResult Result(ResultVM resultVM, Func<ResultVM, IActionResult> successResult, Func<ResultVM, IActionResult> errorResult)
        {
            if (resultVM.Success)
            {
                return successResult(resultVM);
            }
            else
            {
                ModelState.AddModelError(resultVM.ErrorKey, resultVM.ErrorMessage);

                return errorResult(resultVM);
            }
        }

        public IActionResult Result<T>(ResultVM<T> resultVM, Func<ResultVM<T>, IActionResult> successResult, Func<ResultVM<T>, IActionResult> errorResult)
        {
            if (resultVM.Success)
            {
                return successResult(resultVM);
            }
            else
            {
                ModelState.AddModelError(resultVM.ErrorKey, resultVM.ErrorMessage);

                return errorResult(resultVM);
            }
        }
    }
}
