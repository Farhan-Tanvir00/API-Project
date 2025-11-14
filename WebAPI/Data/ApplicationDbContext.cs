using Microsoft.EntityFrameworkCore;
using WebAPI.DTO;

namespace WebAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Shirt> Shirts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Shirt>().HasData(
                new Shirt() { ID = 1, Color = "Red", Size = 10, Gender = "man", Price = 2000 },
                new Shirt() { ID = 2, Color = "Blue", Size = 8, Gender = "man", Price = 3000 },
                new Shirt() { ID = 3, Color = "Green", Size = 12, Gender = "man", Price = 4000 }
            );
        }    
    }
}
