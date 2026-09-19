using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QIP.Web.Infrastructure;
using QIP.Web.Models;

namespace QIP.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (HttpContext.Session.TryGetValue(WardAuthorizeAttribute.SessionKey, out _))
        {
            return RedirectToAction("Index", "Patients");
        }

        return RedirectToAction("Index", "Auth");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
