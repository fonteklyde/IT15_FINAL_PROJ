using IT15_Final_Proj.Models;
using IT15_Final_Proj.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace IT15_Final_Proj.Pages.Supplier;
public class EditModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public EditModel(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [BindProperty]
    public Product Product { get; set; }

    [BindProperty]
    public IFormFile? ProductImage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Product = await _context.Products.FindAsync(id);
        if (Product == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existingProduct = await _context.Products.FindAsync(Product.Id);
        if (existingProduct == null)
        {
            return NotFound();
        }

        existingProduct.Name = Product.Name;
        existingProduct.Price = Product.Price;
        existingProduct.Quantity = Product.Quantity;

        if (ProductImage != null)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProductImage.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ProductImage.CopyToAsync(stream);
            }

            existingProduct.PictureUrl = "/uploads/" + fileName;
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("/Supplier/Products");
    }
}
