using Microsoft.EntityFrameworkCore;

namespace NewParkingAvailabilityServer.Models;
public class ObjectInSpotContext : DbContext
{
    public ObjectInSpotContext(DbContextOptions<ObjectInSpotContext> options) : base(options)
    {
    }

    public DbSet<ObjectInSpotItem> ObjectInSpotItems { get; set; } = null!;
}