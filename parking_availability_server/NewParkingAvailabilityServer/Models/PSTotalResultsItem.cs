namespace NewParkingAvailabilityServer.Models
{
    public class PSTotalResultsItem
    {
        public int Id { get; set; }
        public bool vehicle { get; set; }
        public bool objectInSpot { get; set; }
        public bool parkingSpaceObstructed { get; set; }
    }
}
