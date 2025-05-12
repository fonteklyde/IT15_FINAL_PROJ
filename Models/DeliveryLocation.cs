namespace IT15_Final_Proj.Models
{
    public class DeliveryLocation
    { 
        public int Id { get; set; }
        public int OrderId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
