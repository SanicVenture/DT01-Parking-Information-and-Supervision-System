using Microsoft.EntityFrameworkCore;
namespace NewParkingAvailabilityServer.Models;

public class PSTotalResultsContext : DbContext
{
    public PSTotalResultsContext(DbContextOptions<PSTotalResultsContext> options) : base(options)
    {
    }

    public DbSet<PSTotalResultsItem> PSTotalResultsItems { get; set; } = null!;
}
