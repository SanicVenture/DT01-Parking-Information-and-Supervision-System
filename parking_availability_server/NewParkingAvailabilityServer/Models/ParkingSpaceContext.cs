using Microsoft.EntityFrameworkCore;
namespace NewParkingAvailabilityServer.Models;

public class ParkingSpaceContext : DbContext
{
    public ParkingSpaceContext(DbContextOptions<ParkingSpaceContext> options) : base(options)
    {
    }

    public DbSet<ParkingSpaceItem> ParkingSpaceItems { get; set; } = null!;
}
