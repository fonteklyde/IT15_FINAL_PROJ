using IT15_Final_Proj.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using IT15_Final_Proj.Models;
public class RegisterModel : PageModel
{
    private readonly AppDbContext _context;

    public RegisterModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public User NewUser { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (_context.Users.Any(u => u.Email == NewUser.Email))
        {
            ModelState.AddModelError("NewUser.Email", "Email is already in use.");
            return Page();
        }


        var hasher = new PasswordHasher<User>();
        NewUser.Password = hasher.HashPassword(NewUser, NewUser.Password);
        _context.Users.Add(NewUser);
        await _context.SaveChangesAsync();

        ViewData["ShowSuccessModal"] = true;
        return Page();
    }
}
