namespace IT15_Final_Proj.Models.ViewModels
{
    public class PurchasedProductViewModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int QuantityPurchased { get; set; }
        public string PictureUrl { get; set; }
        public decimal MarkupPrice { get; set; }
    }
}
