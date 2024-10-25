namespace OrderFilter.DAL.Entities
{    
    public class Order
    {
        public Guid Id { get; set; }
        public double Weight { get; set; }
        public CityDistrict CityDistrict { get; set; }
        public DateTime DeliveryDateTime { get; set; }
    }
}
