using IT15_Final_Proj.Models;
using IT15_Final_Proj.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IT15_Final_Proj.Pages.Customer
{
    public class OrderSuccessModel : PageModel
    {
        private readonly AppDbContext _context;

        public OrderSuccessModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null) return RedirectToPage("/Login");

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CustomerEmail == email)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToPage("/Customer/Cart");

            // Create Order
            var order = new Order
            {
                CustomerEmail = email,
                OrderDate = DateTime.Now,
                Items = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Price = c.PricePerItem
                }).ToList()
            };
            _context.Orders.Add(order);

            // Update product quantities
            foreach (var item in cartItems)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                }
            }

            // Clear cart
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return Page();
        }
    }

}
