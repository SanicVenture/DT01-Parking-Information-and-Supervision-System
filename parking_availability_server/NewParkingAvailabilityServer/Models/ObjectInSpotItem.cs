namespace NewParkingAvailabilityServer.Models
{
    public class ObjectInSpotItem //microcontroller data on whether there is an object in the parking spot or not, and whether there is an error with the sensor
    {
        public long Id { get; set; }
        public bool objectInSpot { get; set; }
        public int error { get; set; } //0 means no error, 1 means there is an error with the sensor, 2 means there is no connection to the network

        public ObjectInSpotItem(long id,  bool objectInSpot, int error)
        {
            this.Id = id;
            this.objectInSpot = objectInSpot;
            this.error = error;
        }
    }
}
