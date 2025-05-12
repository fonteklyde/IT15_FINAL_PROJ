using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IT15_Final_Proj.Models;
using IT15_Final_Proj.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace IT15_Final_Proj.Pages.Supplier
{
    public class AddProductModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AddProductModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }

            return Page(); // Stay on the same Razor Page
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToPage("/Login");

            Product.UserId = int.Parse(userIdClaim.Value);

            try
            {
                if (Product.ProductImage != null)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Product.ProductImage.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Product.ProductImage.CopyToAsync(stream);
                    }

                    Product.PictureUrl = "/uploads/" + fileName;
                }

                _context.Products.Add(Product);
                await _context.SaveChangesAsync();
                return RedirectToPage("/Supplier/Products");
            }
            catch (Exception ex)
            {
                // Log error and show a generic message
                ModelState.AddModelError(string.Empty, "An error occurred while adding the product. Please try again.");
                Console.WriteLine(ex.Message);
                return Page();
            }
        }
    }
}
