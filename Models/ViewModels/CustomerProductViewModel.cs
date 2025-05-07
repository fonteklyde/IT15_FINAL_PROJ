namespace IT15_Final_Proj.Models.ViewModels
{
    public class CustomerProductViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public decimal SellingPrice { get; set; }
        public int Quantity { get; set; }
        public string VendorEmail { get; set; }
    }
}
