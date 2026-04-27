using Microsoft.ML.OnnxRuntime;
using NewParkingAvailabilityServer.Models;
using OpenCvSharp;
using System.Drawing;
using System.Security.Cryptography.Xml;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using Humanizer;
using SkiaSharp;
using YoloDotNet;
using YoloDotNet.Models;
using YoloDotNet.Extensions;
using YoloDotNet.ExecutionProvider.Cpu;
using Compunet.YoloSharp;
using Compunet.YoloSharp.Plotting;
using SixLabors.ImageSharp;
using Compunet.YoloSharp.Data;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using Clipper2Lib;

namespace NewParkingAvailabilityServer
{
    public class OpenCVManager
    {
        private SQLManager sqlManager = new SQLManager();
        //private string streamURL = "rtsp://admin:Password@10.18.31.38:554";

        //url for Reolink camera
        private string streamURL = "rtsp://admin:Password@192.168.0.50:554";
        private int msTimeout = 2000;
        private int[] spotIds = [1]; //example parking spot ID. The Maintenance App will have a feature for
                                     //creating parking spots and assigning them to a camera.
        private bool showDetections = false;

        private string[] acceptableVehicles = ["car", "motorcycle", "van", "truck", "bus", "bicycle"];
        private string[] ignoredObjects = ["person", "dog", "cat", "backpack", "suitcase", "handbag",
            "tie", "bottle", "cup", "fork", "knife", "spoon", "bowl", "banana", "apple", "sandwich", 
            "orange", "cell phone", "book"];

        public async void StartImageRecognition()
        {
            using Mutex mut = new Mutex(false);

            //In practice, this might be intensive for a lower-spec server.
            foreach (int Id in spotIds)
            {
                Task.Run(() => ImageRecognition(Id)); //we are going to support multiple parking spots with tasks.
            }
        }

        //The point of this function is to see if the parked object is sufficiently within the parking space boundaries to count as parked.
        private bool[] RectangleChecker(Detection detection, OpenCvSharp.Point[][]? polygon)
        {
            int[] ints = new int[polygon.Length];
            int furthestLeftPoint = polygon[0][0].X;
            int furthestRightPoint = polygon[0][0].X;
            foreach (OpenCvSharp.Point point in polygon[0])
            {
                if (point.X < furthestLeftPoint)
                {
                    furthestLeftPoint = point.X;
                }

                if (point.X > furthestRightPoint)
                {
                    furthestRightPoint = point.X;
                }
            }

            int leftX = detection.Bounds.X;
            int topY = detection.Bounds.Y;
            int rightX = detection.Bounds.X + detection.Bounds.Width;
            int bottomY = detection.Bounds.Y + detection.Bounds.Height;

            //first we check if the detection rectangle could intersect or be within with the parking spot polygon.
            //If it not, then we can return false immediately.
            if (!((leftX < furthestLeftPoint) && (rightX < furthestLeftPoint)) && 
                !((rightX > furthestRightPoint) && (leftX > furthestRightPoint)))
            {
                //converting the YOLO detection rectangle to Clipper Points, which are made a part of the Clipper Path
                Path64 detectionPath = new Path64([
                    new Point64(leftX, topY), //top left
                    new Point64(rightX, topY), //top right
                    new Point64(rightX, bottomY), //bottom right
                    new Point64(leftX, bottomY) //bottom left
                ]);

                //the parking spot boundaries are converted to Clipper Points, which are made a part of the Clipper Path
                Path64 polygonPath = new Path64();
                foreach (OpenCvSharp.Point point in polygon[0])
                {
                    polygonPath.Add(new Point64(point.X, point.Y));
                }

                //The Clipper Intersect function requires what is essentially arrays, despite us only having one path each.
                Paths64 detectionPaths = new Paths64 { detectionPath };
                Paths64 polygonPaths = new Paths64 { polygonPath };

                Paths64 solution = new Paths64();
                //Finding the Intersection between the detection rectangle and the parking space
                solution = Clipper.Intersect(detectionPaths, polygonPaths, FillRule.NonZero);
                double overlapArea = 0;
                foreach (Path64 path in solution)
                {
                    overlapArea += Clipper.Area(path);
                }

                int objectArea = detection.Bounds.Width * detection.Bounds.Height;

                //the area of overlap has to be at least 50% of the area of the object that is doing the overlapping.
                double decimalOverlap = (overlapArea / objectArea);

                //first boolean weeds out overlaps that, in reality, wouldn't actually prevent a person from parking,
                //and the second boolean is for determining if a vehicle is properly parked or not.
                return [decimalOverlap >= 0.1, decimalOverlap >= 0.5];
            }

            return [false, false]; //placeholder return value
        }

