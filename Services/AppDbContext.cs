using Microsoft.EntityFrameworkCore;
using IT15_Final_Proj.Models;

namespace IT15_Final_Proj.Services
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
