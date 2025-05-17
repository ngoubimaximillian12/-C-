using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Petition.Models;
using Petition.ViewModels;
using System.Threading.Tasks;

namespace Petition.Controllers
{
    public class MembersController : Controller
    {
        private readonly UserManager<Member> _userManager;
        private readonly SignInManager<Member> _signInManager;

        public MembersController(UserManager<Member> userManager, SignInManager<Member> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Member { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, Role = "Member" };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Login", "Members");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && user.IsBlocked)
            {
                ModelState.AddModelError(string.Empty, "Your account is blocked.");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid email or password.");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
