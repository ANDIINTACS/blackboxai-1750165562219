using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAspNetCoreApp.Data;
using MyAspNetCoreApp.Models;
using System.Security.Claims;

namespace MyAspNetCoreApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            try
            {
                _logger.LogInformation("Login attempt for username: {Username}", username);

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    _logger.LogWarning("Login failed: Empty username or password");
                    ViewBag.Error = "Please enter both username and password.";
                    return View();
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                
                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found - {Username}", username);
                    ViewBag.Error = "Invalid username or password.";
                    return View();
                }

                _logger.LogInformation("User found in database: {Username}", username);

                if (user.PasswordHash != password) // In real app, use proper password hashing
                {
                    _logger.LogWarning("Login failed: Invalid password for user {Username}", username);
                    ViewBag.Error = "Invalid username or password.";
                    return View();
                }

                _logger.LogInformation("Password validated for user: {Username}", username);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                _logger.LogInformation("User {Username} logged in successfully", username);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Username}", username);
                ViewBag.Error = "An error occurred during login. Please try again.";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    _logger.LogWarning("User not found during password change: {Username}", username);
                    return NotFound();
                }

                if (user.PasswordHash != model.CurrentPassword)
                {
                    _logger.LogWarning("Invalid current password during password change for user: {Username}", username);
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect");
                    return View(model);
                }

                user.PasswordHash = model.NewPassword;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Password changed successfully for user: {Username}", username);
                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change");
                ModelState.AddModelError("", "An error occurred while changing the password. Please try again.");
                return View(model);
            }
        }
    }
}
