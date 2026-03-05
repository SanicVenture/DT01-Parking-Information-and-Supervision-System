namespace NewParkingAvailabilityServer.Models
{
    public class ObjectInSpotItem
    {
        public long Id { get; set; }
        public bool objectInSpot { get; set; }
        public int error { get; set; }

        public ObjectInSpotItem(long id,  bool objectInSpot, int error)
        {
            this.Id = id;
            this.objectInSpot = objectInSpot;
            this.error = error;
        }
    }
}
