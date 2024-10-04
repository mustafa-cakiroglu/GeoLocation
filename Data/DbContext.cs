using GeoLocationTest.Models;
using GeoLocationTest.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace GeoLocationTest.Data
{
    public class LocationContext : Extra360DbContext, IUnitOfWork
    {
        public static readonly string DefaultSchema = "dbo";

        public DbSet<Cm_CustomerLocations> CustomerLocations { get; set; }
        public DbSet<Cm_Customer> Customers { get; set; }

        public LocationContext(DbContextOptions<LocationContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Cm_CustomerLocations>().ToTable("Cm_CustomerLocations");
            modelBuilder.Entity<Cm_Customer>().ToTable("Cm_Customer");
            modelBuilder.HasDefaultSchema(DefaultSchema);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=localhost;Initial Catalog=Customer;User ID=sa;Password=Asdasdxx1;Encrypt=False");
        }
    }
}
