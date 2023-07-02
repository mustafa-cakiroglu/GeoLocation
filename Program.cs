using GeoLocationTest.Configuration;
using GeoLocationTest.Configuration.Interface;
using GeoLocationTest.Data;
using GeoLocationTest.Services;
using GeoLocationTest.Services.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IElasticSearchService, ElasticSearchService>();
builder.Services.AddScoped<IElasticSearchConfigration, ElasticSearchConfigration>();


builder.Services.AddDbContext<IUnitOfWork, LocationContext>(b =>
                 b.UseSqlServer(builder.Configuration["ConnectionString"]));


//builder.Services.add(typeof(ApplicationMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
