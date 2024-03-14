using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Nest;
using RealEstateSearchService.Data;
using RealEstateSearchService.Data.Seeding;
using RealEstateSearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Elasticsearch client
var settings = new ConnectionSettings(new Uri(builder.Configuration["Elasticsearch:Uri"]))
    .DefaultIndex(builder.Configuration["Elasticsearch:DefaultIndex"]);
var client = new ElasticClient(settings);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

// Register the IElasticClient singleton
builder.Services.AddSingleton<IElasticClient>(client);

// Assuming you have an IPropertyListingService interface and a PropertyListingService implementation
builder.Services.AddScoped<IPropertyListingService, PropertyListingService>();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Real Estate Search API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Real Estate Search API V1"));
}
SeedTestData(app.Services.CreateScope().ServiceProvider);
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void SeedTestData(IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {
        var scopedProvider = scope.ServiceProvider;
        var context = scopedProvider.GetRequiredService<ApplicationDbContext>();

        bool migrated = false;
        int retryCount = 0;
        while (!migrated && retryCount < 5)
        {
            try
            {
                context.Database.Migrate();
                migrated = true;
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "57P03")
            {
                Console.WriteLine("Database is starting up, waiting...");
                Thread.Sleep(5000); // wait for 5 seconds
                retryCount++;
            }
        }

        if (migrated)
        {
            var elasticClient = scopedProvider.GetRequiredService<IElasticClient>();
            var testData = TestDataInitializer.GetTestPropertyListings();

            if (!context.PropertyListings.Any())
            {
                context.AddRange(testData);
                context.SaveChanges();

                foreach (var listing in testData)
                {
                    var indexResponse = elasticClient.IndexDocument(listing);
                    if (!indexResponse.IsValid)
                    {
                        Console.WriteLine($"Failed to index document {listing.Id}");
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("Failed to migrate the database within the retry limit.");
        }


    }
}
