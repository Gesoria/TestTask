using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFilter.ApiModels;
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

        /// <summary>
        /// Метод добавления заказа в базу данных
        /// </summary>
        /// <param name="orderModel">Входные данные для создания заказа</param>   
        [HttpPost]
        [Route("add-order")]
        public async Task<IActionResult> AddOrderAsync([FromBody] OrderApiModel orderModel, CancellationToken cancellationToken)
        {
            try
            {
                //Логирую 
                string logInfo = $"Производится поиск района с идентификатором {orderModel.CityDistrictId} для добавления заказа";
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

                //Произвожу поиск района
                var distId = _context.CityDistricts.FirstOrDefault(dis => dis.Id == orderModel.CityDistrictId);

                //Если район был найден
                if (distId != null)
                {
                    //Присваиваю текущее время, если время заказа не указано
                    if (!orderModel.DeliveryDateTime.HasValue)
                    {
                        orderModel.DeliveryDateTime = DateTime.Now;
                        orderModel.DeliveryDateTime = orderModel.DeliveryDateTime.Value.AddTicks(-(orderModel.DeliveryDateTime.Value.Ticks % TimeSpan.TicksPerSecond));
                    }

                    //Сохраняю заказ в базу данных
                    Order order = new Order { CityDistrict = distId, Weight = orderModel.Weight, DeliveryDateTime = orderModel.DeliveryDateTime.Value };
                    await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync(cancellationToken);

                    //Логирую
                    logInfo = $"Заказ с идентификатором {order.Id.ToString()}, район:{order.CityDistrict.Id}, вес:{order.Weight}, дата:{order.DeliveryDateTime} добавлен в базу данных";
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
                //Если района нет в базе данных
                else
                {
                    //Логирую
                    logInfo = $"Район с идентификатором {orderModel.CityDistrictId} не найден";
                    _logger.LogWarning(logInfo);

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
            }
            catch (Exception ex)
            {
                //Логирую
                string errorMessage = $"Произошла ошибка при добавлении заказа {ex.Message}";
                _logger.LogError(errorMessage);

                //Сохраняю лог в базу данных
                DeliveryLog log = new DeliveryLog
                {
                    Type = (DAL.Enums.LogType)3,
                    Message = errorMessage,
                    TimeStamp = DateTime.Now,
                    Source = HttpContext.Request.Path
                };
                await _context.DeliveryLogs.AddAsync(log);
                await _context.SaveChangesAsync();

                return StatusCode(500, errorMessage);
            }
        }

        /// <summary>
        /// Метод получения первого заказа на определенную дату
        /// </summary>
        /// <param name="cityDistrictId">Идентификатор района, в который осуществлена доставка заказа</param>
        /// <param name="dateTime">Предпологаемая дата доставки</param>
        [HttpGet]
        [Route("get-first-order-of-day")]
        public async Task<IActionResult> GetFirstOrderOfDayAsync(uint cityDistrictId, DateTime dateTime, CancellationToken cancellationToken)
        {
            try
            {
                //Логирую 
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

                //Произвожу поиск района
                var districtId = await _context.CityDistricts.FirstOrDefaultAsync(dist => dist.Id == cityDistrictId);

                //Если район был найден
                if (districtId != null)
                {
                    //Получаю заказ из базы данных
                    var order = await _context.Orders
                        .AsNoTracking()
                        .Where(dist => dist.CityDistrict.Id == cityDistrictId && dist.DeliveryDateTime.Date == dateTime.Date)
                        .OrderBy(dt => dt.DeliveryDateTime)
                        .FirstOrDefaultAsync(cancellationToken);

                    //Если заказ был найден
                    if (order != null)
                    {
                        //Логирую 
                        logInfo = $"Получение первого заказа с идентификатором {order.Id} район:{districtId} дата:{dateTime.Date}";
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

                        return Ok($"Первый заказ {dateTime.ToShortDateString()}: заказ с идентификатором {order.Id}, район: {districtId.Name}, время: {order.DeliveryDateTime}");
                    }

                    //Если заказа нет в базе данных
                    else
                    {
                        //Логирую
                        logInfo = $"Заказов в районе: {districtId.Id}, дата: {dateTime.ToShortDateString()} не найдено";
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

                        return NotFound($"Заказов в районе: {districtId.Name}, дата: {dateTime.ToShortDateString()} не найдено");
                    }
                }

                //Если района нет в базе данных
                else
                {
                    //Логирую
                    logInfo = $"Район с идентификатором {cityDistrictId} не найден";
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

                    return NotFound($"{logInfo}");
                }
            }
            catch (Exception ex)
            {
                //Логирую
                string errorMessage = $"Произошла ошибка при поиске заказа {ex.Message}";
                _logger.LogError(errorMessage);

                //Сохраняю лог в базу данных
                DeliveryLog log = new DeliveryLog
                {
                    Type = (DAL.Enums.LogType)3,
                    Message = errorMessage,
                    TimeStamp = DateTime.Now,
                    Source = HttpContext.Request.Path
                };
                _context.DeliveryLogs.Add(log);
                await _context.SaveChangesAsync();

                return StatusCode(500, errorMessage);
            }
        }
    }
}
