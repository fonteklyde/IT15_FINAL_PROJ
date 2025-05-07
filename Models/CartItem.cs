using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IT15_Final_Proj.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        public string VendorEmail { get; set; }

        [Required]
        public string CustomerEmail { get; set; } // From logged-in session

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerItem { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
