using Microsoft.AspNetCore.Mvc;
using QIP.Web.Infrastructure;
using QIP.Web.ViewModels;

namespace QIP.Web.Controllers;

public class AuthController(IConfiguration configuration) : Controller
{
    private readonly HashSet<string> _allowedCodes = configuration.GetSection("WardAccessCodes").Get<string[]>()?.ToHashSet(StringComparer.Ordinal) ?? [];

    [HttpGet]
    public IActionResult Index()
    {
        if (HttpContext.Session.TryGetValue(WardAuthorizeAttribute.SessionKey, out _))
        {
            return RedirectToAction("Index", "Patients");
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.ErrorMessage = "Enter one of the 4 digit access codes.";
            return View("Index", model);
        }

        if (_allowedCodes.Count == 0 || !_allowedCodes.Contains(model.Code))
        {
            model.ErrorMessage = "Invalid access code.";
            return View("Index", model);
        }

        HttpContext.Session.SetString(WardAuthorizeAttribute.SessionKey, model.Code);
        return RedirectToAction("Index", "Patients");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
}
