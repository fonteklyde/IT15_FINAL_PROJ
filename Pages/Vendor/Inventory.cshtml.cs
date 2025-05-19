using IT15_Final_Proj.Models;
using IT15_Final_Proj.Models.ViewModels;
using IT15_Final_Proj.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IT15_Final_Proj.Pages.Vendor
{
    public class InventoryModel : PageModel
    {
        //asdasd
        private readonly AppDbContext _context;

        public InventoryModel(AppDbContext context)
        {
            _context = context;
        }

        public List<PurchasedProductViewModel> PurchasedProducts { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var vendorEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(vendorEmail))
                return RedirectToPage("/Login");

            PurchasedProducts = await _context.ProductRequests
                .Where(r => r.VendorEmail == vendorEmail && r.Status == "PAID")
                .Include(r => r.Product)
                .GroupBy(r => r.Product.Id)
                .Select(g => new PurchasedProductViewModel
                {
                    Name = g.First().Product.Name,
                    Price = g.First().Product.Price,
                    QuantityPurchased = g.Sum(x => x.Quantity),
                    PictureUrl = g.First().Product.PictureUrl
                })
                .Where(p => p.QuantityPurchased > 0)
                .ToListAsync();

            return Page();
        }
    }
}
