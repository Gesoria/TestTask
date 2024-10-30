using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFilter.DAL;
using OrderFilter.DAL.Entities;

namespace OrderFilter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderFilterController : Controller
    {
        private readonly MyDbContext _context;
        private readonly ILogger<OrderFilterController> _logger;


        public OrderFilterController(MyDbContext context, ILogger<OrderFilterController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Метод выполняет фильтрацию заказов в ближайшие полчаса после времени первого заказа в определенном районе
        /// </summary>
        /// <param name="cityDistrictId">Идентификатор района, в который выполняется доставка</param>
        /// <param name="firstDeliveryDateTime">Время, от которого мы ведем поиск закзов в базе данных</param>
        [HttpGet]
        [Route("get-orders-by-first-delivery-dateTime-30min")]
        public async Task<IActionResult> GetOrdersByFirstDeliveryDateTimeAsync(uint cityDistrictId, DateTime firstDeliveryDateTime, CancellationToken cancellationToken)
        {
            try
            {  //Логирую
                string logInfo = $"Производится поиск района с идентификатором {cityDistrictId} для добавления заказа";
                _logger.LogInformation(logInfo);

                //Сохраняю лог в базу данных
                DeliveryLog log = new DeliveryLog
                {
                    Type = (DAL.Enums.LogType)1,
                    Message = logInfo,
                    TimeStamp = DateTime.Now,
                    Source = HttpContext.Request.Path

                };
                await _context.DeliveryLogs.AddAsync(log);
                await _context.SaveChangesAsync(cancellationToken);

                //Выполняю поиск района в базе данных
                var district = await _context.CityDistricts
                    .FirstOrDefaultAsync(dis => dis.Id == cityDistrictId);

                //Если района нет в бд
                if (district == null)
                {
                    //Логирую
                    logInfo = $"Не удалость найти район с идентификатором {cityDistrictId}";
                    _logger.LogInformation(logInfo);

                    //Сохраняю лог в базу данных
                    log = new DeliveryLog
                    {
                        Type = (DAL.Enums.LogType)2,
                        Message = logInfo,
                        TimeStamp = DateTime.Now,
                        Source = HttpContext.Request.Path

                    };
                    await _context.DeliveryLogs.AddAsync(log);
                    await _context.SaveChangesAsync(cancellationToken);

                    return NotFound(logInfo);
                }

                //Если район был найден
                else
                {
                    //Логирую
                    logInfo = $"Производится поиск заказов";
                    _logger.LogInformation(logInfo);

                    //Сохраняю лог в базу данных
                    log = new DeliveryLog
                    {
                        Type = (DAL.Enums.LogType)1,
                        Message = logInfo,
                        TimeStamp = DateTime.Now,
                        Source = HttpContext.Request.Path

                    };
                    await _context.DeliveryLogs.AddAsync(log);
                    await _context.SaveChangesAsync(cancellationToken);

                    //Определяю временные границы для поиска заказов
                    DateTime startTime = firstDeliveryDateTime;
                    DateTime endTime = firstDeliveryDateTime.AddMinutes(30);

                    //Выполняю поиск заказов с учетом временных рамок 
                    var orders = await _context.Orders

                             .Include(order => order.CityDistrict)
                             .Where(order =>
                              order.CityDistrict.Id == (district.Id)
                              && order.DeliveryDateTime >= startTime
                              && order.DeliveryDateTime <= endTime)
                             .ToListAsync();

                    //Добавляю результат в базу данных
                    DeliveryOrder deliveryOrder = new DeliveryOrder
                    {
                        Orders = orders,
                        StartFilterTime = startTime,
                        CityDistrict = district

                    };

                    await _context.AddAsync(deliveryOrder);
                    await _context.SaveChangesAsync(cancellationToken);


                    //Логирую
                    logInfo = $"Результат был добавлен в базу данных. Время начала фильтрации: {startTime}";
                    _logger.LogInformation(logInfo);

                    //Сохраняю лог в базу данных
                    log = new DeliveryLog
                    {
                        Type = (DAL.Enums.LogType)1,
                        Message = logInfo,
                        TimeStamp = DateTime.Now,
                        Source = HttpContext.Request.Path

                    };
                    await _context.DeliveryLogs.AddAsync(log);
                    await _context.SaveChangesAsync(cancellationToken);


                    return Ok(logInfo);
                }
            }
            catch (Exception ex)
            {
                //Логирую
                string errorMesage = $"Произошла ошибка при выполнении метода {ex.Message}";
                _logger.LogInformation(errorMesage);

                //Сохраняю лог в базу данных
                DeliveryLog errorLog = new DeliveryLog
                {
                    Type = (DAL.Enums.LogType)3,
                    Message = errorMesage,
                    TimeStamp = DateTime.Now,
                    Source = HttpContext.Request.Path
                };

                await _context.DeliveryLogs.AddAsync(errorLog);
                await _context.SaveChangesAsync();

                return StatusCode(500, errorMesage);
            }
        }
    }
}
