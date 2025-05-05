using IT15_Final_Proj.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IT15_Final_Proj.Models;

namespace IT15_Final_Proj.Pages
{
    public class LogsModel : PageModel
    {
        private readonly AppDbContext _context;

        public LogsModel(AppDbContext context)
        {
            _context = context;
        }
        public List<LoginLog> Logs { get; set; }
        public IActionResult OnGet()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role))
                return RedirectToPage("/Login");

            var email = HttpContext.Session.GetString("Email");

            Logs = _context.LoginLogs
                           .Where(log => log.Role == role && log.Email == email)
                           .OrderByDescending(log => log.Timestamp)
                           .ToList();

            return Page();
        }
    }
}
