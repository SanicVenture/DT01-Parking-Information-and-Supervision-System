namespace NewParkingAvailabilityServer.Models
{
    public class ParkingSpaceItem
    {
        public long Id { get; set; }
        public int floor { get; set; }
        public bool occupied { get; set; }
        public bool maintenanceAlert { get; set; }

        public ParkingSpaceItem(long id, int floor, bool occupied, bool maintenanceAlert)
        {
            Id = id;
            this.floor = floor;
            this.occupied = occupied;
            this.maintenanceAlert = maintenanceAlert;
        }
    }
}
