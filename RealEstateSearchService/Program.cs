using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // For Swagger configuration
using Nest; // For Elasticsearch
using RealEstateSearchService.Data; // Ensure this namespace points to where ApplicationDbContext is located
using RealEstateSearchService.Services; // Assuming your service implementations are here

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
