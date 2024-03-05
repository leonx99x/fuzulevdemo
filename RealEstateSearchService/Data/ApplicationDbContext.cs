using Microsoft.EntityFrameworkCore;
using RealEstateSearchService.Model;

namespace RealEstateSearchService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<PropertyListing> PropertyListings { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
