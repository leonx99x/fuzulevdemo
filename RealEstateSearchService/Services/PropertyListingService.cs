

using Nest;
using RealEstateSearchService.Data;
using RealEstateSearchService.Model;

namespace RealEstateSearchService.Services
{
    /**
        * This service class is responsible for handling the CRUD operations for property listings, as well as searching for listings based on certain criteria.
        * It uses Entity Framework Core for database operations and Elasticsearch for search operations.
        */
    public class PropertyListingService : IPropertyListingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IElasticClient _elasticClient;

        public PropertyListingService(ApplicationDbContext context, IElasticClient elasticClient)
        {
            _context = context;
            _elasticClient = elasticClient;
        }

        public async Task AddPropertyListing(PropertyListing listing)
        {
            _context.PropertyListings.Add(listing);
            await _context.SaveChangesAsync();

            // Assuming you have an Elasticsearch index set up for PropertyListing
            await _elasticClient.IndexDocumentAsync(listing);
        }

        public async Task UpdatePropertyListing(int id, PropertyListing listing)
        {
            // Update logic here, followed by Elasticsearch sync
        }

        public async Task DeletePropertyListing(int id)
        {
            // Delete logic here, followed by Elasticsearch removal
        }

        public Task<IEnumerable<PropertyListing>> SearchListings(string location, decimal? minPrice, decimal? maxPrice, string propertyType)
        {
            throw new NotImplementedException();
        }

        // public async Task<IEnumerable<PropertyListing>> SearchListings(string location, decimal? minPrice, decimal? maxPrice, string propertyType)
        // {
        //     //throw nonimplemented
        //     await
        // }
    }
}