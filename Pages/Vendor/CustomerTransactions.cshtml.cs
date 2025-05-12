using IT15_Final_Proj.Models.ViewModels;
using IT15_Final_Proj.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IT15_Final_Proj.Pages.Vendor
{
    public class CustomerTransactionsModel : PageModel
    {
        private readonly AppDbContext _context;

        public CustomerTransactionsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<GroupedOrderViewModel> Transactions { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var vendorEmail = HttpContext.Session.GetString("Email");
            if (vendorEmail == null) return RedirectToPage("/Login");

            var transactions = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Where(o => o.Items.Any(i => i.VendorEmail == vendorEmail)) 
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            Transactions = transactions.Select(order => new GroupedOrderViewModel
            {
                OrderId = order.Id,
                CustomerEmail = order.CustomerEmail,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Items = order.Items
                    .Where(i => i.VendorEmail == vendorEmail) 
                    .Select(i => new CustomerTransactionViewModel
                    {
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList(),
                TotalAmount = order.Items
                    .Where(i => i.VendorEmail == vendorEmail)
                    .Sum(i => i.Price * i.Quantity),
                ShipmentFee = order.ShipmentFee
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostMarkShippedAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order != null && order.Status != "AT SORTING FACILITY")
            {
                order.Status = "AT SORTING FACILITY";
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
