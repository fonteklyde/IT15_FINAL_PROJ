using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IT15_Final_Proj.Models;
using IT15_Final_Proj.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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

        public List<ProductRequest> Requests { get; set; }

        public void OnGet()
        {
            var vendorId = HttpContext.Session.GetString("UserId");
            Products = _context.ProductRequests
                .Where(r => r.VendorId == vendorId)
                .ToList();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Login");

            Products = await _context.Products.ToListAsync();

            // Get vendor's requests (if Requests is not already being fetched)
            Requests = await _context.ProductRequests
                .Where(r => r.VendorEmail == email)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostRequestAsync(int productId)
        {
            var vendorEmail = HttpContext.Session.GetString("Email");

            if (string.IsNullOrEmpty(vendorEmail))
                return RedirectToPage("/Login");

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound();

            // Get supplier's email using UserId from the Users table
            var supplier = await _context.Users.FirstOrDefaultAsync(u => u.Id == product.UserId);
            if (supplier == null)
                return NotFound();

            var request = new ProductRequest
            {
                ProductId = productId,
                VendorEmail = vendorEmail,
                SupplierEmail = supplier.Email,
                RequestedAt = DateTime.Now
            };

            _context.ProductRequests.Add(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product request sent.";
            return RedirectToPage();
        }

    }

}
