using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IT15_Final_Proj.Services;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using IT15_Final_Proj.Models;
namespace IT15_Final_Proj.Pages.Profile
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User UserProfile { get; set; }

        public class ChangePasswordModel
        {
            [Required(ErrorMessage = "Current password is required")]
            [DataType(DataType.Password)]
            [Display(Name = "Current Password")]
            public string CurrentPassword { get; set; }

            [Required(ErrorMessage = "New password is required")]
            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{12,}$",
                ErrorMessage = "Password must be at least 12 characters long and contain uppercase, lowercase, number, and symbol.")]
            public string NewPassword { get; set; }

            [Required(ErrorMessage = "Confirm password is required")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
            public string ConfirmPassword { get; set; }
        }

        [BindProperty]
        public ChangePasswordModel PasswordModel { get; set; }

        public IActionResult OnGet()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Login");

            UserProfile = _context.Users.FirstOrDefault(u => u.Email == email);
            if (User == null)
                return RedirectToPage("/Login");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var sessionEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(sessionEmail))
                return RedirectToPage("/Login");

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == sessionEmail);
            if (existingUser == null)
                return RedirectToPage("/Login");

            existingUser.FirstName = UserProfile.FirstName;
            existingUser.MiddleName = UserProfile.MiddleName;
            existingUser.LastName = UserProfile.LastName;
            existingUser.Address = UserProfile.Address;
            existingUser.Email = UserProfile.Email;

            if (!string.IsNullOrWhiteSpace(PasswordModel.CurrentPassword) || !string.IsNullOrWhiteSpace(PasswordModel.NewPassword))
            {
                var hasher = new PasswordHasher<User>();
                var passwordCheck = hasher.VerifyHashedPassword(existingUser, existingUser.Password, PasswordModel.CurrentPassword);

                if (passwordCheck == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError("PasswordModel.CurrentPassword", "Current password is incorrect.");
                    return Page();
                }

                if (PasswordModel.NewPassword != PasswordModel.ConfirmPassword)
                {
                    ModelState.AddModelError(string.Empty, "New password and confirmation do not match.");
                    return Page();
                }

                existingUser.Password = hasher.HashPassword(existingUser, PasswordModel.NewPassword);
            }
            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("Email", existingUser.Email); // Update session
            HttpContext.Session.SetString("FullName", existingUser.FirstName);
            HttpContext.Session.SetString("FullName", existingUser.MiddleName);
            HttpContext.Session.SetString("FullName", existingUser.LastName);
            HttpContext.Session.SetString("Address", existingUser.Address);

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToPage("/Profile/Edit");
        }
    }
}
