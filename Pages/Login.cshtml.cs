using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using IT15_Final_Proj.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using IT15_Final_Proj.Models;
using Microsoft.AspNetCore.Http;

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

    public void OnGet() { }

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

        // Create claims for the user
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.MiddleName} {user.LastName}"), // Combine names into one claim
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // Use NameIdentifier for the user's unique ID
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("Cookies", principal);

        // Store full name and role in session
        HttpContext.Session.SetString("Email", user.Email);
        HttpContext.Session.SetString("FullName", $"{user.FirstName} {user.MiddleName} {user.LastName}"); // Full name in one session key
        HttpContext.Session.SetString("Role", user.Role);
        HttpContext.Session.SetString("UserId", user.Id.ToString());

        LoginAttemptTracker.ResetAttempts(Input.Email);

        user.LastLoginDate = DateTime.Now;
        user.IsActive = true;

        if (user.LastLoginDate < DateTime.Now.AddDays(-30))
        {
            user.IsActive = false;
        }

        _context.LoginLogs.Add(new LoginLog
        {
            Email = user.Email,
            Role = user.Role,
            Timestamp = DateTime.Now
        });
        await _context.SaveChangesAsync();

        // Redirect based on user role
        switch (user.Role)
        {
            case "Admin":
                return RedirectToPage("/Admin/Inventory");
            case "Customer":
                return RedirectToPage("/Customer/CustomerProducts");
            case "Vendor":
                return RedirectToPage("/Vendor/Inventory");
            case "Supplier":
                return RedirectToPage("/Supplier/Products");
            default:
                return RedirectToPage("/Index"); // Fallback in case no role matches
        }
    }
}
