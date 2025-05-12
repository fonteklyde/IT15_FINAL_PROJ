using IT15_Final_Proj.Models;
using IT15_Final_Proj.Models.ViewModels;
using IT15_Final_Proj.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IT15_Final_Proj.Pages.Admin
{
    public class InventoryModel : PageModel
    {
        private readonly AppDbContext _context;

        public InventoryModel(AppDbContext context)
        {
            _context = context;
        }

        public List<InventoryEntry> GroupedInventory { get; set; }

        public async Task OnGetAsync()
        {
            GroupedInventory = await _context.OrderItems
                .Include(i => i.Product)
                .Include(i => i.Order)
                .Where(i => i.Order.Status == "AT SORTING FACILITY")
                .GroupBy(i => i.ProductId)
                .Select(g => new InventoryEntry
                {
                    ProductId = g.Key,
                    ProductName = g.First().Product.Name,
                    PictureUrl = g.First().Product.PictureUrl,
                    TotalQuantity = g.Sum(x => x.Quantity)
                })
                .ToListAsync();
        }
    }
}
