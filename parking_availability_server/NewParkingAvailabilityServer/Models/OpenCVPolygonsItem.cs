namespace NewParkingAvailabilityServer.Models
{
    public class OpenCVPolygonsItem //boundaries of a parking space in the form of a JSON string of the polygon points
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
