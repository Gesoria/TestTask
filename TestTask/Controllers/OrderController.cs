using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFilter.DAL;
using OrderFilter.DAL.Entities;

namespace OrderFilter.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class OrderController : Controller
    {
        private readonly MyDbContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(MyDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
        
            _logger = logger;
        }

        [HttpPost]
        [Route("add-order")]
        public async Task<IActionResult> AddOrderAsync(uint cityDistrictId, double weight, DateTime? dateTime)
        {
            var distId = _context.CityDistricts.FirstOrDefault(dis => dis.Id == cityDistrictId);
            if (distId != null)
            {
                if (!dateTime.HasValue)
                {
                    dateTime = DateTime.Now;
                    dateTime = dateTime.Value.AddTicks(-(dateTime.Value.Ticks % TimeSpan.TicksPerSecond));
                }

                Order order = new Order { CityDistrict = distId, Weight = weight, DeliveryDateTime = dateTime.Value };
                _context.Orders.Add(order);
                _context.SaveChanges();
                
                return Ok($"Заказ №{order.Id} добавлен в базу данных");
            }
            else
            {
                return BadRequest("Выбранный район не найден");
            }
        }


        [HttpGet]
        [Route("get-first-order-of-day")]
        public async Task<IActionResult> GetFirstOrderOfDayAsync(uint cityDistrictId, DateTime dateTime)
        {
            _logger.LogInformation($"{DateTime.Now} : Производится поиск заказов для района с идентификатором {cityDistrictId}. Дата: {dateTime}");
            var district = await _context.CityDistricts.FirstOrDefaultAsync(dist => dist.Id == cityDistrictId);

            if (district != null)
            {
                var order = await _context.Orders.Where(dist => dist.CityDistrict.Id == cityDistrictId && dist.DeliveryDateTime.Date == dateTime.Date)
                    .OrderBy(dt => dt.DeliveryDateTime).FirstOrDefaultAsync();

                if (order != null)
                {
                    return Ok($"Первый заказ {dateTime.ToShortDateString()}: заказ № {order.Id}, район: {district.Name}, время: {order.DeliveryDateTime}");
                }
                else
                {
                    _logger.LogWarning($"{DateTime.Now}:  Не найдено ни одного заказа у района {cityDistrictId}. Дата: {dateTime}");
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
