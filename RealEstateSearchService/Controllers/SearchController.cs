using Microsoft.AspNetCore.Mvc;
using Nest;
using RealEstateSearchService.Data;
using RealEstateSearchService.Model;

[ApiController]
[Route("[controller]")]
public class SearchController : ControllerBase
{
    private readonly IElasticClient _elasticClient;
    private readonly ApplicationDbContext _context;

    public SearchController(IElasticClient elasticClient, ApplicationDbContext context)
    {
        _elasticClient = elasticClient;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Search(string location, string priceRange, string propertyType)
    {
        var searchResponse = await _elasticClient.SearchAsync<PropertyListing>(s => s
            .Query(q => q
                .Bool(b => b
                    .Must(mu => mu.Match(m => m.Field(f => f.Location).Query(location)),
                         mu => mu.Match(m => m.Field(f => f.PropertyType).Query(propertyType))
                    )
                )
            )
        );

        if (!searchResponse.IsValid)
        {
            return BadRequest();
        }

        return Ok(searchResponse.Documents);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PropertyListing listing)
    {
        _context.PropertyListings.Add(listing);
        await _context.SaveChangesAsync();

        var indexResponse = await _elasticClient.IndexDocumentAsync(listing);

        if (!indexResponse.IsValid)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(Search), new { id = listing.Id }, listing);

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PropertyListing updatedListing)
    {
        var existingListing = await _context.PropertyListings.FindAsync(id);
        if (existingListing == null)
        {
            return NotFound();
        }


        existingListing.Location = updatedListing.Location;
        existingListing.Price = updatedListing.Price;
        existingListing.PropertyType = updatedListing.PropertyType;


        await _context.SaveChangesAsync();

        var indexResponse = await _elasticClient.IndexDocumentAsync(existingListing); // IndexDocumentAsync updates if ID exists

        if (!indexResponse.IsValid)
        {

            return BadRequest();
        }

        return Ok(existingListing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var listing = await _context.PropertyListings.FindAsync(id);
        if (listing == null)
        {
            return NotFound();
        }

        _context.PropertyListings.Remove(listing);
        await _context.SaveChangesAsync();

        var deleteResponse = await _elasticClient.DeleteAsync<PropertyListing>(id);

        if (!deleteResponse.IsValid)
        {
            return BadRequest();
        }

        return Ok();
    }

}
