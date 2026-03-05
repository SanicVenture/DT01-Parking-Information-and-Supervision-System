namespace NewParkingAvailabilityServer.Models
{
    public class OpenCVPolygonsItem
    {
        public long Id { get; set; }
        public string polygonPoints { get; set; } = string.Empty;
        public OpenCVPolygonsItem(long id, string polygonPoints)
        {
            this.Id = id;
            this.polygonPoints = polygonPoints;
        }
    }
}
