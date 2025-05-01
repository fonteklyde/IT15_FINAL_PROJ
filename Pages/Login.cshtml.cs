using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using IT15_Final_Proj.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using IT15_Final_Proj.Models;
public class LoginModel : PageModel
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _context;

    public LoginModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ErrorMessage { get; set; }

    public class InputModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public void OnGet() {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        if (LoginAttemptTracker.IsLockedOut(Input.Email))
        {
            ErrorMessage = "Too many failed login attempts. Please try again later.";
            return Page();
        }

        var user = _context.Users.FirstOrDefault(u => u.Email == Input.Email);
        if (user == null)
        {
            LoginAttemptTracker.RecordFailedAttempt(Input.Email);
            ErrorMessage = "Invalid email or password.";
            return Page();
        }

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.Password, Input.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            LoginAttemptTracker.RecordFailedAttempt(Input.Email);
            ErrorMessage = "Invalid email or password.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FirstName),
             new Claim(ClaimTypes.Name, user.MiddleName),
              new Claim(ClaimTypes.Name, user.LastName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("Cookies", principal);

        HttpContext.Session.SetString("Email", user.Email);
        HttpContext.Session.SetString("FullName", user.FirstName);
        HttpContext.Session.SetString("FullName", user.MiddleName);
        HttpContext.Session.SetString("FullName", user.LastName);
        HttpContext.Session.SetString("Role", user.Role);

        LoginAttemptTracker.ResetAttempts(Input.Email);

        switch (user.Role)
        {
            case "Admin":
                return RedirectToPage("/Admin/Dashboard");
            case "Customer":
                return RedirectToPage("/Admin/Dashboard");
            case "Vendor":
                return RedirectToPage("/Admin/Dashboard");
            case "Supplier":
                return RedirectToPage("/Admin/Dashboard");
            case "Driver":
                return RedirectToPage("/Admin/Dashboard");
            default:
                return RedirectToPage("/Index"); // fallback
        }
    }
}
