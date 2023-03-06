using GrpcNet7.Entities;
using Microsoft.EntityFrameworkCore;

namespace GrpcNet7.Data
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            var connSring = Configuration.GetConnectionString("connDef");
            optionsBuilder.UseMySql(connSring, ServerVersion.AutoDetect(connSring));
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
    }
}
