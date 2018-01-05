using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BluePhyre.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BluePhyre.Web.Controllers
{
    public class AccountController : Controller
    {

        public IActionResult Select()
        {
            return View();
        }

        public async Task Login(string id = "google", string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync(id.ToLower(), new AuthenticationProperties { RedirectUri = returnUrl });
        }

        [Authorize]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home")
            });
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [Authorize]
        public IActionResult Claims()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public IActionResult Profile()
        {
            return View(new UserProfile
            {
                Name = User.Identity.Name,
                EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value
            });
        }
    }
}