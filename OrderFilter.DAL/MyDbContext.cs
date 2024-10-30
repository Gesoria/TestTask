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
        public DbSet<DeliveryLog> DeliveryLogs { get; set; }
        public DbSet<DeliveryOrder> DeliveryOrders { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {



            modelBuilder.Entity<Order>()
           .HasMany<DeliveryOrder>()
           .WithMany(c => c.Orders)
           .UsingEntity(j => j.ToTable("DeliveryOrdersWithOrders"));
        }


    }
}
