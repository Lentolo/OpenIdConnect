using Microsoft.AspNetCore.Mvc;

namespace AuthenticationServer.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}