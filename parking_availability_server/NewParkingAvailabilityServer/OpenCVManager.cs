using NewParkingAvailabilityServer.Models;
using OpenCvSharp;

namespace NewParkingAvailabilityServer
{
    public class OpenCVManager
    {
        private SQLManager sqlManager = new SQLManager();
        private string streamURL = "rtsp://admin:Password@10.18.31.38:554/stream";
        private int msTimeout = 120000;
        private int[] spotIds = [1, 2]; //example parking spot IDs

        //This string array of acceptable vehicles is just an example of what the OpenCV implementation will look for.
        private string[] acceptableVehicles = ["car", "motorcycle", "van", "truck"];

        public async void StartImageRecognition()
        {
            Task.Run(ImageRecognition);
        }

        private async void ImageRecognition()
        {
            using (var capture = new VideoCapture(streamURL))
            {
                if (!capture.IsOpened())
                {
                    Console.WriteLine("ERROR: could not open camera stream.");
                    return;
                }
                using (var frame = new Mat())
                {
                    while (true)
                    {
                        capture.Read(frame);

                        if (frame.Empty())
                        {
                            Thread.Sleep(msTimeout);
                            continue;
                        }

                        //pseudocode starts here

                        OpenCVResultsItem[] results = new OpenCVResultsItem[2];


                        //do analysis of the frame for what objects are in the frame
                        //GetFrameObjects still needs to be created
                        string[][] listOfObjectsInPhoto = GetFrameObjects(frame);

                        //do analysis of the frame for whether the parking spot is obstructed or not
                        //GetParkingSpotFrame still needs to be created
                        bool[] parkingSpaceStates = GetParkingSpotState(frame);


                        int index = 0;

                        //the foreach loop checks for
                        foreach (string[] parkingSpaceObjects in listOfObjectsInPhoto)
                        {
                            bool validObject = false;
                            foreach (string objectInSpot in parkingSpaceObjects)
                            {
                                if (acceptableVehicles.Contains(objectInSpot))
                                {
                                    validObject = true;
                                    break;
                                }
                            }
                            results[index] = new OpenCVResultsItem(spotIds[index], validObject, parkingSpaceStates[index]);

                            await sqlManager.createnewOpenCVResultsEntry(results[index]);
                            sqlManager.CheckForMicrocontrollerData(spotIds[index]);
                            index++;
                        }

                        Thread.Sleep(msTimeout);

                        //end pseudocode
                    }
                }
            }
        }
    }
}
