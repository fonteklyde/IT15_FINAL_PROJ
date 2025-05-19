    namespace IT15_Final_Proj.Models.ViewModels
{
    public class GroupedOrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public List<CustomerTransactionViewModel> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShipmentFee { get; set; }
        public decimal TotalWithShipping => TotalAmount + ShipmentFee;
    }
}
//asdasd