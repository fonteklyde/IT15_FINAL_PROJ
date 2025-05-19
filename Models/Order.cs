namespace IT15_Final_Proj.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public string Status { get; set; } = "PROCESSING";
        public decimal ShipmentFee { get; set; }
        public double? DeliveryLatitude { get; set; }
        public double? DeliveryLongitude { get; set; }
        public double? CurrentLat { get; set; }
        public double? CurrentLon { get; set; }
        public int? RouteStepIndex { get; set; }
        public string? EncodedRoutePoints { get; set; }
        public decimal GrandTotal => Items?.Sum(i => i.Quantity * i.Price) ?? 0;
        public decimal TotalWithShipping => GrandTotal + ShipmentFee;
    }
}
//asdasd