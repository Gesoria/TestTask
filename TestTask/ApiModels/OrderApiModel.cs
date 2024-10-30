using System.ComponentModel.DataAnnotations;

namespace OrderFilter.ApiModels
{
    public class OrderApiModel
    {
        [Required]
        [Range(1, uint.MaxValue, ErrorMessage = "Идентификатор района должен быть положительным")]
        public uint CityDistrictId { get; set; }

        [Required]
        [Range(0.001, double.MaxValue, ErrorMessage = "Вес заказа должен быть положительным")]
        public double Weight { get; set; }
        public DateTime? DeliveryDateTime { get; set; }
    }
}
