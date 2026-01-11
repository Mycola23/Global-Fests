using GlobalFests.Services;
using GlobalFests.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GlobalFests.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GlobalFests.Data;

namespace GlobalFests.Controllers
{
    public class AccountController : Controller
    {
        private readonly GlobalFestsContext _context;
        private readonly IUserService _userService;
        private readonly ILookupService _lookupService;

        public AccountController(GlobalFestsContext context, IUserService userService, ILookupService lookupService)
        {
            _context = context;
            _userService = userService;
            _lookupService = lookupService;

        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var model = new RegisterViewModel
            {
                User = new RegisterUserModel(),
                Countries = await _lookupService.GetAllCountriesAsync(),
                Roles = (await _lookupService.GetAllRolesAsync())
                .Where(r => r.Id is (int)UserRole.User or (int)UserRole.Organizer)
                .ToList()
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
                    roleId: model.User.RoleId ?? (int)UserRole.User,
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
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("Login");
            int userId = int.Parse(userIdClaim.Value);

            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Country)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            var model = new UserProfileViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CountryId = user.CountryId,
                RoleName = user.Role.Role1,
                CountryName = user.Country?.CountryName ?? "Global",
                IsVerified = user.Verified ?? false,
                Countries = new SelectList(await _context.Countries.OrderBy(c => c.CountryName).ToListAsync(), "Id", "CountryName", user.CountryId)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("Login");
            int userId = int.Parse(userIdClaim.Value);

            if (model.Id != userId) return Forbid();

            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return NotFound();


                
                user.Username = model.Username;
                user.Email = model.Email;
                user.CountryId = model.CountryId;


                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    
                    if (string.IsNullOrEmpty(model.CurrentPassword))
                    {
                        ModelState.AddModelError("CurrentPassword", "To change password, you must enter your current password.");
                    }
                    else
                    {
                        
                        bool isCurrentCorrect = _userService.VerifyPassword(model.CurrentPassword, user.PasswordHash, user.Salt);

                        if (!isCurrentCorrect)
                        {
                            ModelState.AddModelError("CurrentPassword", "Incorrect current password.");
                        }
                    }

                    
                    if (!ModelState.IsValid)
                    {
                        model.Countries = new SelectList(await _context.Countries.OrderBy(c => c.CountryName).ToListAsync(), "Id", "CountryName", model.CountryId);
                        return View("Profile", model);
                    }

                    
                    var salt = _userService.GenerateSalt();
                    var hash = _userService.HashPassword(model.NewPassword, salt);
                    user.PasswordHash = hash;
                    user.Salt = salt;
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Profile));
            }

            
            model.Countries = new SelectList(await _context.Countries.OrderBy(c => c.CountryName).ToListAsync(), "Id", "CountryName", model.CountryId);
            return View("Profile", model);
        }

        // POST: /Account/DeleteAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("Login");
            int userId = int.Parse(userIdClaim.Value);

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                await HttpContext.SignOutAsync();
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}