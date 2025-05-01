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
            Users = _context.Users.ToList();
        }
        public async Task<IActionResult> OnPostEditAsync()
        {
            var user = _context.Users.Find(EditUser.Id);
            if (user != null)
            {
                user.FirstName = EditUser.FirstName;
                user.MiddleName = EditUser.MiddleName;
                user.LastName = EditUser.LastName;
                user.Address = EditUser.Address;
                user.Email = EditUser.Email;
                user.Role = EditUser.Role;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User successfully edited.";
            }

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(DeleteUserId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User successfully deleted.";
            }
            return RedirectToPage();
        }
    }
}

