namespace IT15_Final_Proj.Models
{
    public class PayMongoLineItem
    {
        public string Name { get; set; }
        public long Amount { get; set; } // per item, in centavos
        public int Quantity { get; set; }
    }
}
