namespace OrderFilter.DAL.Entities
{
    public class DeliveryOrder
    {
        public int Id { get; set; }
        public List<Order>? Orders { get; set; } = new List<Order>();
        public  CityDistrict? CityDistrict { get; set; }   
        public DateTime StartFilterTime { get; set; }
    }
}
