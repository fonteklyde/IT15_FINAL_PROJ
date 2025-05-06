using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IT15_Final_Proj.Models;
using IT15_Final_Proj.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IT15_Final_Proj.Models.ViewModels;

namespace IT15_Final_Proj.Pages.Vendor
{
    public class ProductsModel : PageModel
    {
        

        private readonly AppDbContext _context;

        public ProductsModel(AppDbContext context)
        {
            _context = context;
        }

       
        public List<ProductRequest> VendorRequests { get; set; }

        public List<ProductViewModel> Products { get; set; }



        public async Task<IActionResult> OnGetAsync()
        {
            var email = HttpContext.Session.GetString("Email");
            

            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Login");

            // Get vendor's requests (if Requests is not already being fetched)
            VendorRequests = await _context.ProductRequests
                .Where(r => r.VendorEmail == email && r.Status != "PAID")
                .ToListAsync();

            Products = await (from product in _context.Products
                              join user in _context.Users on product.UserId equals user.Id
                              select new ProductViewModel
                              {
                                  Id = product.Id,
                                  Name = product.Name,
                                  Price = product.Price,
                                  Quantity = product.Quantity,
                                  PictureUrl = product.PictureUrl,
                                  SupplierEmail = user.Email
                              }).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostRequestAsync(int productId, int quantity)
        {
            var vendorEmail = HttpContext.Session.GetString("Email");
            var vendorId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(vendorEmail) || string.IsNullOrEmpty(vendorId))
                return RedirectToPage("/Login");

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound();

            var supplier = await _context.Users.FirstOrDefaultAsync(u => u.Id == product.UserId);
            if (supplier == null)
                return NotFound();

            var request = new ProductRequest
            {
                ProductId = productId,
                VendorEmail = vendorEmail,
                VendorId = vendorId,
                SupplierEmail = supplier.Email,
                RequestedAt = DateTime.Now,
                Quantity = quantity,
                Status = "PENDING"
            };

            _context.ProductRequests.Add(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product request sent.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostPaymentAsync(int requestId)
        {
            var request = await _context.ProductRequests.FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null || request.Status != "Approved")
            {
                TempData["ErrorMessage"] = "Invalid or unapproved request.";
                return RedirectToPage();
            }

            // Redirect to a payment page or initiate payment logic
            // For now, just simulate the redirection
            TempData["SuccessMessage"] = $"Redirecting to payment for Product ID: {request.ProductId}";
            return RedirectToPage("/Vendor/Payment", new { requestId = requestId });
        }
        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var vendorEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(vendorEmail))
                return RedirectToPage("/Login");

            var request = await _context.ProductRequests
                .FirstOrDefaultAsync(r => r.Id == id && r.VendorEmail == vendorEmail);

            if (request == null || request.Status != "PENDING")
                return BadRequest("Only pending requests can be cancelled.");

            _context.ProductRequests.Remove(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Request cancelled successfully.";
            return RedirectToPage();
        }



    }

}
