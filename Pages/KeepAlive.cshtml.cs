using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IT15_Final_Proj.Pages
{
    public class KeepAliveModel : PageModel
    {
        public IActionResult OnGet()
        {
            return new JsonResult("alive");
        }
    }
}
