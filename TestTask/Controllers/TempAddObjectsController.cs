using Microsoft.AspNetCore.Mvc;
using OrderFilter.DAL;
using OrderFilter.DAL.Entities;

namespace OrderFilter.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class TempAddObjectsController : Controller
    {
        private readonly MyDbContext _context;

        public TempAddObjectsController(MyDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Route("add-districts")]
        public ActionResult AddDistricts()
        {
            List<CityDistrict> dists = new List<CityDistrict>();
            CityDistrict dist = new CityDistrict { Name = "Академический" }; dists.Add(dist);
            dist = new CityDistrict { Name = "Гагаринский" }; dists.Add(dist);
            dist = new CityDistrict { Name = "Зюзино" }; dists.Add(dist);
            dist = new CityDistrict { Name = "Коньково" }; dists.Add(dist);
            dist = new CityDistrict { Name = "Котловка" }; dists.Add(dist);
            dist = new CityDistrict { Name = "Ломоносовский" }; dists.Add(dist);
            dist = new CityDistrict { Name = "Обручевский" }; dists.Add(dist);
            dist = new CityDistrict { Name = "Северное Бутово" }; dists.Add(dist);
            dist = new CityDistrict { Name = "Тёплый Стан" }; dists.Add(dist);
            dist = new CityDistrict { Name = "Черёмушки" }; dists.Add(dist);
            dist = new CityDistrict { Name = "Южное Бутово" }; dists.Add(dist);
            dist = new CityDistrict { Name = "Ясенево" }; dists.Add(dist);
            _context.AddRange(dists);
            _context.SaveChanges();
            return Ok();
        }
        [HttpPost]
        [Route("add-order")]
        public ActionResult AddOrder()
        {
            List<Order> orders = new List<Order>();

            for (int i = 0; i < 100; i++)
            {
                Random random = new Random();
                int skipperDist = random.Next(0, _context.CityDistricts.Count());
                var rDist = _context.CityDistricts.Skip(skipperDist).Take(1).FirstOrDefault();


                double skipperWeight = random.Next(0, 4) + random.NextDouble();

                skipperWeight = Math.Round(skipperWeight, 2);
                DateTime startDate = new DateTime(2024, 10, 26);
                DateTime finishDate = DateTime.Now;
                var skipperDT = random.NextInt64(startDate.Ticks, finishDate.Ticks);
                DateTime tempDeliveryDate = new DateTime(skipperDT);

                var orderDeliveryDate =
                    new DateTime(tempDeliveryDate.Year, tempDeliveryDate.Month,
                    tempDeliveryDate.Day, tempDeliveryDate.Hour,
                    tempDeliveryDate.Minute, tempDeliveryDate.Second);

                Order order = new Order { CityDistrict = rDist, Weight = skipperWeight, DeliveryDateTime = orderDeliveryDate };
                orders.Add(order);
            }

            _context.AddRange(orders);
            _context.SaveChanges();
            return Ok();

        }
    }

}
