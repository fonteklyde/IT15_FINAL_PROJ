using IT15_Final_Proj.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using IT15_Final_Proj.Models;

namespace IT15_Final_Proj.Pages.Admin

{

    [Authorize(Roles = "Admin")]
    public class ManageUsersModel : PageModel
    {
        private readonly AppDbContext _context;

        public ManageUsersModel(AppDbContext context)
        {
            _context = context;
        }

        public List<User> Users { get; set; }

        [BindProperty]
        public User EditUser { get; set; }

        [BindProperty]
        public int DeleteUserId { get; set; }

        public void OnGet()
        {
            var threshold = DateTime.Now.AddDays(-30);
            var inactiveUsers = _context.Users
                .Where(u => u.LastLoginDate < threshold && u.IsActive && u.Role != "Admin")
                .ToList();

            foreach (var user in inactiveUsers)
            {
                user.IsActive = false;
            }
            _context.SaveChanges();

            Users = _context.Users
                .Where(u => u.Role != "Admin")
                .ToList();
        }
    }
}

