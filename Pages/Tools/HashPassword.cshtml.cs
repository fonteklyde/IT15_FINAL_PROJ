using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

namespace IT15_Final_Proj.Pages.Tools
{
    public class HashPasswordModel : PageModel
    {
        [BindProperty]
        public string PlainPassword { get; set; }

        public string HashedPassword { get; set; }
        public void OnGet()
        {
        }
        public void OnPost()
        {
            if (!string.IsNullOrWhiteSpace(PlainPassword))
            {
                var hasher = new PasswordHasher<object>();
                HashedPassword = hasher.HashPassword(null, PlainPassword);
            }
        }
    }
}
