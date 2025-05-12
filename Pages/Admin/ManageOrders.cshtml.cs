using IT15_Final_Proj.Models;
using IT15_Final_Proj.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IT15_Final_Proj.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageOrdersModel : PageModel
    {
        private readonly AppDbContext _context;

        public ManageOrdersModel(AppDbContext context)
        {
            _context = context;
        }

        public List<OrderWithVendorInfo> ShippedOrders { get; set; }

        public async Task OnGetAsync()
        {
            ShippedOrders = await _context.Orders
            .Where(o => o.Status == "AT SORTING FACILITY" || o.Status == "OUT FOR DELIVERY" || o.Status == "DELIVERED")
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderWithVendorInfo
            {
                Order = o,
                VendorEmail = o.Items
                    .Select(i => _context.ProductRequests
                        .Where(pr => pr.ProductId == i.ProductId)
                        .Select(pr => pr.VendorEmail)
                        .FirstOrDefault())
                    .FirstOrDefault() ?? "N/A"
            })
            .ToListAsync();
        }
        public async Task<IActionResult> OnPostMarkOutForDeliveryAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status != "AT SORTING FACILITY")
            {
                return NotFound();
            }

            order.Status = "OUT FOR DELIVERY";
            await _context.SaveChangesAsync();

            return RedirectToPage(); 
        }
    }
}
