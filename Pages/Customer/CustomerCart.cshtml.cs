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

        [BindProperty]
        public List<CartItem> CartItems { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null) return RedirectToPage("/Login");

            CartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CustomerEmail == email)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateCartAsync()
        {
            foreach (var item in CartItems)
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

        public async Task<IActionResult> OnPostCheckoutAsync()
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null) return RedirectToPage("/Login");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return RedirectToPage("/Login");

            string fullName = $"{user.FirstName} {user.LastName}";

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CustomerEmail == email)
                .ToListAsync();

            if (!cartItems.Any()) return RedirectToPage();

            // Build line items for PayMongo
            var lineItems = cartItems.Select(item => new PayMongoLineItem
            {
                Name = item.Product.Name,
                Amount = (long)(item.PricePerItem * 100), // in centavos per item
                Quantity = item.Quantity
            }).ToList();

            string description = string.Join(", ", cartItems.Select(i => i.Product.Name));

            // Total = sum of (price per item × quantity) in centavos
            long totalAmount = cartItems.Sum(i => (long)(i.PricePerItem * i.Quantity * 100));

            string checkoutUrl = await _payMongoService.CreateCustomerCheckoutLinkAsync(
                fullName,
                email,
                totalAmount,
                description,
                lineItems
            );

            return Redirect(checkoutUrl);
        }
    }
}

