using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFilter.DAL;

namespace OrderFilter.Controllers
{
    [ApiController]
    [Route ("[controller]")]

    public class OrderController : Controller
    {
        private readonly MyDbContext _context;

        public OrderController(MyDbContext context) 
        { 
            _context = context;
        }

        [HttpGet]
        [Route("filter-order")]
        public async Task<IActionResult> GetFirstOrderOfDayAsync( uint cityDistrictId, DateTime dateTime)
        {
            var district = _context.CityDistricts.FirstOrDefault(dist => dist.Id == cityDistrictId);

            if (district != null)
            {
                var order = _context.Orders.Where(dist => dist.CityDistrict.Id == cityDistrictId)
                    .OrderBy(dt => dt.DeliveryDateTime == dateTime).FirstOrDefault();

                if (order != null)
                {
                    return Ok($"Первый заказ {dateTime.ToShortDateString()}: заказ № {order.Id}, район: {district.Name}, время: {order.DeliveryDateTime}");
                }
                else
                {
                    return NotFound($"Заказов в районе: {district.Name}, на {dateTime} не найдено.");
                }
                   
            }
            else
            {
                return NotFound("Район не найден.");
            }

        }

    }
}
