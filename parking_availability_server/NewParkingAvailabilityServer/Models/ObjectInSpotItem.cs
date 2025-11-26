namespace NewParkingAvailabilityServer.Models
{
    public class ObjectInSpotItem
    {
        public long Id { get; set; }
        public bool objectInSpot { get; set; }

        public ObjectInSpotItem(long id,  bool objectInSpot)
        {
            this.Id = id;
            this.objectInSpot = objectInSpot;
        }
    }
}
