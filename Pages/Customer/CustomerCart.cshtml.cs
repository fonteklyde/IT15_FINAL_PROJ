using IT15_Final_Proj.Services;
using IT15_Final_Proj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Azure.Core;

namespace IT15_Final_Proj.Pages.Customer
{
    public class CartModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly PayMongoService _payMongoService;

        public CartModel(AppDbContext context, PayMongoService payMongoService)
        {
            _context = context;
            _payMongoService = payMongoService;
        }
        private const decimal ShippingFee = 50m;

        [BindProperty]
        public List<CartItem> AllCartItems { get; set; }

        [BindProperty]
        public string SelectedVendor { get; set; }

        [BindProperty]
        public List<CartItem> VendorCartItems { get; set; } = new();

        public Dictionary<string, List<CartItem>> CartItemsByVendor { get; set; } = new();

        public decimal Subtotal => CartItemsByVendor.SelectMany(g => g.Value).Sum(i => i.PricePerItem * i.Quantity);
        public decimal TotalWithShipping => Subtotal + ShippingFee;

        public async Task<IActionResult> OnGetAsync()
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null) return RedirectToPage("/Login");

            AllCartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CustomerEmail == email)
                .ToListAsync();

            CartItemsByVendor = AllCartItems
                .GroupBy(c => c.VendorEmail)
                .ToDictionary(g => g.Key, g => g.ToList());

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateCartAsync()
        {

            foreach (var item in VendorCartItems)
            {
                var cartItem = await _context.CartItems.FindAsync(item.Id);
                if (cartItem != null)
                {
                    cartItem.Quantity = item.Quantity;
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCheckoutVendorAsync()
        {
            
            var email = HttpContext.Session.GetString("Email");
            if (email == null || string.IsNullOrEmpty(SelectedVendor))
                return RedirectToPage("/Login");

            var metadata = new Dictionary<string, string>
            {
                { "customer_email", email },
                { "vendor_email", SelectedVendor }
            };

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return RedirectToPage("/Login");

            var vendorItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CustomerEmail == email && c.VendorEmail == SelectedVendor)
                .ToListAsync();

            if (!vendorItems.Any()) return RedirectToPage();

            var lineItems = vendorItems.Select(item => new PayMongoLineItem
            {
                Name = item.Product.Name,
                Amount = (long)(item.PricePerItem * 100), 
                Quantity = item.Quantity
            }).ToList();

            lineItems.Add(new PayMongoLineItem
            {
                Name = "Shipping Fee",
                Amount = (long)(ShippingFee * 100), 
                Quantity = 1
            });

            string fullName = $"{user.FirstName} {user.LastName}";
            string description = string.Join(", ", vendorItems.Select(i => i.Product.Name)) + " + Shipping Fee";
            long totalAmount = vendorItems.Sum(i => (long)(i.PricePerItem * i.Quantity * 100)) + (long)(ShippingFee * 100);

            string checkoutUrl = await _payMongoService.CreateCustomerCheckoutLinkAsync(
                fullName,
                email,
                totalAmount,
                description,
                lineItems,
                new Dictionary<string, string>
                {
                    { "vendor_email", SelectedVendor },
                    { "customer_email", email }
                }
            );

            return Redirect(checkoutUrl);
        }
    }
}

