namespace IT15_Final_Proj.Models
{
    public class ProductRequest
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string VendorId { get; set; }
        public string VendorEmail { get; set; }
        public string SupplierEmail { get; set; }
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; } = "PENDING";

        public Product Product { get; set; }
    }
}
