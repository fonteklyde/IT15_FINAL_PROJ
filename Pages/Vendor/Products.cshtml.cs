using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IT15_Final_Proj.Models;
using IT15_Final_Proj.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT15_Final_Proj.Pages.Vendor
{
    public class ProductsModel : PageModel
    {
        private readonly AppDbContext _context;

        public ProductsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Product> Products { get; set; }

        public async Task OnGetAsync()
        {
            Products = await _context.Products.ToListAsync(); // Get all products
        }
    }
}
