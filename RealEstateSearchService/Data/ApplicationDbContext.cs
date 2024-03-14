using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using RealEstateSearchService.Model;

namespace RealEstateSearchService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<PropertyListing> PropertyListings { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);




        }
    }
}
