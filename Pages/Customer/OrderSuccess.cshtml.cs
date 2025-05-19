using System.Text.Json;
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
    public class OrderSuccessModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GraphHopperSettings _graphHopperSettings;

        public OrderSuccessModel(AppDbContext context, IHttpClientFactory httpClientFactory, IOptions<GraphHopperSettings> graphHopperOptions)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _graphHopperSettings = graphHopperOptions.Value;
        }

        private async Task<(double lat, double lon)> GeocodeAddressAsync(string address)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://graphhopper.com/api/1/geocode?q={Uri.EscapeDataString(address)}&limit=1&key={_graphHopperSettings.ApiKey}";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var hit = doc.RootElement.GetProperty("hits")[0];
            var point = hit.GetProperty("point");

            double lat = point.GetProperty("lat").GetDouble();
            double lon = point.GetProperty("lng").GetDouble();
            return (lat, lon);
        }

        public async Task<IActionResult> OnGetAsync(string vendorEmail, string customerEmail)
        {
            var sessionEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(sessionEmail)) return RedirectToPage("/Login");

            if (sessionEmail != customerEmail) return RedirectToPage("/Login");

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CustomerEmail == customerEmail && c.VendorEmail == vendorEmail)
                .ToListAsync();

            if (!cartItems.Any()) return RedirectToPage("/Customer/CustomerCart");

            foreach (var item in cartItems)
            {
                var productRequests = await _context.ProductRequests
                    .Where(p => p.ProductId == item.ProductId && p.Status == "PAID" && p.Quantity > 0)
                    .OrderBy(p => p.Id)
                    .ToListAsync();

                int remainingQty = item.Quantity;
                foreach (var pr in productRequests)
                {
                    if (pr.Quantity >= remainingQty)
                    {
                        pr.Quantity -= remainingQty;
                        remainingQty = 0;
                        break;
                    }
                    else
                    {
                        remainingQty -= pr.Quantity;
                        pr.Quantity = 0;
                    }
                }

                if (remainingQty > 0)
                {
                    TempData["Error"] = $"Insufficient stock for product: {item.Product.Name}";
                    return RedirectToPage("/Customer/CustomerCart");
                }
            }

            var customer = await _context.Users.FirstOrDefaultAsync(c => c.Email == sessionEmail);
            var (lat, lon) = await GeocodeAddressAsync(customer.Address);

            var order = new Order
            {
                CustomerEmail = customerEmail,
                OrderDate = DateTime.Now,
                ShipmentFee = 50,
                DeliveryLatitude = lat,
                DeliveryLongitude = lon,
                Items = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Price = c.PricePerItem,
                    VendorEmail = c.VendorEmail
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Customer/Orders");
        }
    }

}
