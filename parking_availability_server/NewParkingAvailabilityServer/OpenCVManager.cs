using NewParkingAvailabilityServer.Models;
using OpenCvSharp;
using System.Drawing;
using System.Security.Cryptography.Xml;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using Humanizer;
using System.Windows.Forms;

namespace NewParkingAvailabilityServer
{
    public class OpenCVManager
    {
        private SQLManager sqlManager = new SQLManager();
        private string streamURL = "rtsp://admin:Password@10.18.31.38:554";
        private int msTimeout = 120000;
        private int[] spotIds = [1, 2]; //example parking spot IDs

        //This string array of acceptable vehicles is just an example of what the
        //OpenCV implementation will look for.
        private string[] acceptableVehicles = ["car", "motorcycle", "van", "truck"];

        private Mat DrawLines(Mat image, LineSegmentPoint[] lines, Scalar color, int thickness = 3)
        {
            //image = new Mat(image);
            Mat line_image = new Mat(image.Size(), image.Type());
            foreach (LineSegmentPoint line in lines)
            {
                Cv2.Line(line_image, line.P1.X, line.P1.Y, line.P2.X, line.P2.Y, color, thickness);
            }
            Cv2.AddWeighted(image, 0.8, line_image, 1.0, 0.0, image);

            return image;
        }

        public async void StartImageRecognition()
        {
            Task.Run(ImageRecognition);
        }

        //private void MouseCallback(MouseEventTypes @event, int x, int y, MouseEventFlags flags, IntPtr userData)
        //{
                
        //}

