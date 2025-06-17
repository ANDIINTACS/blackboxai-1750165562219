using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAspNetCoreApp.Data;
using MyAspNetCoreApp.Models;
using MyAspNetCoreApp.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MyAspNetCoreApp.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(ApplicationDbContext context, IEmailSender emailSender, ILogger<UserManagementController> logger)
        {
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
        }

        // GET: /UserManagement
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // GET: /UserManagement/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /UserManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: /UserManagement/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: /UserManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                var subject = "Account Deletion Confirmation";
                var message = $"Hello {user.Username}, your account has been deleted successfully.";
                await _emailSender.SendEmailAsync(user.Email, subject, message);

                _logger.LogInformation("User {Username} deleted and email confirmation sent.", user.Username);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
