using Microsoft.EntityFrameworkCore;
using OrderFilter.DAL.Entities;

namespace OrderFilter.DAL
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CityDistrict> CityDistricts { get; set; }
    }
}
