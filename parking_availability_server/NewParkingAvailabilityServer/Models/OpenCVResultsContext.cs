using Microsoft.EntityFrameworkCore;

namespace NewParkingAvailabilityServer.Models;
public class OpenCVResultsContext : DbContext
{
    public OpenCVResultsContext(DbContextOptions<OpenCVResultsContext> options) : base(options)
    {
    }

    public DbSet<OpenCVResultsItem> OpenCVResultsItems { get; set; } = null!;
}