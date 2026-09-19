using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QIP.Web.Infrastructure;

public class WardAuthorizeAttribute : ActionFilterAttribute
{
    public const string SessionKey = "WardAccessCode";

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Session.TryGetValue(SessionKey, out _))
        {
            if (string.Equals(context.HttpContext.Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            context.Result = new RedirectToActionResult("Index", "Auth", null);
        }
    }
}