        private async void ImageRecognition()
        {
            using (var capture = new VideoCapture(streamURL))
            {
                if (!capture.IsOpened())
                {
                    Console.WriteLine("ERROR: could not open camera stream.");
                    return;
                }
                //using (var frame = new Mat())
                using (Mat frame = Cv2.ImRead("input_frame.jpg"))
                {
                    while (true)
                    {
                        using (var dest = frame.Clone())
                        {
                            //Cv2.CvtColor(frame, dest, ColorConversionCodes.BGR2GRAY);

                            //Cv2.ImShow("Grayscale Image", dest);
                            //Cv2.WaitKey(0);

                            //Console.WriteLine("Enter Number of Parking Spots:");

                            //string input = Console.ReadLine();

                            //int parkingSpotAmount = int.Parse(input);

                            int parkingSpotAmount = 2;

                            //START OF LOOP


                            List<OpenCvSharp.Point> points = new List<OpenCvSharp.Point>();

                            Cv2.NamedWindow("Select ROI");
                            Cv2.SetMouseCallback("Select ROI", (MouseEventTypes @event, int x, int y, MouseEventFlags flags, IntPtr userData) =>
                            {
                                if (@event == MouseEventTypes.LButtonDown)
                                {
                                    points.Add(new OpenCvSharp.Point(x, y));
                                    Cv2.Circle(dest, new OpenCvSharp.Point(x, y), 3, Scalar.Red, -1);
                                    Cv2.ImShow("Select ROI", dest);
                                }
                            });

                            Cv2.ImShow("Select ROI", dest);
                            Cv2.WaitKey(0);

                            Mat mask = new Mat(dest.Size(), MatType.CV_8UC1, Scalar.All(0));

                            OpenCvSharp.Point[][] polygon;

                            if (points.Count > 2)
                            {
                               polygon = [points.ToArray()];

                                Cv2.FillPoly(mask, polygon, Scalar.All(255)); //fill polygon with white
                            }

                            Mat dst = new Mat();
                            Cv2.BitwiseAnd(dest, dest, dst, mask);

                            Cv2.ImShow("ROI", dst);
                            Cv2.WaitKey(0);

                            //END OF LOOP



                            #region collapsed

                            //Cv2.GaussianBlur(dest, dest, new OpenCvSharp.Size(5, 5), 0);

                            //Cv2.ImShow("Gaussian Blur", dest);
                            //Cv2.WaitKey(0);

                            //Cv2.Dilate(dest, dest, Mat.Ones(3, 3, MatType.CV_8UC1));

                            //Cv2.ImShow("Dilation", dest);
                            //Cv2.WaitKey(0);

                            //Mat kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(2, 2));
                            //Cv2.MorphologyEx(dest, dest, MorphTypes.Close, kernel);

                            //Cv2.ImShow("Morphologically Closed Image", dest);
                            //Cv2.WaitKey(0);


                            //using (var cannyed_image = new Mat())
                            //{
                            //    Cv2.Canny(dest, cannyed_image, 100, 200);

                            //    Cv2.ImShow("Cannyed Image", cannyed_image);
                            //    Cv2.WaitKey(0);

                            //    string roiselect = "Select ROI";



                            //    Rect roi = Cv2.SelectROI(roiselect, cannyed_image);

                            //    Cv2.DestroyWindow(roiselect);

                            //    using (var cropped_image = new Mat(cannyed_image, roi))
                            //    {
                            //        Cv2.ImShow("Cropped Image", cropped_image);
                            //        Cv2.WaitKey(0);

                            //        LineSegmentPoint[] lines = Cv2.HoughLinesP(cropped_image, 6, Math.PI / 60, 80, 40, 25);

                            //        //using (var cropped_original = (new Mat(frame, roi)).Clone())
                            //        //{ 


                            //        //}




                            //        ////image = new Mat(image);
                            //        //Mat line_image = new Mat(cropped_original.Size(), cropped_original.Type());
                            //        //foreach (LineSegmentPoint line in lines)
                            //        //{
                            //        //    Cv2.Line(line_image, line.P1.X, line.P1.Y, line.P2.X, line.P2.Y, Scalar.Red, 3);
                            //        //}
                            //        //Cv2.AddWeighted(image, 0.8, line_image, 1.0, 0.0, image);

                            //        //return image;

                            //        using (var lined_image = DrawLines((new Mat(frame, roi)).Clone(), lines, Scalar.Red))
                            //        {
                            //            Cv2.ImShow("Lined Image", lined_image);
                            //            Cv2.WaitKey(0);
                            //        }

                            //        lines = lines.OrderBy(x => x.P1.X).ToArray();

                            //        //foreach (var line in lines)
                            //        //{
                            //        //    using (var line_image = DrawLines((new Mat(frame, roi)).Clone(), [line], Scalar.Red))
                            //        //    {
                            //        //        Cv2.ImShow("Lined Image", line_image);
                            //        //        Cv2.WaitKey(0);
                            //        //    }
                            //        //}



                            //    }



                            //}

                            #endregion

                        }
                        //KEEP BELOW. Actual Code
                        //capture.Read(frame);

                        //if (frame.Empty())
                        //{
                        //    Thread.Sleep(msTimeout);
                        //    continue;
                        //}
                        //else
                        //{
                        //    //image saving test
                        //    string outputPath = "output_frame.bmp";

                        //    // 3. Save the Mat object to a BMP file using Cv2.ImWrite
                        //    Cv2.ImWrite(outputPath, frame);

                        //    Console.WriteLine($"Frame successfully saved to {outputPath}");
                        //}

                        //END OF REAL CODE TO KEEP

                        //pseudocode starts here

                        //OpenCVResultsItem[] results = new OpenCVResultsItem[2];


                        ////do analysis of the frame for what objects are in the frame
                        ////GetFrameObjects still needs to be created
                        //string[][] listOfObjectsInPhoto = GetFrameObjects(frame);

                        ////do analysis of the frame for whether the parking spot is
                        ////obstructed or not
                        ////GetParkingSpotFrame still needs to be created
                        //bool[] parkingSpaceStates = GetParkingSpotState(frame);


                        //int index = 0;

                        ////the foreach loop checks for whether the parking space objects
                        ////are acceptable to be in the parking space.
                        //foreach (string[] parkingSpaceObjects in listOfObjectsInPhoto)
                        //{
                        //    bool validObject = false;
                        //    foreach (string objectInSpot in parkingSpaceObjects)
                        //    {
                        //        if (acceptableVehicles.Contains(objectInSpot))
                        //        {
                        //            validObject = true;
                        //            break;
                        //        }
                        //    }
                        //    results[index] = new OpenCVResultsItem(
                        //        spotIds[index],
                        //        validObject,
                        //        parkingSpaceStates[index]);

                        //    await sqlManager.createnewOpenCVResultsEntry(results[index]);
                        //    sqlManager.CheckForMicrocontrollerData(spotIds[index]);
                        //    index++;
                        //}

                        //Thread.Sleep(msTimeout);

                        //end pseudocode
                    }
                }
            }
        }
    }
}
