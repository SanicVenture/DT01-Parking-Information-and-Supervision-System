namespace NewParkingAvailabilityServer.Models
{
    public class ParkingSpaceItem //the final result of the parking space availability, which is sent to the customer-facing app and the display; this is determined from both the camera vision and the microcontroller data
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
