using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IT15_Final_Proj.Models;
using IT15_Final_Proj.Services;
using IT15_Final_Proj.Models.ViewModels;

namespace IT15_Final_Proj.Pages.Customer
{
    public class CustomerProductsModel : PageModel
    {
        private readonly AppDbContext _context;

        public CustomerProductsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<CustomerProductViewModel> VendorProducts { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.ProductRequests
                .Where(r => r.Status == "PAID")
                .Include(r => r.Product)
                .GroupBy(r => new
                {
                    r.ProductId,
                    r.Product.Name,
                    r.Product.PictureUrl,
                    r.Product.Price,
                    r.VendorEmail
                });

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(g => g.Key.Name.Contains(SearchTerm));
            }

            VendorProducts = await query
                .Select(g => new CustomerProductViewModel
                {
                    ProductId = g.Key.ProductId,
                    Name = g.Key.Name,
                    PictureUrl = g.Key.PictureUrl,
                    SellingPrice = g.Key.Price * 1.10M,
                    Quantity = g.Sum(r => r.Quantity),
                    VendorEmail = g.Key.VendorEmail
                })
                .Where(p => p.Quantity > 0)
                .ToListAsync();
        }
        [BindProperty]
        public int ProductId { get; set; }

        [BindProperty]
        public string VendorEmail { get; set; }

        [BindProperty]
        public int Quantity { get; set; }

        public async Task<IActionResult> OnPostAddToCartAsync()
        {
            if (Quantity <= 0)
                return RedirectToPage();

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == ProductId);
            if (product == null || product.Quantity < Quantity)
                return RedirectToPage();

            var customerEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(customerEmail))
                return RedirectToPage("/Login");

            var cartItem = new CartItem
            {
                ProductId = ProductId,
                VendorEmail = VendorEmail,
                CustomerEmail = customerEmail,
                Quantity = Quantity,
                PricePerItem = product.Price * 1.10m // Apply markup
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }


    }
}
