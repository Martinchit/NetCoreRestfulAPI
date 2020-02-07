using System;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Api.Data
{
    public class ApiDbContext: DbContext
    {
        public IConfiguration Configuration { get; }


        public ApiDbContext(DbContextOptions<ApiDbContext> options, IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("PSQLConnection"), o => o.SetPostgresVersion(9, 6));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Type = "Bikes" },
                new Category { Id = 2, Type = "Cars" },
                new Category { Id = 3, Type = "Trucks" }
            );
        }

    }
}
