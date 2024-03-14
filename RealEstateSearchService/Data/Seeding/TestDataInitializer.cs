using RealEstateSearchService.Model;

namespace RealEstateSearchService.Data.Seeding
{
    public static class TestDataInitializer
    {
        public static List<PropertyListing> GetTestPropertyListings()
        {
            var testData = new List<PropertyListing>
            {
                 new PropertyListing
                {
                    Location = "Istanbul",
                    Price = 300000,
                    PropertyType = "Apartment",
                    Description = "A spacious apartment in the heart of the city."
                },
                new PropertyListing
                {
                    Location = "Ankara",
                    Price = 150000,
                    PropertyType = "House",
                    Description = "Cozy house with a beautiful garden."
                },
                new PropertyListing
                {
                    Location = "Izmir",
                    Price = 250000,
                    PropertyType = "Villa",
                    Description = "Luxurious villa with sea views."
                },
                new PropertyListing
                {
                    Location = "Antalya",
                    Price = 200000,
                    PropertyType = "Apartment",
                    Description = "Modern apartment close to the beach."
                },
                new PropertyListing
                {
                    Location = "Bursa",
                    Price = 100000,
                    PropertyType = "House",
                    Description = "Affordable house with easy access to the city center."
                }
            };
            return testData;
        }
    }
}