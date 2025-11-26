namespace NewParkingAvailabilityServer.Models
{
    public class PSTotalResultsItem
    {
        public int Id { get; set; }
        public bool vehicle { get; set; }
        public bool objectInSpot { get; set; }
        public bool parkingSpaceObstructed { get; set; }

        public ParkingSpaceItem convertedSpot { get; set; }

        public PSTotalResultsItem(OpenCVResultsItem openCVResultsItem, ObjectInSpotItem objectInSpotItem)
        {
            if (openCVResultsItem.Id == objectInSpotItem.Id)
            {
                Id = openCVResultsItem.Id;
                vehicle = openCVResultsItem.vehicle;
                objectInSpot = objectInSpotItem.objectInSpot;
                parkingSpaceObstructed = openCVResultsItem.parkingSpaceObstructed;
                convertPSTotalResultsItem();
            }
        }

        public PSTotalResultsItem(int id, bool vehicle, bool objectInSpot, bool parkingSpaceObstructed)
        {
            Id = id;
            this.vehicle = vehicle;
            this.objectInSpot = objectInSpot;
            this.parkingSpaceObstructed = parkingSpaceObstructed;
            convertPSTotalResultsItem();
        }

        public void convertPSTotalResultsItem()
        {
            bool maintenanceState = false;

            if (objectInSpot)
            {
                maintenanceState = !parkingSpaceObstructed || !vehicle;
            }
            if (!objectInSpot)
            {
                maintenanceState = parkingSpaceObstructed;
            }

            convertedSpot = new ParkingSpaceItem(
                Id,
                -1,
                objectInSpot,
                maintenanceState
            );
        }


    }
}
