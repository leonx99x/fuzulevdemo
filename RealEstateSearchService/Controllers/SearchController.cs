using Microsoft.AspNetCore.Mvc;
using Nest;
using RealEstateSearchService.Model;

[ApiController]
[Route("[controller]")]
public class SearchController : ControllerBase
{
    private readonly IElasticClient _elasticClient;

    public SearchController(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    [HttpGet]
    public async Task<IActionResult> Search(string location, string priceRange, string propertyType)
    {
        // Construct the query based on input
        // For simplicity, we are using a match query, consider using a more complex query for production
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
            // Handle error
            return BadRequest();
        }

        return Ok(searchResponse.Documents);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PropertyListing listing)
    {
        // Add to PostgreSQL
        // Sync with Elasticsearch

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PropertyListing listing)
    {
        // Update in PostgreSQL
        // Sync with Elasticsearch

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        // Delete from PostgreSQL
        // Remove from Elasticsearch

        return Ok();
    }

}
