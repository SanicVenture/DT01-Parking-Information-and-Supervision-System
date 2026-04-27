using System.ComponentModel.DataAnnotations.Schema;

namespace NewParkingAvailabilityServer.Models
{
    public class PSTotalResultsItem
    {
        public int Id { get; set; } = -1;
        public bool vehicle { get; set; }
        public bool objectInSpot { get; set; } //determined from microcontroller
        public bool parkingSpaceObstructed { get; set; } //for a visually obstructed parking space; this is only determined from camera vision
        public bool sensorConnectedToNetwork { get; set; } = true;


        [NotMapped]
        public ParkingSpaceItem? convertedSpot { get; set; } = null;

        public PSTotalResultsItem(OpenCVResultsItem openCVResultsItem, ObjectInSpotItem objectInSpotItem)
        {
            if (openCVResultsItem != null && objectInSpotItem != null && openCVResultsItem.Id == objectInSpotItem.Id)
            {
                Id = openCVResultsItem.Id;
                vehicle = openCVResultsItem.vehicle;
                objectInSpot = objectInSpotItem.objectInSpot;
                parkingSpaceObstructed = openCVResultsItem.parkingSpaceObstructed;
                sensorConnectedToNetwork = objectInSpotItem.error == 2 ? false : true;
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
            bool maintenanceState = false; //whether there is something wrong with the spot or not

            if (objectInSpot)
            {
                maintenanceState = !parkingSpaceObstructed || !vehicle;
            }
            if (!objectInSpot)
            {
                maintenanceState = parkingSpaceObstructed;
            }

            //ParkingSpaceItem which is used for customer-facing app and the display
            convertedSpot = new ParkingSpaceItem(
                Id,
                -1,
                objectInSpot,
                maintenanceState
            );
        }
    }
}
