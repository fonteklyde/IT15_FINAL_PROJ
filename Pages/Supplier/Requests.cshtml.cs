using IT15_Final_Proj.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IT15_Final_Proj.Models;

namespace IT15_Final_Proj.Pages.Supplier
{
    public class RequestsModel : PageModel
    {
        private readonly AppDbContext _context;

        public RequestsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<ProductRequest> Requests { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Login");

            Requests = await _context.ProductRequests
                .Include(r => r.Product)
                .Where(r => r.SupplierEmail == email)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostApproveAsync(int requestId)
        {
            var request = await _context.ProductRequests.FindAsync(requestId);
            if (request != null && request.Status == "PENDING")
            {
                request.Status = "APPROVED";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Request approved.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDenyAsync(int requestId)
        {
            var request = await _context.ProductRequests.FindAsync(requestId);
            if (request != null && request.Status == "PENDING")
            {
                request.Status = "DENIED";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Request denied.";
            }
            return RedirectToPage();
        }

    }
}
