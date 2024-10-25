using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            Random random = new Random();
            int skipperDist = random.Next(0, _context.CityDistricts.Count());
            var rDist = _context.CityDistricts.Where(cd => cd.Id == skipperDist).FirstOrDefault();


            double skipperWeight = random.Next(0, 4) + random.NextDouble();

            DateTime startDate = new DateTime(2023, 11, 18);
            DateTime finishDate = DateTime.Now;
            var skipperDT = random.NextInt64(startDate.Ticks, finishDate.Ticks);
            DateTime orderDeliveryDate = new DateTime(skipperDT);

            Order order = new Order { CityDistrict = rDist, Weight = skipperWeight, DeliveryDateTime = orderDeliveryDate };
            _context.Add(order);
            _context.SaveChanges();
            return Ok();

        }
    }

}
