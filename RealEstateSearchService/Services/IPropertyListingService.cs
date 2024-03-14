using RealEstateSearchService.Model;

namespace RealEstateSearchService.Services
{
    /**
        * Interface for the PropertyListingService
        */
    public interface IPropertyListingService
    {
        Task AddPropertyListing(PropertyListing listing);
        Task UpdatePropertyListing(int id, PropertyListing listing);
        Task DeletePropertyListing(int id);
        Task<PropertyListing> GetPropertyListing(int id);
        Task<IEnumerable<PropertyListing>> SearchListings(string location, decimal? minPrice, decimal? maxPrice, string propertyType);
    }
}