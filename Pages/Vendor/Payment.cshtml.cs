using Azure.Core;
using IT15_Final_Proj.Models;
using IT15_Final_Proj.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IT15_Final_Proj.Pages.Vendor
{
    public class PaymentModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly PayMongoService _payMongoService;
        public PaymentModel(AppDbContext context, PayMongoService payMongoService)
        {
            _context = context;
            _payMongoService = payMongoService;
        }

        public ProductRequest RequestData { get; set; }
        public string CheckoutUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(int requestId)
        {
            RequestData = await _context.ProductRequests
                .Include(r => r.Product) // assuming navigation property
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (RequestData == null || RequestData.Status != "APPROVED")
                return RedirectToPage("/Vendor/Products");

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == RequestData.ProductId);
            if (product == null) return RedirectToPage("/Vendor/Products");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == product.UserId);

            string fullName = user.FirstName + " " + user.LastName; // Assuming these fields exist
            long amountInCentavos = (long)(product.Price * 100);
            int quantity = RequestData.Quantity;
            string description = $"Payment for product: {product.Name}";
            requestId = RequestData.Id;

            CheckoutUrl = await _payMongoService.CreateCheckoutLinkAsync(fullName, RequestData.VendorEmail, amountInCentavos, quantity, description, requestId);
            return Redirect(CheckoutUrl); // direct redirect to PayMongo
        }
    }
}