        //used for protecting the VideoCapture so only one thread has access to it at a time.
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private async void ImageRecognition(int Id)
        {
            //in a rewrite, OpenCV could potentially be eliminated entirely.
            string outputPath = $"output_frame_{Id}.bmp";
            while (true)
            {
                //protect VideoCapture
                await _semaphore.WaitAsync();
                using (var capture = new VideoCapture(streamURL))
                {
                    if (!capture.IsOpened())
                    {
                        Console.WriteLine("ERROR: could not open camera stream.");
                        await sqlManager.CheckForMicrocontrollerData(Id);
                        //done with VideoCapture
                        _semaphore.Release();
                        Thread.Sleep(msTimeout);
                        //need to add a proper camera error state.
                        continue;
                    }

                    using (var frame = new Mat())
                    {
                        capture.Read(frame);
                        capture.Dispose();
                        //done with VideoCapture
                        _semaphore.Release();

                        if (frame.Empty())
                        {
                            Thread.Sleep(msTimeout);
                            continue; //skip the rest of the loop and come back to checking the capture.
                        }
                        else
                        {
                            //Save the Mat object to a BMP file using Cv2.ImWrite
                            Cv2.ImWrite(outputPath, frame);

                            Console.WriteLine($"Frame successfully saved to {outputPath}");
                        }


                        using (var frameClone = frame.Clone())
                        {
                            Cv2.PyrDown(frameClone, frameClone); //downscaling the frame so that it fits on screens and so that the user can select points.
                                                                 //The points will be scaled back up to the original frame size before being saved to the database.
                            OpenCvSharp.Point[][]? polygon = sqlManager.OpenPolygonEntry(Id);

                            //if no points saved...
                            if (polygon == null)
                            {
                                //will escape once specifically 4 points have been selected, since a parking spot is generally a quadrilateral.
                                while (true)
                                {
                                    List<OpenCvSharp.Point> points = new List<OpenCvSharp.Point>();

                                    Cv2.NamedWindow("Select ROI");
                                    Cv2.ResizeWindow("Select ROI", frameClone.Size());
                                    Cv2.SetMouseCallback("Select ROI", (MouseEventTypes @event, int x, int y, MouseEventFlags flags, IntPtr userData) =>
                                    {
                                        if (@event == MouseEventTypes.LButtonDown)
                                        {
                                            points.Add(new OpenCvSharp.Point(x, y));
                                            Cv2.Circle(frameClone, new OpenCvSharp.Point(x, y), 3, Scalar.Red, -1);
                                            Cv2.ImShow("Select ROI", frameClone);
                                        }
                                    });

                                    Cv2.ImShow("Select ROI", frameClone);
                                    Cv2.WaitKey(0);

                                    Mat mask = new Mat(frameClone.Size(), MatType.CV_8UC1, Scalar.All(0));

                                    if (points.Count == 4)
                                    {
                                        polygon = [points.ToArray()];

                                        Cv2.FillPoly(mask, polygon, Scalar.All(255)); //fill polygon with white

                                        //Need to scale the points back up to the original frame size before saving
                                        //to the database, since the user is selecting points on a downscaled version of the frame.
                                        for (int i = 0; i < polygon[0].Length; i++)
                                        {
                                            polygon[0][i].X *= 2;
                                            polygon[0][i].Y *= 2;
                                        }

                                        await sqlManager.createNewPolygonEntry(polygon, Id);
                                    }

                                    Mat dst = new Mat();
                                    Cv2.BitwiseAnd(frameClone, frameClone, dst, mask); //apply mask to the frame so that only the parking spot is visible,
                                                                                       //which will best show what the user just selected.
                                    Cv2.ImShow("ROI", dst);
                                    Cv2.WaitKey(0);

                                    if (points.Count == 4)
                                    {
                                        break; //escape condition for while loop
                                    }
                                }

                            }

                            // Start of YOLO object detection
                            using var predictor = new YoloPredictor("yolov8s.onnx");
                            //uses the camera frame that we saved as a BMP file as the input for the YOLO model, and gets the results back in a list of Detection objects.
                            YoloResult<Detection> result = await predictor.DetectAsync(outputPath);
                            Image<Rgba32> image = SixLabors.ImageSharp.Image.Load<Rgba32>(outputPath);
                            //a bit of a misnomer, since we're actually using YOLO for the object detection
                            OpenCVResultsItem openCVresult = new OpenCVResultsItem(Id, false, false);

                            foreach (Detection detection in result)
                            {
                                //Weed out detections that we don't care about, since they won't affect parking availability
                                if (!ignoredObjects.Contains(detection.Name.Name))
                                {
                                    var rectangle = new SixLabors.ImageSharp.RectangleF(
                                        detection.Bounds.X,
                                        detection.Bounds.Y,
                                        detection.Bounds.Width,
                                        detection.Bounds.Height);

                                    image.Mutate(ctx =>
                                    {
                                        ctx.Draw(Rgba32.ParseHex("FF0000"), 2, rectangle);
                                        ctx.DrawText(
                                            detection.Name.Name,
                                            SystemFonts.CreateFont("Arial", 32),
                                            Rgba32.ParseHex("FF0000"),
                                            new SixLabors.ImageSharp.PointF(detection.Bounds.X, detection.Bounds.Y + 20));
                                    });

                                    //the RectangleChecker function checks how much of the detected object is within the parking space polygon, and returns two booleans.
                                    //first boolean is if object in spot, second boolean is if the object (vehicle) is properly in the spot.
                                    bool[] objectInParkingSpace = RectangleChecker(detection, polygon);

                                    if (objectInParkingSpace[0] && !objectInParkingSpace[1])
                                    {
                                        //doesn't matter what the object is, if it's sufficiently within the parking space but not sufficiently within the parking space to
                                        //be considered parked, then we consider the parking space to be obstructed but not occupied by a vehicle.
                                        openCVresult = new OpenCVResultsItem(Id, false, true);
                                    }
                                    else if (objectInParkingSpace[0] && objectInParkingSpace[1])
                                    {
                                        if (acceptableVehicles.Contains(detection.Name.Name))
                                        {
                                            //if the detected object is a vehicle and it's sufficiently within the parking space to be considered parked, then we consider the parking space to be occupied by a vehicle.
                                            openCVresult = new OpenCVResultsItem(Id, true, true);
                                            break;
                                        }
                                        else
                                        {
                                            //if the detected object is not a vehicle, then we consider the parking space to be obstructed but not occupied by a vehicle.
                                            openCVresult = new OpenCVResultsItem(Id, false, true);
                                        }
                                    }                                        
                                }
                            }

                            await sqlManager.CreateNewOpenCVResultsEntry(openCVresult);
                            //in this function is where parking spaces will be created if there is microcontroller data.
                            await sqlManager.CheckForMicrocontrollerData(Id);

                            var pen = Pens.Solid(SixLabors.ImageSharp.Color.Blue, 12f);


                        //convert opencv points to sixlabors points
                        SixLabors.ImageSharp.PointF[] polygonSixLabors = new SixLabors.ImageSharp.PointF[]
                        {
                            new SixLabors.ImageSharp.PointF(polygon[0][0].X, polygon[0][0].Y),
                            new SixLabors.ImageSharp.PointF(polygon[0][1].X, polygon[0][1].Y),
                            new SixLabors.ImageSharp.PointF(polygon[0][2].X, polygon[0][2].Y),
                            new SixLabors.ImageSharp.PointF(polygon[0][3].X, polygon[0][3].Y)
                        };

                        image.Mutate(ctx =>
                        {
                            ctx.DrawPolygon(pen, polygonSixLabors);
                        });

                        try
                        {
                            image.SaveAsPng($"wwwroot/images/output_frame_with_detections_{Id}.png");
                        }
                        catch (Exception ex)
                        {
                        }

                        if (showDetections == true)
                        {
                            Mat final = Cv2.ImRead($"wwwroot/images/output_frame_with_detections_{Id}.png");
                            Cv2.PyrDown(final, final);

                            Cv2.ImShow("YOLO Detections", final);
                            Cv2.WaitKey(0);
                        }

                        else
                        {
                            Thread.Sleep(msTimeout);
                        }
                        }
                    }
                }                
            }
        }
    }
}
