using Microsoft.EntityFrameworkCore;
using NewParkingAvailabilityServer;
using NewParkingAvailabilityServer.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ParkingSpaceContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnection")));
builder.Services.AddDbContext<PSTotalResultsContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("SecondConnection")));
builder.Services.AddDbContext<ObjectInSpotContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("ObjectInSpotConnection")));
builder.Services.AddDbContext<OpenCVResultsContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("OpenCVResultsConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

OpenCVManager openCVManager = new OpenCVManager();
openCVManager.StartImageRecognition();

Console.WriteLine("pizza");

app.Run();

