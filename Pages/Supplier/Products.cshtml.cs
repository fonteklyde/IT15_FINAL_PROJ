using Microsoft.AspNetCore.Mvc.RazorPages;
using IT15_Final_Proj.Models;
using IT15_Final_Proj.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IT15_Final_Proj.Pages.Supplier
{
    public class ProductsModel : PageModel
    {
        private readonly AppDbContext _context;

        public ProductsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Product> Products { get; set; }

        public IActionResult OnGet()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToPage("/Login");
            }

            int supplierId = int.Parse(userIdClaim.Value);

            Products = _context.Products
                .Where(p => p.UserId == supplierId)
                .ToList();

            return Page();
        }
    }
}
