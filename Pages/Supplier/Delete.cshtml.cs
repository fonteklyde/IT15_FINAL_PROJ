using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IT15_Final_Proj.Models;
using IT15_Final_Proj.Services;

namespace IT15_Final_Proj.Pages.Supplier
{
    public class DeleteProductModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteProductModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _context.Products.FindAsync(id);
            if (Product == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var productToDelete = await _context.Products.FindAsync(Product.Id);
            if (productToDelete == null)
            {
                return NotFound();
            }

            _context.Products.Remove(productToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Supplier/Products");
        }
    }
}
