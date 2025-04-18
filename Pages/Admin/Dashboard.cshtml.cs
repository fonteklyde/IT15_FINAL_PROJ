using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IT15_Final_Proj.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        public string FullName { get; set; }
        public void OnGet()
        {
            FullName = HttpContext.Session.GetString("FullName") ?? "Admin";
        }
    }
}
