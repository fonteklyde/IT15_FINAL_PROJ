using IT15_Final_Proj.Models;
using IT15_Final_Proj.Pages.Tools;
using IT15_Final_Proj.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IT15_Final_Proj.Pages.Customer
{
    //asdasd
    public class TrackOrdersModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly string _graphHopperApiKey;

        public TrackOrdersModel(AppDbContext context, IOptions<GraphHopperSettings> options)
        {
            _context = context;
            _graphHopperApiKey = options.Value.ApiKey;
        }

        public Order Order { get; set; }
        public string ApiKey => _graphHopperApiKey;

        public async Task<IActionResult> OnGetAsync(int orderId)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email)) return RedirectToPage("/Login");

            Order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.CustomerEmail == email && o.Id == orderId);

            if (Order == null) return NotFound();
            return Page();
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnGetLocationAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.CustomerEmail != HttpContext.Session.GetString("Email"))
                return NotFound();

            return new JsonResult(new
            {
                lat = order.CurrentLat,
                lon = order.CurrentLon,
                status = order.Status
            });
        }

        public async Task<IActionResult> OnPostMarkDeliveredAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

            order.Status = "DELIVERED";
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, redirectUrl = Url.Page("/Customer/Orders") });
        }
    }
}
