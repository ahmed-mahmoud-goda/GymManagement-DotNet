using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IServiceManager _serviceManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IServiceManager serviceManager, SignInManager<ApplicationUser> signInManager)
        {
            _serviceManager = serviceManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            var analytics = await _serviceManager.AnalyticsService.GetAnalyticsDataAsync();
            ViewBag.Members = analytics.TotalMembers;
            ViewBag.Trainers = analytics.TotalTrainers;
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var user = _serviceManager.AccountService.ValidateUser(input);

            if(user is null)
            {
                ModelState.AddModelError("InvalidLogin", "Your Account Is Not Allowed");
                return View(input);
            }

            var result = _signInManager.PasswordSignInAsync(user, input.Password, input.RememberMe, false).Result;

            if(result.IsNotAllowed)
                ModelState.AddModelError("InvalidLogin", "Your Account Is Not Allowed");
            if(result.IsLockedOut)
                ModelState.AddModelError("InvalidLogin", "Your Account Is Locked Out");
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            return View(input);
        }
        [HttpPost]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync().GetAwaiter().GetResult();
            return RedirectToAction(nameof(Login));
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
