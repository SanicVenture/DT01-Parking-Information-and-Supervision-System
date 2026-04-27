namespace NewParkingAvailabilityServer.Models
{
    public class OpenCVResultsItem
    {
        // All data from YOLO object detection.
        public int Id { get; set; }
        public bool vehicle { get; set; }
        public bool parkingSpaceObstructed { get; set; }

        public OpenCVResultsItem(int id, bool vehicle, bool parkingSpaceObstructed)
        {
            Id = id;
            this.vehicle = vehicle;
            this.parkingSpaceObstructed = parkingSpaceObstructed;
        }
    }
}
