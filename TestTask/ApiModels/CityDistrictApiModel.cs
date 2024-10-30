using System.ComponentModel.DataAnnotations;

namespace OrderFilter.ApiModels
{
    public class CityDistrictApiModel
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Максимальная длина строки - 50")]
        public string Name { get; set; }
    }
}
