using Microsoft.EntityFrameworkCore;

namespace NewParkingAvailabilityServer.Models;
public class OpenCVPolygonsContext : DbContext
{
    public OpenCVPolygonsContext(DbContextOptions<OpenCVPolygonsContext> options) : base(options)
    {
    }

    public DbSet<OpenCVPolygonsItem> OpenCVPolygonsItems { get; set; } = null!;
}