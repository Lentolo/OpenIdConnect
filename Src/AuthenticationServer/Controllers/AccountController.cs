using System.Security.Claims;
using AuthenticationServer.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationServer.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromServices] SignInManager<ApplicationUser> signInManager, LoginViewModel model)
    {
        ViewData["ReturnUrl"] = model.ReturnUrl;

        if (ModelState.IsValid)
        {
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, false, true);
            if(result.Succeeded)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, model.Username)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

                if (Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            ViewData["Errors"] = "Unable to login";
            return View(model);
        }

        ViewData["Errors"] = string.Join("\r\n", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return RedirectToAction(nameof(HomeController.Index), "Home");
    }
}
