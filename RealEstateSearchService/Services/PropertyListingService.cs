

using Microsoft.EntityFrameworkCore;
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
        public async Task<PropertyListing> GetPropertyListing(int id)
        {
            var propertyListing = await _context.PropertyListings
                .FirstOrDefaultAsync(p => p.Id == id);

            if (propertyListing == null)
            {
                throw new KeyNotFoundException($"A property listing with the ID {id} was not found.");
            }

            return propertyListing;
        }

        public async Task UpdatePropertyListing(int id, PropertyListing updatedListing)
        {
            var existingListing = await _context.PropertyListings.FindAsync(id);
            if (existingListing == null)
            {
                throw new KeyNotFoundException($"PropertyListing with id {id} not found.");
            }

            existingListing.Location = updatedListing.Location;
            existingListing.Price = updatedListing.Price;
            existingListing.Phone = updatedListing.Phone;
            existingListing.Email = updatedListing.Email;
            existingListing.PropertyType = updatedListing.PropertyType;
            existingListing.Description = updatedListing.Description;
            existingListing.ImageUrl = updatedListing.ImageUrl;
            existingListing.IsActive = updatedListing.IsActive;
            existingListing.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Elasticsearch update
            var indexResponse = await _elasticClient.UpdateAsync<DocumentPath<PropertyListing>>(
                DocumentPath<PropertyListing>.Id(id),
                u => u.Doc(existingListing).RetryOnConflict(5)
            );

            if (!indexResponse.IsValid)
            {
                // Log error or handle Elasticsearch update failure
                throw new Exception("Failed to update Elasticsearch index.");
            }
        }


        public async Task DeletePropertyListing(int id)
        {
            var listing = await _context.PropertyListings.FindAsync(id);
            if (listing == null)
            {
                throw new KeyNotFoundException($"PropertyListing with id {id} not found.");
            }

            _context.PropertyListings.Remove(listing);
            await _context.SaveChangesAsync();

            var deleteResponse = await _elasticClient.DeleteAsync(new DeleteRequest("propertylistings", id.ToString()));

            if (!deleteResponse.IsValid)
            {
                throw new Exception("Failed to delete from Elasticsearch index.");
            }
        }

        public async Task<IEnumerable<PropertyListing>> SearchListings(string location, decimal? minPrice, decimal? maxPrice, string propertyType)
        {
            var searchResponse = await _elasticClient.SearchAsync<PropertyListing>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Must(mustQueries =>
                        {
                            var queries = new List<QueryContainer>();

                            if (!string.IsNullOrWhiteSpace(location))
                            {
                                queries.Add(mustQueries.Match(m => m
                                    .Field(f => f.Location)
                                    .Query(location)));
                            }

                            if (minPrice.HasValue)
                            {
                                queries.Add(mustQueries.Range(r => r
                                    .Field(f => f.Price)
                                    .GreaterThanOrEquals((double)minPrice.Value)));
                            }

                            if (maxPrice.HasValue)
                            {
                                queries.Add(mustQueries.Range(r => r
                                    .Field(f => f.Price)
                                    .LessThanOrEquals((double)maxPrice.Value)));
                            }

                            if (!string.IsNullOrWhiteSpace(propertyType))
                            {
                                queries.Add(mustQueries.Match(m => m
                                    .Field(f => f.PropertyType)
                                    .Query(propertyType)));
                            }

                            return new BoolQuery { Must = queries };
                        })
                    )
                )
            );

            if (!searchResponse.IsValid)
            {
                throw new Exception("Search query failed.");
            }

            return searchResponse.Documents;
        }

    }
}