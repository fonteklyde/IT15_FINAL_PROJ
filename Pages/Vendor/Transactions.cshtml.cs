using IT15_Final_Proj.Models;
using IT15_Final_Proj.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IT15_Final_Proj.Pages.Vendor
{
    public class TransactionsModel : PageModel
    {
        private readonly AppDbContext _context;

        public TransactionsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<ProductRequest> PaidRequests { get; set; }

        public async Task<IActionResult> OnGetAsync(int? requestId)
        {
            var vendorEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(vendorEmail))
                return RedirectToPage("/Login");

            if (requestId.HasValue)
            {
                var request = await _context.ProductRequests
                    .Include(r => r.Product)
                    .FirstOrDefaultAsync(r => r.Id == requestId.Value && r.VendorEmail == vendorEmail);

                if (request != null && request.Status == "APPROVED")
                {
                    request.Status = "PAID";
                    request.PaidAt = DateTime.Now;

                    if (request.Product != null)
                    {
                        request.Product.Quantity -= request.Quantity;

                        // Optional safeguard: Don't go below 0
                        if (request.Product.Quantity < 0)
                            request.Product.Quantity = 0;
                    }

                    await _context.SaveChangesAsync();
                }
            }

            PaidRequests = await _context.ProductRequests
                .Where(r => r.VendorEmail == vendorEmail && r.Status == "PAID")
                .Include(r => r.Product)
                .ToListAsync();

            return Page();
        }


    }
}
