using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFilter.ApiModels;
using OrderFilter.DAL;
using OrderFilter.DAL.Entities;
using OrderFilter.Extensions;

namespace OrderFilter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CityDistrictController : Controller
    {
        private readonly MyDbContext _context;
        private readonly ILogger<OrderController> _logger;

        public CityDistrictController(MyDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [Route("add-district")]
        public async Task<IActionResult> AddCityDistrictAsync([FromBody] CityDistrictApiModel districtModel, CancellationToken cancellationToken)
        {
            try
            {
                //Логирую 
                string logInfo = $"Производится поиск района по имени {districtModel.Name} в базе данных на совпадения";
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

                //Выполняю поиск района в базе данных по имени
                var districts = await _context.CityDistricts.ToListAsync();
                var district = districts.FirstOrDefault(dist =>
                    string.Equals(dist.Name, districtModel.Name, StringComparison.OrdinalIgnoreCase));

                //Если район не был найден
                if (district == null)
                {
                    //Добавляю район в базу данных 
                    CityDistrict cityDistrict = new CityDistrict
                    {
                        Name = districtModel.Name.CapitalizeFirstLetter(),
                    };
                    await _context.CityDistricts.AddAsync(cityDistrict);
                    await _context.SaveChangesAsync(cancellationToken);

                    //Логирую 
                    logInfo = $"Район с именем {cityDistrict.Name} c идентификатором {cityDistrict.Id} сохранен в базу данных";
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

                    return Ok($"Район с именем {cityDistrict.Name} сохранен в базу данных");
                }

                //Если район с таким именем уже существует в базе данных
                else
                {
                    //Логирую 
                    logInfo = $"Не удалось сохранить район под именем {districtModel.Name}, такое имя уже есть в базе данных";
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
                    return BadRequest(logInfo);
                }
            }
            catch (Exception ex)
            {
                //Логирую
                string errorMessage = $"Произошла ошибка при добавлении района {ex.Message}";
                _logger.LogInformation(errorMessage);

                //Сохраняю лог в базу данных
                DeliveryLog logError = new DeliveryLog
                {
                    Type = (DAL.Enums.LogType)3,
                    Message = errorMessage,
                    TimeStamp = DateTime.Now,
                    Source = HttpContext.Request.Path
                };
                await _context.DeliveryLogs.AddAsync(logError);
                await _context.SaveChangesAsync();

                return StatusCode(500, errorMessage);
            }
        }
    }
}
