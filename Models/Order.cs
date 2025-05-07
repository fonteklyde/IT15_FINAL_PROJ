namespace IT15_Final_Proj.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}
