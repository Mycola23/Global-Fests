using GlobalFests.Services;
using GlobalFests.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GlobalFests.Helpers;

namespace GlobalFests.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILookupService _lookupService;

        public AccountController(IUserService userService, ILookupService lookupService)
        {
            _userService = userService;
            _lookupService = lookupService;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var model = new RegisterViewModel
            {
                User = new RegisterUserModel(),
                Countries = await _lookupService.GetAllCountriesAsync()
            };
            return View(model);
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            
            if (!ModelState.IsValid)
            {
                model.Countries = await _lookupService.GetAllCountriesAsync();
                return View(model);
            }

            try
            {
                
                var existingUser = await _userService.GetUserByEmailAsync(model.User.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("User.Email", "A user with this email already exists");
                    model.Countries = await _lookupService.GetAllCountriesAsync();
                    return View(model);
                }

               
                var user = await _userService.RegisterUserAsync(
                    model.User.Username,
                    model.User.Email,
                    model.User.Password,
                    roleId: 2,
                    countryId: model.User.CountryId);

                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                model.Countries = await _lookupService.GetAllCountriesAsync();
                return View(model);
            }
        }

        // GET: Account/Login
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _userService.AuthenticateAsync(model.Email, model.Password);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid email or password");
                    return View(model);
                }

                // Перевірка ролі (наприклад, якщо роль не завантажена)
                if (user.Role == null)
                {
                    ModelState.AddModelError("", "User role not found");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.Role1),
                    new Claim("Verified", user.Verified?.ToString() ?? "False")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                return View(model);
            }
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Profile
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login");

            if (!int.TryParse(userIdClaim.Value, out var userId))
                return RedirectToAction("Login");

            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // GET: Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}