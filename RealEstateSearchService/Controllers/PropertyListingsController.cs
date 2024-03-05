using Microsoft.AspNetCore.Mvc;
using RealEstateSearchService.Data;
using RealEstateSearchService.Model;
using RealEstateSearchService.Services;

[Route("api/[controller]")]
[ApiController]
public class PropertyListingsController : ControllerBase
{
    private readonly IPropertyListingService _propertyListingService;

    public PropertyListingsController(IPropertyListingService propertyListingService)
    {
        _propertyListingService = propertyListingService;
    }

    [HttpPost]
    public async Task<IActionResult> AddPropertyListing([FromBody] PropertyListing listing)
    {
        await _propertyListingService.AddPropertyListing(listing);
        return CreatedAtAction(nameof(GetPropertyListing), new { id = listing.Id }, listing);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPropertyListing(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePropertyListing(int id, [FromBody] PropertyListing listing)
    {
        await _propertyListingService.UpdatePropertyListing(id, listing);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePropertyListing(int id)
    {
        await _propertyListingService.DeletePropertyListing(id);
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchListings([FromQuery] string location, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] string propertyType)
    {
        var listings = await _propertyListingService.SearchListings(location, minPrice, maxPrice, propertyType);
        return Ok(listings);
    }
}
