using Login_Page.Models;
using Login_Page.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Login_Page.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;

        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // ✅ LOGIN (GET)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ✅ LOGIN (POST) with Lockout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                ModelState.AddModelError("", "Your account is locked. Try again after 30 minutes.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Too many failed attempts. Account locked for 30 minutes.");
            }
            else
            {
                await _userManager.AccessFailedAsync(user);
                ModelState.AddModelError("", "Invalid email or password.");
            }

            return View(model);
        }

        // ✅ REGISTER (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ✅ REGISTER (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new Users
            {
                FullName = model.Name,
                Email = model.EmailAddress,
                UserName = model.EmailAddress,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // ✅ VERIFY EMAIL FOR PASSWORD RESET
        [HttpGet]
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.EmailAddress);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            return RedirectToAction("ChangePassword", new { username = user.UserName });
        }

        // ✅ CHANGE PASSWORD (GET)
        [HttpGet]
        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("VerifyEmail");

            return View(new ChangePasswordViewModel { EmailAddress = username });
        }

        // ✅ CHANGE PASSWORD (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(model.EmailAddress);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            var remove = await _userManager.RemovePasswordAsync(user);
            if (!remove.Succeeded)
            {
                foreach (var error in remove.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            var add = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!add.Succeeded)
            {
                foreach (var error in add.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            return RedirectToAction("Login");
        }

        // ✅ EXTERNAL LOGIN (Google, Facebook, LinkedIn)
        [HttpGet]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction("Login");

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email != null)
            {
                var user = new Users { UserName = email, Email = email };
                var identityResult = await _userManager.CreateAsync(user);
                if (identityResult.Succeeded)
                {
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("Login");
        }

        // ✅ LOGOUT
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
